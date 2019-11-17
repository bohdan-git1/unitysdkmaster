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

        private ActivationStatus    _enumActiveStatus;
        private LicenseStatus       _enumLicenseStatus;

        public string HardwareId {
            get {
                _hid = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                return _hid; 
            }
            set { _hid = value; } 
        }
        void Awake()
        {
            _licensePath = Path.Combine(Application.persistentDataPath, Application.productName, "lock.lic");
            _apiPath = Path.Combine(Application.persistentDataPath, Application.productName, "app.lic");

            _enumActiveStatus = ActivationStatus.Unknown;

            if (!File.Exists(_apiPath))
            {
                _enumActiveStatus = ActivationStatus.Unknown;
                throw new LicenseConfigurationException("Configuration invalid");
            }
            else
            {
                try
                {
                    //extended configuration options.
                    _lseo = new LicenseSpringExtendedOptions
                    {
                        HardwareID = this.HardwareId,
                        LicenseFilePath = _licensePath,
                        CollectHostNameAndLocalIP = true
                    };

                    //local config decryptor
                    _localKey = KeyStorage.ReadLocalKey();

                    //main configurations
                    LicenseSpringConfiguration lsConfig = new LicenseSpringConfiguration(_localKey.ApiKey,
                        _localKey.SharedKey,
                        _localKey.ProductCode,
                        _localKey.ApplicationName,
                        _localKey.ApplicationVersion,
                        _lseo);

                    _internalLicenseManager = (LicenseManager)LicenseManager.GetInstance();
                    _internalLicenseManager.Initialize(_lsConfig);

                    _installedLicense = (License)_internalLicenseManager.CurrentLicense();
                    SetLicenseStatus();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }

            DontDestroyOnLoad(this);
        }

        void Start()
        {
            _installedLicense = (License)_internalLicenseManager.CurrentLicense();
            SetLicenseStatus();
        }

        private void Update()
        {
            SetLicenseStatus();
        }

        private void SetLicenseStatus()
        {
            if (_installedLicense == null)
            {
                _enumLicenseStatus = LicenseStatus.Unknown;
                return;
            }

            if (_installedLicense.IsEnabled())
            {
                if (_installedLicense.IsActive())
                {
                    _enumLicenseStatus = LicenseStatus.Active;
                    _enumActiveStatus = ActivationStatus.Registered;

                    if (_installedLicense.IsTrial())
                    {
                        _enumActiveStatus = ActivationStatus.Trial;
                    }

                    if (_installedLicense.IsExpired())
                    {
                        _enumActiveStatus = ActivationStatus.Expired;
                    }

                    if (_installedLicense.IsOfflineActivated())
                    {
                        _enumActiveStatus = ActivationStatus.Offline;
                    }

                }
                else
                {
                    _enumLicenseStatus = LicenseStatus.InActive;
                }

            }
            else
            {
                _enumActiveStatus = ActivationStatus.Unknown;
                _enumLicenseStatus = LicenseStatus.Unknown;
                return;
            }
        }
    }
}
