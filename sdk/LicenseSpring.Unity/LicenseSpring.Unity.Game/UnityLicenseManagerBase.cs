using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

using SecurityDriven;
using SecurityDriven.Inferno;
using SecurityDriven.Inferno.Hash;

namespace LicenseSpring.Unity.Game
{
    /// <summary>
    /// License Spring Unity Base License Manager.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class UnityLicenseManagerBase :  MonoBehaviour
    {
        private string          _licensePath, 
                                _apiPath, 
                                _hid;

        private LicenseManager  _internalLicenseManager;
        private License         _installedLicense;
        private LSLocalKey      _localKey;

        private LicenseSpringConfiguration      _lsConfig;
        private LicenseSpringExtendedOptions    _lseo;

        public string HardwareId {
            get {
                _hid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                return _hid; 
            }
            set { _hid = value; } 
        }

        void Awake()
        {
            _licensePath = Path.Combine(Application.dataPath, Application.productName,"lock.lic");
            _apiPath = Path.Combine(Application.dataPath, Application.productName,"app.lic");

            if (!File.Exists(_apiPath))
            {
                throw new LicenseConfigurationException("Configuration invalid");
            }

            //extended configuration options.
            _lseo = new LicenseSpringExtendedOptions
            {
                HardwareID = this.HardwareId,
                LicenseFilePath = _licensePath,
                CollectHostNameAndLocalIP = true
            };

            //local config decryptor
            //_localKey = Utilities.ReadLocalKey(_apiPath, this.HardwareId);

            //main configurations

            _internalLicenseManager = (LicenseManager)LicenseManager.GetInstance();
            _internalLicenseManager.Initialize(_lsConfig);

            DontDestroyOnLoad(this);
        }

        void Start()
        {
            
        }

    }
}
