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
    /// Change :    Should automatically check if this is dev machine or not by looking at set of places.
    ///             eg : if an api config files located at UnityEditor.EditorApplication.applicationContentsPath than it's 
    ///                 assumed as developer machine.
    /// </summary>
    public class LicenseApiConfigurationHelper
    {
        static readonly string standardFolderPath = Path.Combine(UnityEngine.Application.dataPath, "Plugins", "LicenseSpring");
        static readonly string developerFolderPath = Path.Combine(UnityEngine.Application.persistentDataPath, "LicenseSpring");

        static LicenseApiConfigurationHelper()
        {
            //create this path, to prevent error.
            //TODO :find more elegant path to avoid folder not found!
            
            var existDevPath = Directory.Exists(developerFolderPath);
            if (!existDevPath)
                Directory.CreateDirectory(developerFolderPath);

            var existStandardPath = Directory.Exists(standardFolderPath);
            if (!existStandardPath)
                Directory.CreateDirectory(standardFolderPath);
            
        }

        /// <summary>
        /// check existance of local config.
        /// </summary>
        /// <param name="isDevMachine"></param>
        /// <returns></returns>
        public static bool IsExistDeployedConfig()
        {
            try
            {
                //development machine autodetections.
                var nonDevMachineConfiFiles = Directory.GetFiles(standardFolderPath, "*.bin", SearchOption.AllDirectories);
                return nonDevMachineConfiFiles.Length > 0;
            }
            catch(DirectoryNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsExistDeveloperConfig()
        {
            try
            {
                var files = Directory.GetFiles(developerFolderPath, "*.bin",
                  SearchOption.AllDirectories);

                return files.Length > 0;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Reading api configuration key and detect if this key is developer key or deployment key.
        /// </summary>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public static LicenseSpringLocalKey ReadApiFileKey(string projectname = "", bool  isDevMachine = false)
        {
            string saveFilePath = string.Empty;

            if (isDevMachine)
            {
                //check this path, if this is development machine this key should not included inside asset folder.
                saveFilePath = Path.Combine(developerFolderPath, $"{projectname}.bin");
            }
            else
                saveFilePath = Path.Combine(standardFolderPath, $"{projectname}.bin");

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

        public static void WriteApiFileKey(LicenseSpringLocalKey localKey, bool isDevMachine = false)
        {
            string saveFilePath = string.Empty;
            if (isDevMachine)
                saveFilePath = Path.Combine(developerFolderPath, $"{UnityEngine.Application.productName}.bin");
            else
                saveFilePath = Path.Combine(standardFolderPath, $"{UnityEngine.Application.productName}.bin");

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
