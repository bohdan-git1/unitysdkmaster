
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LicenseSpring.Unity.Helpers;


namespace LicenseSpring.Unity.Plugins
{

    /// <summary>
    /// LicenseSpringWatcher, intended to be only as editor script and only used as editor script.
    /// change note : made this utils to only responsible to maintenance license spring unity manager game object, 
    /// all license initialization moved to LicenseSpringUnityManager
    /// </summary>
    [InitializeOnLoad]
    public class LicenseSpringWatcher
    {
        private static LicenseSpringUnityManager    _licenseSpringUnityManager;

        public const string WATCH_NAME = "License Manager Runner";

        static LicenseSpringWatcher()
        {
            EditorApplication.update += OnEditorUpdateCycle;
            EditorApplication.projectChanged += OnProjectCompositionChanged;
            EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;

            Debug.Log("Watcher log init");

            RunLicenseWatchdog();
        }


        #region EditorEvents

        private static void OnProjectCompositionChanged()
        {
            RunLicenseWatchdog();
        }

        private static void OnEditorHierarchyChanged()
        {
            RunLicenseWatchdog();
        }


        private static void OnEditorUpdateCycle()
        {
            //RunLicenseWatchdog();
        }

        #endregion

        #region Internal Methods


        /// <summary>
        /// Seek or Creating license watcher game object when not found
        /// </summary>
        private static void RunLicenseWatchdog()
        {
            //Debug.Log("Running License Watchdog");

            _licenseSpringUnityManager = GameObject.FindObjectOfType<LicenseSpringUnityManager>();
            if (_licenseSpringUnityManager == null)
                _licenseSpringUnityManager = new GameObject(WATCH_NAME).AddComponent<LicenseSpringUnityManager>();

        }
        
        #endregion
    }
}
