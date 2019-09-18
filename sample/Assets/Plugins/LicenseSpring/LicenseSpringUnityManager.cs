﻿using LicenseSpring.Unity.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace LicenseSpring.Unity.Plugins
{
    /// <summary>
    /// This ManagerBehaviour is the heart of all operation within license spring connection with unity.
    /// Do not use it manually, it will generated by LicenseSpring.
    /// </summary>
    [DefaultExecutionOrder(-100),
        ExecuteAlways]
    public class LicenseSpringUnityManager : MonoBehaviour
    {
        public bool InitInGameUIComplete;
        public bool InitNotificationComplete;
        public bool InitLicenseManagerComplete;

        public static LicenseSpringUnityManager Instance
        {
            get
            {
                //this made sense only in game, in editor mode this instance is maintained by license watcher.
                if (INSTANCE == null)
                    INSTANCE = new GameObject("License Spring Unity Plugins")
                        .AddComponent< LicenseSpringUnityManager>();

                return INSTANCE;
            }
        }

        private static LicenseSpringUnityManager INSTANCE;

        /// <summary>
        /// all of the used license behaviour in the scenes.
        /// </summary>
        public ILicenseBehaviour[] LicenseBehaviours;

        /// <summary>
        /// Local license manager.
        /// </summary>
        public LicenseManager AppLicenseManager { get; private set; }

        //current license
        private License                     _currentLicense;
        //notification runner
        private LicenseSpringNotification   _licenseSpringNotification;
        //in game only ui
        private LicenseSpringUI             _licenseSpringInGameUi;

        private void InitNotificationSystem()
        {
            //establish notification system for license manager
            _licenseSpringNotification = GameObject.FindObjectOfType<LicenseSpringNotification>();
            if (_licenseSpringNotification == null)
                _licenseSpringNotification = Camera.main.gameObject.AddComponent<LicenseSpringNotification>();

            //set initial status
            _licenseSpringNotification.SetStatus(LicenseStatus.Unknown);
            InitNotificationComplete = true;
        }


        private void InGameNotificationSystem()
        {
            _licenseSpringInGameUi = FindObjectOfType<LicenseSpringUI>();
            if (_licenseSpringInGameUi == null)
            {
                var data = Resources.Load("LicenseSpringUIPrefabs", typeof(GameObject));
                var instance = (GameObject)Instantiate(data);
                _licenseSpringInGameUi = instance.GetComponent<LicenseSpringUI>();
            }

            //init license spring
            if (Application.isEditor)
            {

            }

            InitInGameUIComplete = true;
        }


        private void Awake()
        {

            InGameNotificationSystem();
            InitNotificationSystem();

            if (AppLicenseManager == null || AppLicenseManager.IsInitialized() == false)
                InitLicenseManager();

            SeekAllLicenseBehaviour();

            if (Application.isPlaying)
                DontDestroyOnLoad(this);

            if (INSTANCE == null)
                INSTANCE = this;

        }

        private IEnumerable Start()
        {
            var currentInstalledLicense = CheckCurrentLicense();
            Notify(currentInstalledLicense);

            while (true)
            {
                yield return new WaitForSeconds(60);
                currentInstalledLicense = CheckCurrentLicense();
                Notify(currentInstalledLicense);
            }
        }

        /// <summary>
        /// get all deployed license behaviour.
        /// </summary>
        private void SeekAllLicenseBehaviour()
        {
            LicenseBehaviours = GameObject.FindObjectsOfType<LicenseBehaviour>();
        }

        /// <summary>
        /// Routine of checking current installed license.
        /// </summary>
        /// <returns></returns>
        public License CheckCurrentLicense()
        {
            if (!AppLicenseManager.IsInitialized())
                InitLicenseManager();

            return (License)AppLicenseManager?.CurrentLicense();
        }

        /// <summary>
        /// initialize license manager, reading deployed license or register a new one, 
        /// it show trial period and expiration date
        /// </summary>
        public void InitLicenseManager()
        {
            AppLicenseManager =(LicenseManager)LicenseManager.GetInstance();

            var licenseFilePath = Path.Combine(Application.persistentDataPath, "License","license.bin");
            LicenseSpringExtendedOptions licenseSpringExtendedOptions = new LicenseSpringExtendedOptions
            {
                HardwareID = SystemInfo.deviceUniqueIdentifier,
                EnableLogging = false,
                CollectHostNameAndLocalIP = true,
                LicenseFilePath = licenseFilePath
            };

            //HACK : if there is no baked credential read at files.
            if (Helpers.LicenseApiConfigurationHelper.CheckLocalConfiguration())
            {
                var licenseLocalKey = Helpers.LicenseApiConfigurationHelper.ReadApiFileKey();

                var licenseConfig = new LicenseSpringConfiguration(licenseLocalKey.ApiKey,
                    licenseLocalKey.SharedKey,
                    licenseLocalKey.ProductCode,
                    licenseLocalKey.ApplicationName,
                    licenseLocalKey.ApplicationVersion,
                    licenseSpringExtendedOptions);

                AppLicenseManager.Initialize(licenseConfig);
            }
            else
            {
                Notify(null);
                if(Application.isEditor)
                    throw new UnityEngine.UnityException("No Api Configuration detected, Contact your asset Developer");
                else
                {
                    throw new UnityEngine.UnityException("UnAuthorized License Manager detected");
                }
            }
            
        }

        /// <summary>
        /// Notification routine for editor and gameplay
        /// </summary>
        /// <param name="licenseData"></param>
        public void Notify(License licenseData)
        {
            if(Application.isEditor)
            {
                if(Application.isPlaying)
                {
                    _licenseSpringInGameUi.SetStatus(licenseData);
                }
                else
                {
                    _licenseSpringNotification = GameObject.FindObjectOfType<LicenseSpringNotification>();
                    if (_licenseSpringNotification == null)
                    {
                        _licenseSpringNotification = Camera.main.gameObject.AddComponent<LicenseSpringNotification>();
                    }

                    if (licenseData == null)
                        _licenseSpringNotification.SetStatus(LicenseStatus.Unknown);
                    else
                        _licenseSpringNotification.SetStatus(licenseData.Status());
                }   
            }
            else
            {
                _licenseSpringInGameUi.SetStatus(licenseData);
            }
        }

    }
}