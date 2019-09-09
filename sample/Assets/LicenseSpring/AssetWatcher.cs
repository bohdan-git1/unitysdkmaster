using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace LicenseSpring.Unity
{

    /// <summary>
    /// Watcher Routine to keep the component in hierarchy and restore them when deleted
    /// This routine is the one that create, manage and keep LicenseSpringUnityManager
    /// in Editor.
    /// </summary>
    [InitializeOnLoad]
    public class AssetWatcher
    {
        static LicenseSpringUnityManager _licenseSpringUnityManager;

        static FileStream   _fs;
        static string       _existingLocalKeyFile;

        static Rect area = new Rect(20, 20, 250, 60);
        static Color areaColor;
        static Color textColor;
        static Color btnColor;

        static AssetWatcher()
        {
            //events, editor internal Update loop.
            EditorApplication.update += OnEditorUpdate;
            //Events, when game object hierarchy change.
            EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;

            //creating license game object and license checker
            InitLicense();
        }

        private static void CheckLocalFile()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "lic");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //check if there is existance of a shared key
            var keys = Directory.GetFiles(folderPath, "*.skey");
            if (keys?.Length > 0)
            {
                _existingLocalKeyFile = keys[0];
            }
            else
            {

            }
        }

        #region Editor Events

        private static void OnEditorHierarchyChanged()
        {
            //reinit license
            InitLicense();
        }

        private static void OnEditorUpdate()
        {

        }
        
        #endregion

        /// <summary>
        /// create place holder for license detection and warning
        /// </summary>
        private static void InitLicense()
        {
            _licenseSpringUnityManager = GameObject.FindObjectOfType<LicenseSpringUnityManager>();
            if(_licenseSpringUnityManager == null)
            {
                _licenseSpringUnityManager = new GameObject(GUID.Generate().ToString())
                    .AddComponent<LicenseSpringUnityManager>();
            }
        }
    }
}
