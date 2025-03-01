using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AES
{
    /// <summary>
    /// AES-256-CBC
    /// </summary>
    public static class MessageByteCryption
    {
        public static byte[] GenerateKey()
        {
            Random rand = new Random();
            return Enumerable.Range(0, 32).Select(i => (byte)rand.Next(0, 256)).ToArray();
        }

        public static byte[] GenerateIV()
        {
            Random rand = new Random();
            return Enumerable.Range(0, 16).Select(i => (byte)rand.Next(0, 256)).ToArray();
        }

        public static byte[] Encrypt(byte[] bytes, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = GenerateIV();

                MemoryStream inputStream = new MemoryStream(bytes);
                MemoryStream outputStream = new MemoryStream();
                outputStream.Write(aes.IV, 0, aes.IV.Length);
                using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // 1Mb
                    byte[] buffer = new byte[1048576];
                    int readedLength;

                    while ((readedLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, readedLength);
                    }
                }
                return outputStream.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] bytes, byte[] key)
        {
            MemoryStream inputStream = new MemoryStream(bytes);
            MemoryStream outputStream = new MemoryStream();
            byte[] iv = new byte[16];
            inputStream.Read(iv, 0, iv.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // With CryptoStreamMode.Write
                using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    // 1Mb
                    byte[] buffer = new byte[1048576];
                    int readLength;

                    while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, readLength);
                    }
                }
                return outputStream.ToArray();
            }
        }
    }
}