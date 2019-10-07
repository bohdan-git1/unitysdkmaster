
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using LicenseSpring.Unity.Components;
using System.Collections.Generic;
using System.Linq;

namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// License spring asset protection system, editor script only.
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringUnityAssets
    {
        private static string _Uid;

        private static LicenseManager               _licenseManager;
        private static LicenseSpringNotification    _licenseSpringNotification;
        private static bool                         _IsDeveloperMode = false;

        /// <summary>
        /// Toggle developer/testing mode, by enable/disable all license checking routine
        /// </summary>
        public static void DeveloperToggleTestMode()
        {
            _IsDeveloperMode = !_IsDeveloperMode;
        }

        /// <summary>
        /// Get license manager initialization status
        /// </summary>
        /// <returns></returns>
        public static bool GetInitializeStatus()
        {
            if (_licenseManager == null)
                return false;

            return _licenseManager.IsInitialized();
        }

        /// <summary>
        /// Get Current installed license status.
        /// </summary>
        /// <returns></returns>
        public static LicenseStatus GetLicenseStatus()
        {
            if (_licenseManager == null || !_licenseManager.IsInitialized())
                return LicenseStatus.Unknown;

            if (_licenseManager.CurrentLicense() == null)
                return LicenseStatus.Unknown;

            return _licenseManager.CurrentLicense().Status();
        }

        /// <summary>
        /// Get Current State of Editor for Dev.
        /// </summary>
        /// <returns></returns>
        public static bool GetDeveloperStatus()
        {
            return _IsDeveloperMode && GetInitializeStatus();
        }

        /// <summary>
        /// Get current installed license.
        /// </summary>
        /// <returns></returns>
        public static License GetCurrentLicense()
        {
            return (License) _licenseManager.CurrentLicense();
        }

        /// <summary>
        /// Request demo/trial key
        /// </summary>
        /// <param name="email">your individual email</param>
        /// <returns></returns>
        public static  string RequestDemo(string email)
        {
            var key = _licenseManager.GetTrialKey(email);
            _licenseManager.ActivateLicense(key);

            return key;
        }

        public static void Register(string key)
        {
            _licenseManager.ActivateLicense(key);
        }

        public static void ResetLicense()
        {
            if (_licenseManager != null && _licenseManager.IsInitialized())
            {
                //_licenseManager.ClearLocalStorage();
                var licenseFilePath = Path.Combine(Application.dataPath, "Plugins", "LicenseSpring", "License",
                    $"{Application.productName}.bin");

                File.Delete(licenseFilePath);
            }
        }

        static LicenseSpringUnityAssets()
        {
            //this is for asset differentiation.
            _Uid = GUID.Generate().ToString();

            //register editor application events.
            EditorApplication.update += OnEditorUpdateCycle;
            EditorApplication.projectChanged += OnProjectCompositionChanged;
            EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;

            //initialize license spring manager.
            InitLicenseManager();
            //init notification systme
            //InitNotificationSystem();
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
        const   int     TimeInterval = 30;

        private static void OnEditorUpdateCycle()
        {
            if (GetDeveloperStatus())
            {
                //disable screen blit
                var notify = InitNotificationSystem();
                notify.enabled = false;
                return;
                
            }

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


        private static LicenseSpringNotification InitNotificationSystem()
        {
            var currentCamera = Camera.main;
            var licenseSpringNotification = currentCamera.GetComponent<LicenseSpringNotification>();
            if(licenseSpringNotification == null)
                licenseSpringNotification = currentCamera.gameObject.AddComponent<LicenseSpringNotification>();
            else
                licenseSpringNotification.SetStatus(LicenseStatus.Unknown);

            return licenseSpringNotification;
        }

        private static void InitLicenseManager()
        {
            try
            {
                _IsDeveloperMode = false;

                _licenseManager = (LicenseManager)LicenseManager.GetInstance();

                //path for registered license.
                var licenseFilePath = Path.Combine(Application.dataPath, "Plugins", "LicenseSpring", "License", 
                    $"{Application.productName}.bin");
                
                //init extended options of License spring configs.
                LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
                {
                    HardwareID = SystemInfo.deviceUniqueIdentifier,
                    EnableLogging = false,
                    CollectHostNameAndLocalIP = true,
                    LicenseFilePath = licenseFilePath
                };

                if(Helpers.LicenseApiConfigurationHelper.IsExistDeveloperConfig())
                {
                    _licenseSpringLocalKey = Helpers.LicenseApiConfigurationHelper.ReadApiFileKey(Application.productName, true);
                }
                else if(Helpers.LicenseApiConfigurationHelper.IsExistDeployedConfig())
                {
                    _licenseSpringLocalKey = Helpers.LicenseApiConfigurationHelper.ReadApiFileKey(Application.productName, false);

                }
                else
                {
                    throw new UnityEngine.UnityException("No Api Configuration detected, Contact your asset Developer");
                }

                //HACK : if there is no baked credential read at files.
                if (_licenseSpringLocalKey != null)
                {
                    _IsDeveloperMode = _licenseSpringLocalKey.IsDevelopment;

                    var licenseConfig = new LicenseSpringConfiguration(_licenseSpringLocalKey.ApiKey,
                        _licenseSpringLocalKey.SharedKey,
                        _licenseSpringLocalKey.ProductCode,
                        _licenseSpringLocalKey.ApplicationName,
                        _licenseSpringLocalKey.ApplicationVersion,
                        licenseSpringExtendedOptions);

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
            _IsDeveloperMode = GetDeveloperStatus();

            var currentSceneCamera = Camera.main;
            bool? licenseIsInitialized = _licenseManager?.IsInitialized();

            if (!licenseIsInitialized.HasValue || !licenseIsInitialized.Value)
            {
                InitLicenseManager();
                return;
            }

            var license = (License)_licenseManager.CurrentLicense();
            if (license == null)
                RunUnlicensedProduct();

            if (license.IsActive())
                RunRegisteredProduct();

            if (license.IsTrial())
                RunTrialProduct();

            if (license.IsExpired())
                RunProductExpired();

        }

        private static void RunProductExpired()
        {
            var notify = InitNotificationSystem();
            notify.enabled = true;
            notify.SetStatus(LicenseStatus.Expired);

            OpenLicenseRegistrationForm();

            throw new UnityEngine.UnityException("Product license/trial is expired, contact author/publisher");
        }

        private static void RunTrialProduct()
        {
            var notify = InitNotificationSystem();
            notify.enabled = true;
            notify.SetStatus(LicenseStatus.Active);

            Debug.Log("Product in trial mode ");
        }

        private static void RunRegisteredProduct()
        {
            var notify = InitNotificationSystem();
            notify.enabled = true;
            notify.SetStatus(LicenseStatus.Active);
        }

        private static void RunUnlicensedProduct()
        {
            //open registration form
            OpenLicenseRegistrationForm();
            //fire up notification game object.
            var notify = InitNotificationSystem();
            notify.enabled = true;
            notify.SetStatus(LicenseStatus.Unknown);

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
