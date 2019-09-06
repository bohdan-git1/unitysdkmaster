
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
    [DefaultExecutionOrder(-10), ExecuteInEditMode]
    public class LicenseSpringInfo : MonoBehaviour
    {
        ILicense            _currentLicense;
        ILicenseBehaviour   _warningScript;

        private void Awake()
        {
            _currentLicense = LicenseSpringUnityManager.Instance
                .UnityLicenseManager.CurrentLicense();

            CheckLicenseStatus();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(30);
                CheckLicenseStatus();
            }
        }

        private void CheckLicenseStatus()
        {
            _warningScript = Camera.main.gameObject.GetComponent<ILicenseBehaviour>();

            if (_currentLicense == null)
            {
                if (_warningScript == null)
                    Camera.main.gameObject.AddComponent<UnLicensed>();
                
                throw new UnityEngine.UnityException("License not specified");
            }
            else
            {
                if (!_currentLicense.IsActive())
                {

                    if (_warningScript == null)
                        Camera.main.gameObject.AddComponent<UnLicensed>();
                    throw new UnityEngine.UnityException("License Inactive");
                }

                if (!_currentLicense.IsExpired())
                {
                    if (_warningScript == null)
                        Camera.main.gameObject.AddComponent<WarningLicenseExpired>();
                    throw new UnityEngine.UnityException("License is Expired");
                }

                //more will be added
                if (!_currentLicense.IsTrial())
                {
                    var currentDate = DateTime.Now;
                    var validatyPeriod = _currentLicense.ValidityPeriod();
                }

            }
        }

       
    }

}