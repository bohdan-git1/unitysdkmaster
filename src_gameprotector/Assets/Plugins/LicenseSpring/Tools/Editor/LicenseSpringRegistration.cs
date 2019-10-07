using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using LicenseSpring.Unity.Plugins;

public class LicenseSpringRegistration : EditorWindow
{
    TextField   _txtEmail, 
                _txtSerial;

    Label       _lblStatusHeader, 
                _lblStatusLicense;

    Button      _btnRequestDemo, 
                _btnRegister;

    [MenuItem("License Spring/Registration")]
    public static void ShowExample()
    {
        LicenseSpringRegistration wnd = GetWindow<LicenseSpringRegistration>();
        wnd.titleContent = new GUIContent("License Spring Registration");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = Resources.Load<StyleSheet>("Styles/LicenseSpringStyles");
        root.styleSheets.Add(styleSheet);

        // Import UXML
        //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Tools/Editor/LicenseSpringRegistration.uxml");
        var visualTree = Resources.Load<VisualTreeAsset>("Layout/LicenseSpringRegistration");
        VisualElement cloneTree = visualTree.CloneTree();
        root.Add(cloneTree);

        _btnRegister = cloneTree.Query<Button>(name:"btnSubmit");
        _btnRequestDemo = cloneTree.Query<Button>(name:"btnRequestTrial");

        _txtEmail = cloneTree.Query<TextField>(name:"email");
        _txtSerial = cloneTree.Query<TextField>(name:"cdkey");

        _lblStatusHeader = cloneTree.Q<Label>(name:"lblStatusHeader");
        _lblStatusLicense = cloneTree.Q<Label>(name: "lblStatus");

        _btnRegister.clickable.clicked      += OnRegisterClick;
        _btnRequestDemo.clickable.clicked   += OnRequestDemoClick;

        var trialElement = cloneTree.Q<VisualElement>(name: "licenseStatus");
        trialElement.style.display = DisplayStyle.None;

        //checking status
        var isInitialized = LicenseSpringUnityAssets.GetInitializeStatus();
        if (isInitialized)
        {
            var currentLicense = LicenseSpringUnityAssets.GetCurrentLicense();
            if(currentLicense != null)
            {
                trialElement.style.display = DisplayStyle.Flex;
                _lblStatusHeader.text = $"License Type : {LicenseSpringUnityAssets.GetCurrentLicense().Type()}";

                if (LicenseSpringUnityAssets.GetCurrentLicense().IsTrial())
                    _lblStatusLicense.text = $"Trial Days : {LicenseSpringUnityAssets.GetCurrentLicense().DaysRemaining()}";
                else
                {

                    _lblStatusLicense.text = $"{LicenseSpringUnityAssets.GetCurrentLicense().Key()}";
                }
            }
            else
            {
                trialElement.style.display = DisplayStyle.None;
            }

        }
        else
        {
            trialElement.style.display = DisplayStyle.None;
        }

    }

    private void OnRequestDemoClick()
    {
        LicenseSpringUnityAssets.RequestDemo(_txtEmail.text);
        this.Close();
    }

    private void OnRegisterClick()
    {
        LicenseSpringUnityAssets.Register(_txtSerial.text);
        this.Close();
    }
}