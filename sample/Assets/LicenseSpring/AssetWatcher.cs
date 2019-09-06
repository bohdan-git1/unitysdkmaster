using UnityEngine;
using UnityEditor;
using System;

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
