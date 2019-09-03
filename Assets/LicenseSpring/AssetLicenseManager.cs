using LicenseSpring;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//tentative names
public class AssetLicenseManager : MonoBehaviour
{
    public bool IsInitialized { get; private set; }
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
        
    }
}
