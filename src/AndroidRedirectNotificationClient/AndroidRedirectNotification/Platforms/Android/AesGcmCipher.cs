using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification.Platforms.Android
{
    internal static class AesGcmCipher
    {
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public static byte[] Encrypt(byte[] plaintext, byte[] key32)
        {
            if (key32.Length != 32)
                throw new ArgumentException("AES-256 key must be 32 bytes.", nameof(key32));

            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[plaintext.Length];

            using var aesGcm = new AesGcm(key32, TagSize);
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

            // [Nonce (12)][Tag (16)][Ciphertext (N)]
            byte[] output = new byte[NonceSize + TagSize + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, output, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, output, NonceSize, TagSize);
            Buffer.BlockCopy(ciphertext, 0, output, NonceSize + TagSize, ciphertext.Length);
            return output;
        }

        public static byte[] Decrypt(byte[] payload, byte[] key32)
        {
            if (key32.Length != 32)
                throw new ArgumentException("AES-256 key must be 32 bytes.", nameof(key32));

            if (payload.Length < NonceSize + TagSize)
                throw new CryptographicException("Invalid AES-GCM payload length.");

            byte[] nonce = payload.AsSpan(0, NonceSize).ToArray();
            byte[] tag = payload.AsSpan(NonceSize, TagSize).ToArray();
            byte[] ciphertext = payload.AsSpan(NonceSize + TagSize).ToArray();
            byte[] plaintext = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(key32, TagSize);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
            return plaintext;
        }
    }
}
