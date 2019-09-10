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

    [MenuItem("License Spring/Asset Author Licensing")]
    public static void Init()
    {
        AuthorLicensingWindow wnd = GetWindow<AuthorLicensingWindow>();
        wnd.titleContent = new GUIContent("Assets Author Licensing and Deployment");
        wnd.Show();
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Editor/AuthorLicensingWindow.uxml");
        VisualElement uiTree = visualTree.CloneTree();
        root.Add(uiTree);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/AuthorLicensingWindow.uss");

        root.styleSheets.Add(styleSheet);

        //querying input part
        _txtInputKey = root.Q<TextField>("inputApiKey");
        _txtInputSharedKey = root.Q<TextField>("inputApiSharedKey");
        _txtProductCode = root.Q<TextField>("inputApiProductCode");
        _txtProductName = root.Q<TextField>("inputApiProductName");
        _txtProductVersion = root.Q<TextField>("inputApiAppVersion");

        //querying buttons
        Button btnCreateFile = root.Q<Button>("btnGenerateFile");
        btnCreateFile.clickable.clicked += OnBtnCreateFileClick;

        Button btnCreateDevFile = root.Q<Button>("btnGenerateDevFile");
        btnCreateDevFile.clickable.clicked += OnbtnCreateDevClick;
        
    }

    private void OnbtnCreateDevClick()
    {
        LocalKey localKey = new LocalKey {
            ApiKey = _txtInputKey.text,
            ApplicationName = _txtProductName.text,
            IsDevelopment = true,
            ApplicationVersion = _txtProductVersion.text,
            ProductCode = _txtProductCode.text,
            SharedKey = _txtInputSharedKey.text
        };

        LicenseFileHelper.WriteApiFileKey(localKey);
    }

    private void OnBtnCreateFileClick()
    {
        LocalKey localKey = new LocalKey
        {
            ApiKey = _txtInputKey.text,
            ApplicationName = _txtProductName.text,
            IsDevelopment = false,
            ApplicationVersion = _txtProductVersion.text,
            ProductCode = _txtProductCode.text,
            SharedKey = _txtInputSharedKey.text
        };

        LicenseFileHelper.WriteApiFileKey(localKey);
    }
}