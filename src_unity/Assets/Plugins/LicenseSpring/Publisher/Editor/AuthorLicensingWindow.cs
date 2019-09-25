using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using LicenseSpring.Unity.Plugins;
using LicenseSpring.Unity.Helpers;

public class AuthorLicensingWindow : EditorWindow
{
    private TextField _txtInputKey;
    private TextField _txtInputSharedKey;
    private TextField _txtProductCode;
    private TextField _txtProductVersion;
    private TextField _txtProductName;

    private Button  _btnPublishKey, 
                    _btnDevKey, 
                    _btnTestMode, _btnResetLicense;

    private VisualElement           _headerImage;
    private LicenseSpringLocalKey   _LocalKey;

    private const string TestNonDevMode = "Test Non Developer Mode";
    private const string StopNonDevMode = "Stop Non Developer Mode";
    private const string Title = "License Spring Api Registration";

    [MenuItem("License Spring/Publisher/Author Api")]
    public static void Init()
    {
        AuthorLicensingWindow wnd = GetWindow<AuthorLicensingWindow>();
        wnd.titleContent = new GUIContent(Title);
        wnd.Show();
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = Resources.Load<VisualTreeAsset>("Layout/AuthorLicensingWindow");
        VisualElement uiTree = visualTree.CloneTree();
        root.Add(uiTree);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = Resources.Load<StyleSheet>("Styles/LicenseSpringStyles");
        root.styleSheets.Add(styleSheet);

        //querying input part
        _txtInputKey = root.Q<TextField>("inputApiKey");
        _txtInputSharedKey = root.Q<TextField>("inputApiSharedKey");
        _txtProductCode = root.Q<TextField>("inputApiProductCode");
        _txtProductName = root.Q<TextField>("inputApiProductName");
        _txtProductVersion = root.Q<TextField>("inputApiAppVersion");

        //header image
        _headerImage = root.Q<VisualElement>(className: "HeaderImage");
        _headerImage.style.unitySliceLeft = new StyleInt(StyleKeyword.Auto);
        _headerImage.style.unitySliceRight = new StyleInt(StyleKeyword.Auto);
        _headerImage.style.unitySliceTop = new StyleInt(StyleKeyword.Auto);
        _headerImage.style.unitySliceBottom = new StyleInt(StyleKeyword.Auto);

        //querying buttons
        _btnPublishKey = root.Q<Button>("btnGenerateFile");
        _btnPublishKey.clickable.clicked += OnBtnCreateFileClick;

        _btnDevKey = root.Q<Button>("btnGenerateDevFile");
        _btnDevKey.clickable.clicked += OnbtnCreateDevClick;

        _btnTestMode = root.Query<Button>("btnToggleDevMode");
        _btnTestMode.text = TestNonDevMode;
        _btnTestMode.clickable.clicked += OnToggleDevModeClick;

        //reseting license, for dev only
        _btnResetLicense = root.Query<Button>("btnResetLicense");
        _btnResetLicense.clickable.clicked += OnResetLicenseClick;

        //checking initialize status..
        var isInitialized = LicenseSpringUnityAssets.GetInitializeStatus();
        
        //checking current installed license
        var currentLicense = LicenseSpringUnityAssets.GetCurrentLicense();
        
        //detect the real mode of editor
        var isDeveloper = LicenseSpringUnityAssets.GetDeveloperStatus();

        if (isInitialized)
        {
            if (!isDeveloper)
            {
                _btnDevKey.SetEnabled(false);
                _btnPublishKey.SetEnabled(false);

                
                _btnTestMode.SetEnabled(false);
                _btnResetLicense.SetEnabled(false);
            }
            else
            {
                _btnDevKey.SetEnabled(true);
                _btnPublishKey.SetEnabled(true);
                _btnTestMode.SetEnabled(true);
                _btnResetLicense.SetEnabled(true);
            }
        }
        else
        {
            _btnDevKey.SetEnabled(true);
            _btnPublishKey.SetEnabled(true);
            _btnTestMode.SetEnabled(false);
            _btnResetLicense.SetEnabled(false);
        }

    }

    private void OnResetLicenseClick()
    {
        //got an error here, hack 
        LicenseSpringUnityAssets.ResetLicense();
    }

    private void OnToggleDevModeClick()
    {
        if (_btnTestMode.text == StopNonDevMode)
            _btnTestMode.text = TestNonDevMode;
        else
            _btnTestMode.text = StopNonDevMode;

        LicenseSpringUnityAssets.DeveloperToggleTestMode();
    }

    private void OnbtnCreateDevClick()
    {
        LicenseSpringLocalKey localKey = new LicenseSpringLocalKey {
            ApiKey = _txtInputKey.text,
            ApplicationName = _txtProductName.text,
            IsDevelopment = true,
            ApplicationVersion = _txtProductVersion.text,
            ProductCode = _txtProductCode.text,
            SharedKey = _txtInputSharedKey.text
        };

        LicenseApiConfigurationHelper.WriteApiFileKey(localKey, isDevMachine: true);
    }

    /// <summary>
    /// creating deployment key to be include on client files
    /// </summary>
    private void OnBtnCreateFileClick()
    {
        
        LicenseSpringLocalKey localKey = new LicenseSpringLocalKey
        {
            ApiKey = _txtInputKey.text,
            ApplicationName = _txtProductName.text,
            IsDevelopment = false,
            ApplicationVersion = _txtProductVersion.text,
            ProductCode = _txtProductCode.text,
            SharedKey = _txtInputSharedKey.text
        };

        LicenseApiConfigurationHelper.WriteApiFileKey(localKey, isDevMachine: false);
    }

    private void OnValidate()
    {
        Debug.Log("validated");
    }
}