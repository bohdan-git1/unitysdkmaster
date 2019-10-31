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
    public class UnityBaseLicenseManager :  MonoBehaviour
    {
        private string          _licensePath, 
                                _apiPath;
        private LicenseManager  _internalLicenseManager;

        private LSLocalKey      _localKey;

        void Awake()
        {
            _licensePath = Path.Combine(Application.dataPath, Application.productName,"lock.lic");
            _apiPath = Path.Combine(Application.dataPath, Application.productName,"app.lic");

            if (!File.Exists(_apiPath))
            {
                throw new LicenseConfigurationException("Configuration invalid");
            }

            DontDestroyOnLoad(this);
        }

        void Start()
        {

        }

    }
}
