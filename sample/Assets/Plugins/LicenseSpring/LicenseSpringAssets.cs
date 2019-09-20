
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LicenseSpring.Unity.Helpers;
using LicenseSpring.Unity.Components;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// License spring asset protection system, editor script only.
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringAssets
    {
        private static string _Uid;

        private static LicenseManager               _licenseManager;
        private static LicenseSpringNotification    _licenseSpringNotification;
        private static bool                         _IsDeveloperMode = false;

        public static void DeveloperToggleTestMode()
        {
            _IsDeveloperMode = !_IsDeveloperMode;
        }

        public static LicenseStatus GetLicenseStatus()
        {
            if (_licenseManager == null || !_licenseManager.IsInitialized())
                return LicenseStatus.Unknown;

            if (_licenseManager.CurrentLicense() == null)
                return LicenseStatus.Unknown;

            return _licenseManager.CurrentLicense().Status();
        }

        public static bool GetDeveloperStatus()
        {
            if (_licenseManager == null || !_licenseManager.IsInitialized())
                return false;

            return false;
        }

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
            try
            {
                _licenseManager = (LicenseManager)LicenseManager.GetInstance();

                //path for registered license.
                var licenseFilePath = Path.Combine(Application.dataPath, "Plugins", "LicenseSpring", "License", "license.bin");

                //init extended options of License spring configs.
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
                    //reading api configuration from 2 place, depend on this is development machine or not.
                    var _licenseSpringLocalKey = Helpers.LicenseApiConfigurationHelper.ReadApiFileKey();

                    var licenseConfig = new LicenseSpringConfiguration(_licenseSpringLocalKey.ApiKey,
                        _licenseSpringLocalKey.SharedKey,
                        _licenseSpringLocalKey.ProductCode,
                        _licenseSpringLocalKey.ApplicationName,
                        _licenseSpringLocalKey.ApplicationVersion,
                        licenseSpringExtendedOptions);

                    _IsDeveloperMode = _licenseSpringLocalKey.IsDevelopment;

                    _licenseManager.Initialize(licenseConfig);
                }
                else
                {
                    throw new UnityEngine.UnityException("No Api Configuration detected, Contact your asset Developer");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

        }


        #region EditorEvents

        private static void OnProjectCompositionChanged()
        {

            if(!_IsDeveloperMode)
                RunLicenseWatchdog();
        }

        private static void OnEditorHierarchyChanged()
        {
            if (!_IsDeveloperMode)
                RunLicenseWatchdog();
        }

        static  bool    snapshoot = true;
        static  float   timer = 0;
        static  float   lastSnapshootTime = 0;
        const   int     TimeInterval = 60;

        private static void OnEditorUpdateCycle()
        {
            if (_IsDeveloperMode)
                return;

            //cycle all rendering 
            timer = Time.realtimeSinceStartup;
            if ((timer - lastSnapshootTime) >= TimeInterval)
                snapshoot = true;

            if (snapshoot)
            {
                //Debug.Log($"Snapshoot! at {Time.realtimeSinceStartup}");
                RunLicenseWatchdog();

                lastSnapshootTime = Time.realtimeSinceStartup;
                snapshoot = false;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Utility tools to find all editorwindow.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Type> GetWindowTypes()
        {
            var baseType = typeof(UnityEditor.EditorWindow);
            var requiredAttribute = baseType.Assembly.GetType("UnityEditor.EditorWindowTitleAttribute");

            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   where baseType.IsAssignableFrom(type) 
                   select type;
        }

        /// <summary>
        /// Run license checking service
        /// </summary>
        private static void RunLicenseWatchdog()
        {
            var currentSceneCamera = Camera.main;
            bool? licenseIsInitialized = _licenseManager?.IsInitialized();

            if (!licenseIsInitialized.HasValue || !licenseIsInitialized.Value)
            {
                InitLicenseManager();
                return;
            }

            var license = (License)_licenseManager.CurrentLicense();
            if (license == null)
                RunUnlicensedProduct(currentSceneCamera.gameObject);

            if (license.IsActive())
                RunRegisteredProduct(currentSceneCamera.gameObject);

            if (license.IsTrial())
                RunTrialProduct(currentSceneCamera.gameObject);

            if (license.IsExpired())
                RunProductExpired(currentSceneCamera.gameObject);

        }

        private static void RunProductExpired(GameObject cameraGameObject)
        {
            var notification = cameraGameObject.GetComponent<LicenseSpringNotification>();
            if (notification != null)
            {
                notification.enabled = true;
                notification.SetStatus(LicenseStatus.Expired);
            }

            throw new UnityEngine.UnityException("Product license/trial is expired, contact author/publisher");
        }

        private static void RunTrialProduct(GameObject cameraGameObject)
        {
            var notification = cameraGameObject.GetComponent<LicenseSpringNotification>();
            if (notification != null)
            {
                notification.SetStatus(LicenseStatus.Active);
                notification.enabled = false;
            }
            Debug.Log("Product in trial mode ");
        }

        private static void RunRegisteredProduct(GameObject cameraGameObject)
        {
            var notification = cameraGameObject.GetComponent<LicenseSpringNotification>();
            if (notification != null)
            {
                notification.SetStatus(LicenseStatus.Active);
                notification.enabled = false;
            }
        }

        private static void RunUnlicensedProduct(GameObject cameraGameObject)
        {
            OpenLicenseRegistrationForm();
            var notification = cameraGameObject.GetComponent<LicenseSpringNotification>();
            if (notification != null)
            {
                notification.enabled = true;
                notification.SetStatus(LicenseStatus.Unknown);
            }

            throw new UnityEngine.UnityException("This Product is not registered, please contact author/publisher");
        }

        private static void OpenLicenseRegistrationForm()
        {
            //always exit playmode.
            EditorApplication.ExitPlaymode();
            //var window = GetWindowTypes().Single(s=>s.FullName.Contains("Registration"));
            //open registration windows
            var menuItem = new MenuItem("License Spring/Registration");
            EditorApplication.ExecuteMenuItem(menuItem.menuItem);
        }

        #endregion
    }
}
