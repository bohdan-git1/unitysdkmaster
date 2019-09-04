using LicenseSpring;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//tentative names, this will be move out to seperate dll and repacked and probably generated 
//and installed to client machines
public class AssetLicenseManager : MonoBehaviour
{
    private static AssetLicenseManager _INSTANCE;

    public static AssetLicenseManager Instance
    {
        get
        {
            return _INSTANCE;
        }
    }

    public bool IsInitialized { get; private set; }
    public License CurrentLicense { get; set; }
    public LicenseManager LicenseManager { get; private set; }

    public AssetLicenseManager()
    {
        //all client specific api will be 'burn-in' into this api behaviour and made into a dll
        var api = "afce72fb-9fba-406e-8d19-ffde5b0a7cad";
        var skey = "Qc8EdU7DY-gMI87-JMueZWXdtJ0Ek_hS6dGC_SwusO8";
        var productCode = "udu";
        var appName = "Unity Asset Store Item Licensor";
        var appVersion = "v.1.0";

        //TODO :license path, this still producing errors, had to run as administrator
        var licensePath = Path.Combine(Directory.GetCurrentDirectory(), "configs");
        if (!Directory.Exists(licensePath))
            Directory.CreateDirectory(licensePath);

        LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
        {
            HardwareID = System.Guid.NewGuid().ToString(),
            EnableLogging = true,
            CollectHostNameAndLocalIP = true,
            LicenseFilePath = licensePath
        };

        //configuring
        var licenseConfig = new LicenseSpringConfiguration(api, skey,
            productCode,
            appName,
            appVersion,
            extendedOptions: licenseSpringExtendedOptions);

        this.LicenseManager = (LicenseManager)LicenseManager.GetInstance();
        this.LicenseManager.Initialize(licenseConfig);

        IsInitialized = LicenseManager.IsInitialized();
    }

    private void Awake()
    {
        //this where we hook to unity engine internal script initialization
        if (_INSTANCE ==null || _INSTANCE != this)
            _INSTANCE = this;

        //this is odd and hacked...we had to call it from unity own awake
        //so we dont run into an error
        CurrentLicense = (License)LicenseManager.CurrentLicense();

    }
}
