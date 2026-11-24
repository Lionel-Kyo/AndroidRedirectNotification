using NSec.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PublicKey = NSec.Cryptography.PublicKey;

namespace AndroidRedirectNotification
{
    internal sealed class MyTcpListener : IDisposable
    {
        public const string DefaultSigningKeyAsset = "AndroidRedirectNotification.Assets.server_ed25519.key";
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public event Action<MyNotificationData>? OnMessageReceived;

        private readonly Key _signingKey;
        private Task? listeningTask;
        private TcpListener? listener;
        private CancellationTokenSource? cancelToken;
        private bool _disposed;

        public ushort Port { get; private set; }

        public MyTcpListener(ushort port)
        {
            Port = port;
            var ed25519 = SignatureAlgorithm.Ed25519;

            _signingKey = LoadSigningKey(ed25519);
        }

        private static Key LoadSigningKey(SignatureAlgorithm ed25519)
        {
            try
            {
                var assembly = typeof(MyTcpListener).Assembly;
                using Stream? stream = assembly.GetManifestResourceStream(DefaultSigningKeyAsset);
                if (stream == null)
                {
                    throw new FileNotFoundException(
                        $"Embedded signing key resource '{DefaultSigningKeyAsset}' was not found.");
                }

                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                byte[] embeddedKey = ms.ToArray();

                if (embeddedKey.Length == 0)
                {
                    throw new CryptographicException(
                        $"Embedded signing key resource '{DefaultSigningKeyAsset}' is empty.");
                }

                return Key.Import(ed25519, embeddedKey, KeyBlobFormat.RawPrivateKey);
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                throw;
            }
        }

        private static Key DeriveSessionKey(
            SharedSecret? sharedSecret,
            byte[] serverEphemeralPub,
            byte[] clientEphemeralPub,
            AeadAlgorithm aesGcm)
        {
            if (sharedSecret == null)
                throw new CryptographicException("X25519 key agreement failed.");

            var hkdf = KeyDerivationAlgorithm.HkdfSha256;
            byte[] salt = SHA256.HashData(serverEphemeralPub.Concat(clientEphemeralPub).ToArray());
            byte[] info = Encoding.UTF8.GetBytes("AndroidRedirectNotification-X25519-AES256GCM-v1");

            return hkdf.DeriveKey(
                sharedSecret,
                salt,
                info,
                aesGcm,
                new KeyCreationParameters { ExportPolicy = KeyExportPolicies.None });
        }

        private static byte[] EncryptAes256Gcm(AeadAlgorithm aesGcm, byte[] plaintext, Key key)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] ciphertext = aesGcm.Encrypt(key, nonce, ReadOnlySpan<byte>.Empty, plaintext);

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

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream networkStream = client.GetStream())
                {
                    var stream = new MyNetworkStream(networkStream);
                    var x25519 = KeyAgreementAlgorithm.X25519;
                    var ed25519 = SignatureAlgorithm.Ed25519;
                    var aesGcm = AeadAlgorithm.Aes256Gcm;

                    // 1. Generate ephemeral X25519 keypair and sign its public key with persistent Ed25519 key
                    using var serverEphemeral = Key.Create(x25519);
                    byte[] serverEphPub = serverEphemeral.Export(KeyBlobFormat.RawPublicKey);
                    byte[] signature = ed25519.Sign(_signingKey, serverEphPub);

                    // Send [32-byte X25519 PubKey + 64-byte Ed25519 Signature] = 96 bytes
                    byte[] handshakePacket = new byte[serverEphPub.Length + signature.Length];
                    Buffer.BlockCopy(serverEphPub, 0, handshakePacket, 0, serverEphPub.Length);
                    Buffer.BlockCopy(signature, 0, handshakePacket, serverEphPub.Length, signature.Length);
                    await stream.WriteAsync(handshakePacket);

                    // 2. Receive client's ephemeral X25519 public key (32 bytes)
                    byte[] clientEphPubBytes = await stream.ReadAsync();
                    if (clientEphPubBytes.Length != x25519.PublicKeySize)
                        throw new CryptographicException("Invalid client X25519 public key length.");

                    var clientEphPub = PublicKey.Import(x25519, clientEphPubBytes, KeyBlobFormat.RawPublicKey);

                    // 3. Derive ephemeral AES-256-GCM session key
                    using SharedSecret? sharedSecret = x25519.Agree(serverEphemeral, clientEphPub);
                    using Key sessionKey = DeriveSessionKey(sharedSecret, serverEphPub, clientEphPubBytes, aesGcm);

                    // 4. Receive and decrypt MyNotificationData
                    byte[] encryptedMsg = await stream.ReadAsync();
                    byte[] utf8Message = DecryptAes256Gcm(aesGcm, encryptedMsg, sessionKey);
                    string jsonMessage = Encoding.UTF8.GetString(utf8Message);

                    var data = JsonSerializer.Deserialize(jsonMessage, AppJsonSerializerContext.Default.MyNotificationData);
                    if (data != null)
                        OnMessageReceived?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
            }
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            cancelToken = new CancellationTokenSource();
            listeningTask = StartListeningAsync(cancelToken.Token);
        }

        public async Task StopAsync()
        {
            if (cancelToken == null)
                return;

            cancelToken.Cancel();
            listener?.Stop();

            if (listeningTask != null)
                await listeningTask;

            cancelToken.Dispose();
            cancelToken = null;
            listener = null;
            listeningTask = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _ = StopAsync();
            _signingKey.Dispose();
        }

        private async Task StartListeningAsync(CancellationToken token)
        {
            if (listener == null)
                return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(token);
                    _ = Task.Run(() => HandleClientAsync(client), token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ExceptionRecord.AddExceptionRecord(ex);
                }
            }
        }
    }
}
