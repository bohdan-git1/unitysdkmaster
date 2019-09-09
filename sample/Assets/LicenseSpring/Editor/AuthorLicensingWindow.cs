using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LicenseSpring/" +
            "Editor/AuthorLicensingWindow.uxml");
        VisualElement uiTree = visualTree.CloneTree();
        root.Add(uiTree);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/LicenseSpring/" +
            "Editor/AuthorLicensingWindow.uss");

        root.styleSheets.Add(styleSheet);

        //querying input part
        _txtInputKey = root.Q<TextField>("");

        //querying buttons
        Button btnCreateFile = root.Q<Button>("btnGenerateFile");
        btnCreateFile.clickable.clicked += OnBtnCreateFileClick;
        
    }

    private void OnBtnCreateFileClick()
    {
        
    }
}