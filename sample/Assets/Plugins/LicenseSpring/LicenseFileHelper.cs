using LicenseSpring.Unity.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LicenseSpring.Unity.Helpers
{
    public class LicenseFileHelper
    {
        public static bool CheckLocalConfiguration()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "lic");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //looking for file with skey extension
            var keys = Directory.GetFiles(folderPath, "*.skey");
            if (keys.Length > 0)
                return true;
            else
                return false;
        }

        public static LocalKey ReadApiFileKey()
        {
            var saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "lic", "key.skey");
            
            //File.Decrypt(saveFilePath);

            using (FileStream fs = new FileStream(saveFilePath, FileMode.Open))
            {
                
                var bf = new BinaryFormatter();
                return (LocalKey)bf.Deserialize(fs);
            }
        }

        public static void WriteApiFileKey(LocalKey localKey)
        {
            var saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "lic", "key.skey");
            var jsonRep = UnityEngine.JsonUtility.ToJson(localKey);

            //TODO :sodium implementation, just hack
            var key = Sodium.SecretBox.GenerateKey();
            var noonce = Sodium.SecretBox.GenerateNonce();

            var cipher = Sodium.SecretBox.Create(jsonRep, noonce, key);
            using (MemoryStream msCipher = new MemoryStream(cipher))
            {
                using (FileStream fs = new FileStream(saveFilePath, FileMode.OpenOrCreate))
                {
                    msCipher.CopyTo(fs);
                    var bf = new BinaryFormatter();
                    bf.Serialize(fs, localKey);
                }
            }
        }
    }
}
