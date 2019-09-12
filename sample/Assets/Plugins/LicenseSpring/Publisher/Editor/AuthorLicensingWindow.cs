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

    private VisualElement  _headerImage;

    [MenuItem("License Spring/Publisher/Author Licensing")]
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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Publisher/Editor/AuthorLicensingWindow.uxml");
        VisualElement uiTree = visualTree.CloneTree();
        root.Add(uiTree);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Publisher/Editor/AuthorLicensingWindow.uss");

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
        Button btnCreateFile = root.Q<Button>("btnGenerateFile");
        btnCreateFile.clickable.clicked += OnBtnCreateFileClick;

        Button btnCreateDevFile = root.Q<Button>("btnGenerateDevFile");
        btnCreateDevFile.clickable.clicked += OnbtnCreateDevClick;
        
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

        LicenseFileHelper.WriteApiFileKey(localKey, isDevMachine: true);
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

        LicenseFileHelper.WriteApiFileKey(localKey, isDevMachine: false);
    }
}