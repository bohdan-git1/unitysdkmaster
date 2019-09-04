using System.IO;

using UnityEngine;
using UnityEngine.Windows;

using LicenseSpring;

namespace LicenseSpring.Unity
{

    //tentative names, this will be move out to seperate dll and repacked and probably generated 
    //and installed to client machines
    [DefaultExecutionOrder(-100), ExecuteAlways]
    public class LicenseSpringUnityManager : MonoBehaviour
    {
        const string LICENSE_CHECKER_NAME = "LICENSE-CHECKER";

        private static 
            LicenseSpringUnityManager    _INSTANCE;

        private GameObject              _licenseChecker;

        private string          _api,
                                _skey,
                                _prodCode,
                                _appName,
                                _appVersion;

        private LicenseSpringConfiguration _licenseConfig;
        private LicenseManager             _licenseManager;


        public static LicenseSpringUnityManager Instance
        {
            get
            {
                if (_INSTANCE == null)
                    _INSTANCE = FindObjectOfType<LicenseSpringUnityManager>();

                return _INSTANCE;
            }
        }

        public bool IsInitialized { get; private set; }

        public LicenseManager UnityLicenseManager {
            get {
                if (_licenseManager == null)
                    InitLicenseManager();

                if (!_licenseManager.IsInitialized())
                    InitLicenseManager();

                return _licenseManager;
            }
        }

        public LicenseSpringUnityManager()
        {
            //all client specific api will be 'burn-in' into this api behaviour and made into a dll
            _api = "afce72fb-9fba-406e-8d19-ffde5b0a7cad";
            _skey = "Qc8EdU7DY-gMI87-JMueZWXdtJ0Ek_hS6dGC_SwusO8";
            _prodCode = "udu";
            _appName = "Unity Asset Store Item Licensor";
            _appVersion = "v.1.0";

            //configuring
            _licenseManager = (LicenseManager)LicenseManager.GetInstance();
        }

        private void Awake()
        {
            InitLicenseManager();

            _licenseChecker = GameObject.Find(LICENSE_CHECKER_NAME);
            if (_licenseChecker == null)
            {
                _licenseChecker = new GameObject(LICENSE_CHECKER_NAME);
                _licenseChecker.AddComponent<LicenseSpringInfo>();
            }

        }

        private void InitLicenseManager()
        {
            //TODO :license path, this still producing errors, had to run as administrator which is very unlikely to happen
            LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
            {
                HardwareID = System.Guid.NewGuid().ToString(),
                EnableLogging = true,
                CollectHostNameAndLocalIP = true,
                LicenseFilePath = Application.dataPath
            };

            _licenseConfig = new LicenseSpringConfiguration(_api, _skey,
                _prodCode,
                _appName,
                _appVersion,
                extendedOptions: licenseSpringExtendedOptions);

            //initializing manually
            _licenseManager.Initialize(_licenseConfig);

            IsInitialized = UnityLicenseManager.IsInitialized();
        }

        private void OnPreRender()
        {
            
        }
    } 
}
