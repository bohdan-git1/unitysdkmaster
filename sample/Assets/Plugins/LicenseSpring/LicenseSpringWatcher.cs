
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LicenseSpring.Unity.Helpers;

namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// LicenseSpringWatcher, intended to be only as editor script and only used as editor script.
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringWatcher
    {
        private static LicenseManager   _licenseManager;
        private static LocalKey         _licenseLokalKey;

        private static LicenseSpringUnityManager _licenseSpringUnityManager;

        private const string WATCH_NAME = "License Manager Runner";

        static LicenseSpringWatcher()
        {
            EditorApplication.update += OnEditorUpdateCycle;
            EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;

            _licenseLokalKey = CheckLocalFileSettings();
            if(_licenseLokalKey != null)
            {
                InitLicenseManager();
                InitLicenseWatchdog();
            }
            else
            {
                EditorApplication.ExecuteMenuItem("License Spring/Asset Author Licensing");
            }
        }


        #region EditorEvents

        private static void OnEditorHierarchyChanged()
        {
            QueryLicenseWatchdog();
        }


        private static void OnEditorUpdateCycle()
        {
            if (_licenseManager != null && _licenseManager.IsInitialized())
            {
                QueryLicenseWatchdog();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Watchdog routine to monitor wactcher game objects
        /// </summary>
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
            //sanity check if in case there is possibility that we miss out something
            if (_licenseManager != null)
            {
                _licenseSpringUnityManager = new GameObject(WATCH_NAME)
                    .AddComponent<LicenseSpringUnityManager>();
                _licenseSpringUnityManager.AppLicenseManager = _licenseManager;
            }
            else
            {
                throw new UnityEngine.UnityException("License Manager must be initiate first");
            }
        }

        /// <summary>
        /// CheckLocalFileSettings, check local file cache for settings and load it to license manager
        /// </summary>
        private static LocalKey CheckLocalFileSettings()
        {
           
            if (LicenseFileHelper.CheckLocalConfiguration())
            {
                return LicenseFileHelper.ReadApiFileKey();
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

       

        #endregion
    }
}
