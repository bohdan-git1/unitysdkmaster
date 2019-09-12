using UnityEditor;
using UnityEngine.UIElements;
using LicenseSpring.Unity.Plugins;

namespace LicenseSpring.Unity.Tools
{

    //[CustomEditor(typeof(LicenseSpringUnityManager))]
    public class AssetLicenseEditor : Editor
    {
        LicenseSpringUnityManager _assetLicenseManager;
        License _currentLicense;

        VisualElement _root;
        VisualTreeAsset _visualTreeAsset;

        TextField _txtKey,
                    _txtEmail;

        public void OnEnable()
        {
            // Each editor window contains a root VisualElement object
            _root = new VisualElement();

            // Import UXML
            _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Editor/AssetLicenseEditor.uxml");

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/AssetLicenseEditor.uss");

            _root.styleSheets.Add(styleSheet);
            _assetLicenseManager = this.target as LicenseSpringUnityManager;
        }

        public override VisualElement CreateInspectorGUI()
        {
            //i dont know yet why he had to copy this..its reference type..for now i will follow him.
            var root = _root;
            root.Clear();

            _visualTreeAsset.CloneTree(root);

            var btn = root.Q<Button>(name: "btnSubmit");
            btn.clickable.clicked += OnSubmitClick;

            _txtEmail = root.Q<TextField>(name: "txtEmail");
            _txtKey = root.Q<TextField>(name: "txtKey");

            return _root;
        }

        private void OnSubmitClick()
        {
            var key = _txtKey.value;
            var email = _txtEmail.text;

            if (!string.IsNullOrEmpty(key))
            {
                //var license = _assetLicenseManager..ActivateLicense(key);
            }
            else
            {
                //var trialKey = _assetLicenseManager.UnityLicenseManager.GetTrialKey(email);
                //var license = _assetLicenseManager.UnityLicenseManager.ActivateLicense(trialKey);
            }
        }
    } 
}