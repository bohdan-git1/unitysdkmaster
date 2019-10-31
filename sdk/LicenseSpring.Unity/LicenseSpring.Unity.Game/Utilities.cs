using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SecurityDriven.Inferno;

namespace LicenseSpring.Unity.Game
{
    public static class Utilities
    {
        public static string Sha256String(string value)
        {
            StringBuilder sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var item in result)
                {
                    sb.Append(item.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        public static bool CompareHash(string hashString, string value)
        {
            var valueHashed = Sha256String(value);
            return hashString == valueHashed;
        }

        public static LSLocalKey ReadLocalKey(string keyPath,string password)
        {
            LSLocalKey lSLocalKey = null;
            var hashPass = Sha256String(password);
            var passBytes = SecurityDriven.Inferno.Utils.SafeUTF8.GetBytes(hashPass);

            using (var stream = File.OpenRead(keyPath))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var streamBytesArray = ms.ToArray();

                    var cipher = new ArraySegment<byte>(streamBytesArray);
                    var isAuthenticated = SuiteB.Authenticate(passBytes, cipher);

                    if (isAuthenticated)
                    {
                        var decryptedBytes = SuiteB.Decrypt(passBytes, cipher);
                        var strValue = SecurityDriven.Inferno.Utils.SafeUTF8.GetString(decryptedBytes);
                        lSLocalKey = new LSLocalKey();
                    }
                }
            }

            return lSLocalKey;
        }

    }
}
