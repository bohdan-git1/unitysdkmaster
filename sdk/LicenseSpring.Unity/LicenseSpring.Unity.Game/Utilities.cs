using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SecurityDriven.Inferno;
using UnityEngine;

namespace LicenseSpring.Unity.Game
{

    public static class KeyStorage
    {
        public static LSLocalKey ReadLocalKey()
        {
            LSLocalKey lSLocalKey = null;
            string keyPath = Path.Combine(Application.persistentDataPath, "api.bin");

            var content = File.ReadAllText(keyPath);
            content = SecureStorage.Decrypt(content);

            lSLocalKey = LSLocalKey.FromString(content);
            return lSLocalKey;
        }

        public static void SaveLocalKey(LSLocalKey localKey, string password)
        {
            try
            {
                var savePath = Path.Combine(Application.persistentDataPath, "api.bin");
                var encryptResult = SecureStorage.Encrypt(localKey.ToString(), password);

                using (StreamWriter writer = new StreamWriter(savePath))
                {
                    writer.Write(encryptResult.Item1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public static class SecureStorage
    {
        private static byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private static int BlockSize = 128;

        public static (string, byte[]) Encrypt(string content, string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(content);
            byte[] passBytes = Encoding.Unicode.GetBytes(password);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.BlockSize = BlockSize;
            crypt.Key = hash.ComputeHash(passBytes);
            crypt.IV = IV;

            byte[] sum = new byte[] { };
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }
                sum = crypt.Key.Concat(memoryStream.ToArray()).ToArray();
            }
            return (Convert.ToBase64String(sum), sum);
        }

        public static string Decrypt(string content)
        {
            byte[] contentBytes = Convert.FromBase64String(content);
            byte[] bytes = contentBytes.Skip(16).ToArray();
            byte[] passBytes = contentBytes.Take(16).ToArray();

            string result = string.Empty;

            SymmetricAlgorithm crypt = Aes.Create();
            crypt.Key = passBytes;
            crypt.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    result = Encoding.Unicode.GetString(decryptedBytes);
                }
            }

            return result;
        }
    }
}
