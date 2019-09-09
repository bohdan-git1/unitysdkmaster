
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// LicenseSpringWatcher, intended to be only as editor script and only used as editor script.
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringWatcher
    {
        private static LicenseManager   _licenseManager;
        private static bool             _isAuthorMode;
        private static string           _apiKeyPath;

        static LicenseSpringWatcher()
        {
            EditorApplication.update            += OnEditorUpdateCycle;
            EditorApplication.hierarchyChanged  += OnEditorHierarchyChanged;

            CheckLocalFileSettings(out _apiKeyPath);
            InitLicenseManager();
        }

        #region EditorEvents

        private static void OnEditorHierarchyChanged()
        {

        }

        private static void OnEditorUpdateCycle()
        {
            
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// CheckLocalFileSettings, check local file cache for settings and load it to license manager
        /// </summary>
        private static bool CheckLocalFileSettings(out string filePath)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "lic");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            //looking for file with skey extension
            var keys = Directory.GetFiles(folderPath, "*.skey");
            if(keys?.Length > 0)
            {
                filePath = keys[0];
                return true;
            }
            else
            {
                if (EditorApplication.isPlaying)
                    EditorApplication.ExitPlaymode();

                filePath = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Initialize license manager
        /// </summary>
        private static void InitLicenseManager()
        {
            LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
            {
                HardwareID = SystemInfo.deviceUniqueIdentifier,
                EnableLogging = false,
                CollectHostNameAndLocalIP = true,
                LicenseFilePath = licPath
            };

            

            _licenseConfig = new LicenseSpringConfiguration(_api, _skey,
                _prodCode,
                _appName,
                _appVersion,
                extendedOptions: licenseSpringExtendedOptions);

            //initializing manually
            _licenseManager.Initialize(_licenseConfig);
        }

        #endregion
    }
}
