
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// LicenseSpringWatcher, intended to be only as editor script and only used as editor script.
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringWatcher
    {
        private static LicenseManager               _licenseManager;
        private static LocalKey                     _licenseLokalKey;

        private static LicenseSpringUnityManager    _licenseSpringUnityManager;

        private const string WATCH_NAME = "License Manager Runner";

        static LicenseSpringWatcher()
        {
            EditorApplication.update            += OnEditorUpdateCycle;
            EditorApplication.hierarchyChanged  += OnEditorHierarchyChanged;

            _licenseLokalKey = CheckLocalFileSettings();
            InitLicenseManager();
            InitLicenseWatchdog();
        }


        #region EditorEvents

        private static void OnEditorHierarchyChanged()
        {
            QueryLicenseWatchdog();
        }


        private static void OnEditorUpdateCycle()
        {
            if (_licenseManager.IsInitialized())
            {
                QueryLicenseWatchdog();
            }
        }

        #endregion

        #region Internal Methods


        private static void QueryLicenseWatchdog()
        {
            if (_licenseSpringUnityManager == null)
                InitLicenseWatchdog();
        }

        /// <summary>
        /// Creating license watcher game object
        /// </summary>
        private static void InitLicenseWatchdog()
        {
            _licenseSpringUnityManager = new GameObject(WATCH_NAME)
                .AddComponent<LicenseSpringUnityManager>();

            _licenseSpringUnityManager.AppLicenseManager = _licenseManager;
        }

        /// <summary>
        /// CheckLocalFileSettings, check local file cache for settings and load it to license manager
        /// </summary>
        private static LocalKey CheckLocalFileSettings()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "lic");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            //looking for file with skey extension
            var keys = Directory.GetFiles(folderPath, "*.skey");
            if(keys?.Length > 0)
            {
                var filePath = keys[0];
                return ReadApiFileKey(filePath);
            }
            else
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.ExitPlaymode();

                throw new UnityEngine.UnityException("Deployment Configuration not exist, contact publisher of this assets");
            }
        }

        /// <summary>
        /// Initialize license manager
        /// </summary>
        private static void InitLicenseManager()
        {
            var licenseFilePath = Path.Combine(Application.persistentDataPath, "lic.bin");
            LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
            {
                HardwareID = SystemInfo.deviceUniqueIdentifier,
                EnableLogging = false,
                CollectHostNameAndLocalIP = true,
                LicenseFilePath = licenseFilePath
            };

            var licenseConfig = new LicenseSpringConfiguration(_licenseLokalKey.ApiKey,
                _licenseLokalKey.SharedKey,
                _licenseLokalKey.ProductCode,
                _licenseLokalKey.ApplicationName,
                _licenseLokalKey.ApplicationVersion, 
                licenseSpringExtendedOptions);

            _licenseManager = (LicenseManager)LicenseManager.GetInstance();
            _licenseManager.Initialize(licenseConfig);
            
        }

        public static LocalKey ReadApiFileKey(string licenseApiKeyPath)
        {
            File.Decrypt(licenseApiKeyPath);

            using (FileStream fs = new FileStream(licenseApiKeyPath, FileMode.Open))
            {
                var bf = new BinaryFormatter();

                return (LocalKey)bf.Deserialize(fs);
            }
        }

        public static void WriteApiFileKey(LocalKey localKey, string saveFilePath)
        {
            using (FileStream fs = new FileStream(saveFilePath, FileMode.OpenOrCreate))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, localKey);
            }

            File.Encrypt(saveFilePath);
        }

        #endregion
    }
}
