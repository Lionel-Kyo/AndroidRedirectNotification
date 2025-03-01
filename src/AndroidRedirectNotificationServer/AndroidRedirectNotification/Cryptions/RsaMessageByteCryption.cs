using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RSA
{
    public class CryptionKeys
    {
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        public CryptionKeys(byte[] publicKey, byte[] privateKey)
        {
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;
        }
    }
    /// <summary>
    /// name: "RSA-OAEP", modulusLength: 2048, hash: "SHA-256",
    /// public key: "spki", private key: "pkcs8"
    /// </summary>
    public class MessageByteCryption
    {
        public static CryptionKeys GenerateRsaKeys()
        {
            var rsa = System.Security.Cryptography.RSA.Create(2048);
            // "spki"
            byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
            // "pkcs8"
            byte[] privateKey = rsa.ExportPkcs8PrivateKey();
            return new CryptionKeys(publicKey, privateKey);
        }

        public static byte[] EncryptRsa(byte[] bytes, byte[] publicKey)
        {
            if (bytes.Length >= 190)
                throw new ArgumentException("bytes length > 190");

            var rsa = System.Security.Cryptography.RSA.Create(2048);
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            return rsa.Encrypt(bytes, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] DecryptRsa(byte[] bytes, byte[] privateKey)
        {
            var rsa = System.Security.Cryptography.RSA.Create(2048);
            rsa.ImportPkcs8PrivateKey(privateKey, out _);
            return rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256);
        }
    }
}
