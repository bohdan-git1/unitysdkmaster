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
        var visualTree = Resources.Load<VisualTreeAsset>("Layouts/LicenseSpringRegistration");
        VisualElement cloneTree = visualTree.CloneTree();
        root.Add(cloneTree);

        _btnRegister = cloneTree.Query<Button>("btnSubmit");
        _btnRequestDemo = cloneTree.Query<Button>("btnRequestTrial");

        _txtEmail = cloneTree.Query<TextField>("email");
        _txtSerial = cloneTree.Query<TextField>("cdkey");

        _lblStatusHeader = cloneTree.Query<Label>("lblStatusHeader");
        _lblStatusLicense = cloneTree.Query<Label>("lblStatus");

        _btnRegister.clickable.clicked      += OnRegisterClick;
        _btnRequestDemo.clickable.clicked   += OnRequestDemoClick;

        var trialElement = cloneTree.Query<Label>("trial");
        trialElement.NotVisible();

        //checking status
        var isInitialized = LicenseSpringUnityAssets.GetInitializeStatus();
        if (isInitialized)
        {
            trialElement.Visible();
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
            trialElement.NotVisible();
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