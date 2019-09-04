
using LicenseSpring.Unity;
using LicenseSpring.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LicenseSpring.Unity
{
    /// <summary>
    /// Run time creatable object by asset license manager, it run first afatre assetlicensemanager,here we can check state of license
    /// usable to asset creator and game developer alike.
    /// Do not modify execution order
    /// </summary>
    [DefaultExecutionOrder(-10)]
    internal class LicenseInfo : MonoBehaviour
    {
        private GameObject _trialWarning;

        private void Awake()
        {

            var license = AssetLicenseManager.Instance.CurrentLicense;
            if (license == null)
            {
                EventSystem.current.BroadcastMessage("LicenseReceiver", "License is not issued", SendMessageOptions.DontRequireReceiver);
                throw new UnityEngine.UnityException("License is not issued");
            }


            if (!license.IsActive())
                throw new UnityEngine.UnityException("License is not active");

            if (!license.IsExpired())
                throw new UnityEngine.UnityException("License is expired");

            //more will be added
            if (!license.IsTrial())
            {
                var currentDate = DateTime.Now;
                var validatyPeriod = license.ValidityPeriod();
            }
        }


    }

}