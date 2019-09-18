
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LicenseSpring.Unity.Helpers;


namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// LicenseSpringWatcher, intended to be only as editor script.
    /// change note :   made this utils to only responsible to maintenance license spring unity manager game object, 
    ///                 all license initialization moved to LicenseSpringUnityManager
    /// change note :   (sept 18, 2019) - made this component as editor only protection level, game protection level will be
    ///                 managed elsewhere, and it will not create in gameobject to run license manager and license checker.
    /// 
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringAssets
    {
        private static string _Uid;

        private static LicenseManager _licenseManager;

        static LicenseSpringAssets()
        {
            //this is for asset differentiation.
            _Uid = GUID.Generate().ToString();

            //register editor application events.
            EditorApplication.update += OnEditorUpdateCycle;
            EditorApplication.projectChanged += OnProjectCompositionChanged;
            EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;

            //initialize license spring manager.
            InitLicenseManager();
        }

        public static void InitLicenseManager()
        {
            _licenseManager = (LicenseManager)LicenseManager.GetInstance();

            var licenseFilePath = Path.Combine(Application.dataPath, "License", "license.bin");
            LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
            {
                HardwareID = SystemInfo.deviceUniqueIdentifier,
                EnableLogging = false,
                CollectHostNameAndLocalIP = true,
                LicenseFilePath = licenseFilePath
            };

            //HACK : if there is no baked credential read at files.
            if (Helpers.LicenseApiConfigurationHelper.CheckLocalConfiguration())
            {
                var licenseLocalKey = Helpers.LicenseApiConfigurationHelper.ReadApiFileKey();

                var licenseConfig = new LicenseSpringConfiguration(licenseLocalKey.ApiKey,
                    licenseLocalKey.SharedKey,
                    licenseLocalKey.ProductCode,
                    licenseLocalKey.ApplicationName,
                    licenseLocalKey.ApplicationVersion,
                    licenseSpringExtendedOptions);

                _licenseManager.Initialize(licenseConfig);
            }
            else
            {
                if (Application.isEditor)
                    throw new UnityEngine.UnityException("No Api Configuration detected, Contact your asset Developer");
                else
                {
                    throw new UnityEngine.UnityException("UnAuthorized License Manager detected");
                }
            }

        }


        #region EditorEvents

        private static void OnProjectCompositionChanged()
        {
            RunLicenseWatchdog();
        }

        private static void OnEditorHierarchyChanged()
        {
            RunLicenseWatchdog();
        }


        private static void OnEditorUpdateCycle()
        {
            if(Time.realtimeSinceStartup % 30 == 0)
            {
                RunLicenseWatchdog();
            }
        }

        #endregion

        #region Internal Methods


        /// <summary>
        /// Seek or Creating license watcher game object when not found
        /// </summary>
        private static void RunLicenseWatchdog()
        {
            
        }
        
        #endregion
    }
}
