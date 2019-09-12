using LicenseSpring.Unity.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LicenseSpring.Unity.Helpers
{
    /// <summary>
    /// Baked Identity Hack!..
    /// </summary>
    public class LicenseFileHelper
    {
        public static bool CheckLocalConfiguration(bool isDevMachine = true)
        {
            string folderPath = string.Empty;
            if (isDevMachine)
                folderPath = Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "LicenseSpring");
            else
                folderPath = Path.Combine(Directory.GetCurrentDirectory(), "LicenseSpring", "key.bin");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //looking for file with bin extension
            var keys = Directory.GetFiles(folderPath, "*.bin");
            if (keys.Length > 0)
                return true;
            else
                return false;
        }

        public static LicenseSpringLocalKey ReadApiFileKey(bool isDevMachine = true)
        {
            string saveFilePath = string.Empty;
            if (isDevMachine)
                saveFilePath = Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "LicenseSpring", "key.bin");
            else
                saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "LicenseSpring", "key.bin");

            var keyLen = Sodium.SecretBox.GenerateKey().Length;
            var noOnceLen = Sodium.SecretBox.GenerateNonce().Length;
            byte[] union = null;

            using (FileStream fs = new FileStream(saveFilePath, FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    union = ms.ToArray();
                }
            }

            var key = union.Take(keyLen).ToArray();
            var noonce = union.Skip(keyLen).Take(noOnceLen).ToArray();
            var cipher = union.Skip(keyLen + noOnceLen).ToArray();

            var actualData = Sodium.SecretBox.Open(cipher, noonce, key);
            return JsonConvert.DeserializeObject<LicenseSpringLocalKey>(Encoding.UTF8.GetString( actualData));
        }

        public static void WriteApiFileKey(LicenseSpringLocalKey localKey, bool isDevMachine = true)
        {
            string saveFilePath = string.Empty;
            if (isDevMachine)
                saveFilePath = Path.Combine(UnityEditor.EditorApplication.applicationContentsPath, "LicenseSpring", "key.bin");
            else
                saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "LicenseSpring", "key.bin");

            var jsonRep = JsonConvert.SerializeObject(localKey);

            //FINISH :sodium implementation.
            var key = Sodium.SecretBox.GenerateKey();
            var noonce = Sodium.SecretBox.GenerateNonce();
            var cipher = Sodium.SecretBox.Create(jsonRep, noonce, key);

            var union = new byte[key.Length + noonce.Length + cipher.Length];
            key.CopyTo(union, 0);
            noonce.CopyTo(union, key.Length);
            cipher.CopyTo(union, key.Length + noonce.Length);

            using (MemoryStream msCipher = new MemoryStream(union))
            {
                using (FileStream fs = new FileStream(saveFilePath, FileMode.OpenOrCreate))
                {
                    msCipher.CopyTo(fs);
                }
            }
        }
    }
}
