using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LicenseSpring.Unity.Plugins;
using System;

[CustomEditor(typeof(LicenseSpringUnityManager))]
public class LicenseSpringUnityEditor : Editor
{
    VisualElement   _rootElement;
    VisualTreeAsset _visualTreeAsset;

    //control and input control
    TextField   _txtEmail;
    TextField   _txtKeyCode;
    Button      _btnSubmit;

    LicenseSpringUnityManager _target;

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        _rootElement = new VisualElement();

        // Import UXML
        _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uxml");

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uss");
        //_rootElement.styleSheets.Add(styleSheet);
        _target = (LicenseSpringUnityManager)target;

    }

    public override VisualElement CreateInspectorGUI()
    {
        var root =_rootElement;
        //clearing any previous, if any, tree
        root.Clear();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uss");
        root.styleSheets.Add(styleSheet);
        //clone loaded visual tree to existing root element
        _visualTreeAsset.CloneTree(root);
        //querying control
        _btnSubmit = root.Query<Button>("btnSubmit");
        _txtEmail = root.Query<TextField>("txtEmail");
        _txtKeyCode = root.Query<TextField>("txtKey");

        //events registration
        _btnSubmit.clickable.clicked += OnSubmitClick;

        return root;
    }

    private void OnSubmitClick()
    {
        if (string.IsNullOrEmpty(_txtKeyCode.text) && !string.IsNullOrEmpty(_txtEmail.text))
        {
            //TODO validate email address
            _target.AppLicenseManager.GetTrialKey(_txtEmail.text);
        }
        else {
            _target?.AppLicenseManager.ActivateLicense(_txtKeyCode?.text);
        }
    }
}