
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
    internal class LicenseSpringInfo : MonoBehaviour
    {
        private void Awake()
        {
            var uiGameObject = GameObject.FindObjectOfType<LicenseSpringUI>();
            if (uiGameObject == null)
            {
                uiGameObject = new GameObject(LicenseSpringUI.UI_NAME)
                    .AddComponent<LicenseSpringUI>();
            }

            var license = LicenseSpringUnityManager.Instance
                .UnityLicenseManager.CurrentLicense();
            if (license == null)
            {
                ExecuteEvents.Execute<ILicenseSpringMessaging>(uiGameObject.gameObject, null, (m, b)=> {
                    m.Message(UnityLicenseMessageType.LicenseInvalid, "License not issued");
                });
            }
            else
            {
                if (!license.IsActive())
                {
                    ExecuteEvents.Execute<ILicenseSpringMessaging>(uiGameObject.gameObject, null, (m, b) => {
                        m.Message(UnityLicenseMessageType.LicenseInActive, "License inactive");
                    });
                }

                if (!license.IsExpired())
                {
                    ExecuteEvents.Execute<ILicenseSpringMessaging>(uiGameObject.gameObject, null, (m, b) => {
                        m.Message(UnityLicenseMessageType.LicenseExpired, "License is Expired");
                        m.SetSender(this.gameObject);
                    });
                }

                //more will be added
                if (!license.IsTrial())
                {
                    var currentDate = DateTime.Now;
                    var validatyPeriod = license.ValidityPeriod();
                }

            }



        }

        public void RegisterTrial(string email = null)
        {
            var key = LicenseSpringUnityManager.Instance.UnityLicenseManager.GetTrialKey(email);
            LicenseSpringUnityManager.Instance.UnityLicenseManager.ActivateLicense(key);
        }

        public void Register(string key)
        {
            LicenseSpringUnityManager.Instance.UnityLicenseManager.ActivateLicense(key);
        }

    }

}