using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AndroidRedirectNotification.Platforms.Android
{
    internal sealed class MyNetworkStream
    {
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const string DefaultServerPubKeyAsset = "server_ed25519.pub";
        private static byte[]? TrustedServerEd25519PublicKey;

        public NetworkStream Stream { get; private set; }

        public MyNetworkStream(NetworkStream stream)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Write(Span<byte> bytes)
        {
            Stream.Write(BitConverter.GetBytes(bytes.Length));
            Stream.Write(bytes);
        }

        public async Task WriteAsync(byte[] bytes)
        {
            await Stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await Stream.WriteAsync(bytes);
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> bytes)
        {
            await Stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await Stream.WriteAsync(bytes);
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[4];
            int bytesRead = Stream.Read(buffer, 0, buffer.Length);
            if (bytesRead < 4)
                throw new IOException("Failed to read the message length.");

            int expectedLength = BitConverter.ToInt32(buffer, 0);
            if (expectedLength <= 0)
                throw new InvalidOperationException("Invalid data length received.");

            buffer = new byte[expectedLength];
            int totalRead = 0;

            while (totalRead < expectedLength)
            {
                int remaining = expectedLength - totalRead;
                int read = Stream.Read(buffer, totalRead, remaining);
                if (read == 0)
                    throw new IOException("Connection closed");

                totalRead += read;
            }

            return buffer;
        }

        public async Task<byte[]> ReadAsync()
        {
            byte[] buffer = new byte[4];
            int bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead < 4)
                throw new IOException("Failed to read the message length.");

            int expectedLength = BitConverter.ToInt32(buffer, 0);
            if (expectedLength <= 0)
                throw new InvalidOperationException("Invalid data length received.");

            buffer = new byte[expectedLength];
            int totalRead = 0;

            while (totalRead < expectedLength)
            {
                int remaining = expectedLength - totalRead;
                int read = await Stream.ReadAsync(buffer, totalRead, remaining);
                if (read == 0)
                    throw new IOException("Connection closed");

                totalRead += read;
            }

            return buffer;
        }

        private static async Task<byte[]> LoadTrustedServerPublicKeyAsync()
        {
            try
            {
                await using Stream appPackageStream = await FileSystem.OpenAppPackageFileAsync(DefaultServerPubKeyAsset);
                using var ms = new MemoryStream();
                await appPackageStream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (FileNotFoundException)
            {
                string fallbackPath = Path.Combine(FileSystem.AppDataDirectory, DefaultServerPubKeyAsset);
                if (File.Exists(fallbackPath))
                    return await File.ReadAllBytesAsync(fallbackPath);

                throw;
            }
        }

        /// <summary>
        /// Loads the pinned server Ed25519 public key, performs X25519 key agreement
        /// authenticated by the server's Ed25519 signature, derives an AES-256-GCM session key via
        /// HKDF-SHA256, and sends the encrypted JSON payload.
        /// </summary>
        public async Task SendEncryptedMessageAsync(string jsonPayload)
        {
            byte[] trustedServerEd25519PublicKey = TrustedServerEd25519PublicKey ?? await LoadTrustedServerPublicKeyAsync();
            if (TrustedServerEd25519PublicKey == null)
            {
                TrustedServerEd25519PublicKey = trustedServerEd25519PublicKey;
            }

            var ed25519 = SignatureAlgorithm.Ed25519;
            var x25519 = KeyAgreementAlgorithm.X25519;
            var hkdf = KeyDerivationAlgorithm.HkdfSha256;
            var aesGcm = AeadAlgorithm.Aes256Gcm;

            if (trustedServerEd25519PublicKey.Length != ed25519.PublicKeySize)
                throw new CryptographicException("Invalid trusted server Ed25519 public key length.");

            // 1. Read [32-byte Server X25519 Ephemeral PubKey + 64-byte Ed25519 Signature]
            byte[] handshakePacket = await ReadAsync();
            if (handshakePacket.Length != x25519.PublicKeySize + ed25519.SignatureSize)
                throw new CryptographicException("Invalid handshake packet length.");

            byte[] serverEphPubBytes = handshakePacket.AsSpan(0, x25519.PublicKeySize).ToArray();
            byte[] signatureBytes = handshakePacket.AsSpan(x25519.PublicKeySize, ed25519.SignatureSize).ToArray();

            // 2. Verify server's Ed25519 signature against pinned server public key
            var serverSignPubKey = PublicKey.Import(ed25519, trustedServerEd25519PublicKey, KeyBlobFormat.RawPublicKey);
            if (!ed25519.Verify(serverSignPubKey, serverEphPubBytes, signatureBytes))
                throw new CryptographicException("Server Ed25519 signature verification failed.");

            // 3. Generate client ephemeral X25519 keypair and send public key
            using var clientEphemeral = Key.Create(x25519);
            byte[] clientEphPubBytes = clientEphemeral.Export(KeyBlobFormat.RawPublicKey);
            await WriteAsync(clientEphPubBytes);

            // 4. Derive AES-256-GCM key via X25519 + HKDF-SHA256
            var serverEphPubKey = PublicKey.Import(x25519, serverEphPubBytes, KeyBlobFormat.RawPublicKey);
            using SharedSecret? sharedSecret = x25519.Agree(clientEphemeral, serverEphPubKey);
            if (sharedSecret == null)
                throw new CryptographicException("X25519 key agreement failed.");

            byte[] salt = SHA256.HashData(serverEphPubBytes.Concat(clientEphPubBytes).ToArray());
            byte[] info = Encoding.UTF8.GetBytes("AndroidRedirectNotification-X25519-AES256GCM-v1");

            using var sessionKey = hkdf.DeriveKey(
                sharedSecret,
                salt,
                info,
                aesGcm,
                new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None });

            // 5. Encrypt and send notification JSON
            byte[] encryptedPayload = EncryptAes256Gcm(aesGcm, Encoding.UTF8.GetBytes(jsonPayload), sessionKey);
            await WriteAsync(encryptedPayload);
        }

        private static byte[] EncryptAes256Gcm(AeadAlgorithm aesGcm, byte[] plaintext, Key key)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] ciphertext = aesGcm.Encrypt(key, nonce, ReadOnlySpan<byte>.Empty, plaintext);

            // Wire format: [12-byte Nonce][Ciphertext + 16-byte Tag produced by NSec Encrypt]
            byte[] packet = new byte[NonceSize + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, packet, 0, NonceSize);
            Buffer.BlockCopy(ciphertext, 0, packet, NonceSize, ciphertext.Length);
            return packet;
        }

        private static byte[] DecryptAes256Gcm(AeadAlgorithm aesGcm, byte[] packet, Key key)
        {
            if (packet.Length < NonceSize + TagSize)
                throw new CryptographicException("Invalid AES-256-GCM packet length.");

            byte[] nonce = packet.AsSpan(0, NonceSize).ToArray();
            byte[] ciphertextWithTag = packet.AsSpan(NonceSize).ToArray();

            byte[]? plaintext = aesGcm.Decrypt(key, nonce, ReadOnlySpan<byte>.Empty, ciphertextWithTag);
            if (plaintext == null)
                throw new CryptographicException("AES-256-GCM authentication or decryption failed.");

            return plaintext;
        }
    }
}
