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
    Label       _trialLabel, _trialPeriod;

    Button      _btnSubmit;
    Button      _btnRequestDemo;
    Button      _btnReset;

    LicenseSpringUnityManager _target;

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        _rootElement = new VisualElement();

        // Import UXML
        _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Tools/Editor/LicenseSpringUnityEditor.uxml");

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uss");
        //_rootElement.styleSheets.Add(styleSheet);
        _target = (LicenseSpringUnityManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_txtEmail.HasMouseCapture())
            Debug.Log("Mouse clicked");
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root =_rootElement;
        //clearing any previous, if any, tree
        root.Clear();
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Tools/Editor/LicenseSpringUnityEditor.uss");
        root.styleSheets.Add(styleSheet);
        //clone loaded visual tree to existing root element
        _visualTreeAsset.CloneTree(root);
        //querying control
        _btnSubmit = root.Query<Button>("btnSubmit");
        _btnRequestDemo = root.Query<Button>("btnSubmitEmail");
        _btnReset = root.Query<Button>("btnReset");
        
        _txtEmail = root.Query<TextField>("txtEmail");
        _txtKeyCode = root.Query<TextField>("txtKey");
        _trialLabel = root.Query<Label>("trialLabel");
        _trialPeriod = root.Query<Label>("trialPeriod");

        _trialPeriod.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        _trialLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

        //events registration
        _btnSubmit.clickable.clicked += OnSubmitClick;
        _btnRequestDemo.clickable.clicked += OnBtnRequestDemo;
        _btnReset.clickable.clicked += OnBtnReset;

        //textfield events;

        var license = _target.CheckCurrentLicense();
        if(license == null)
        {
            _trialLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _trialLabel.text = "License file not found, contact author/publisher";
        }

        if (license != null && license.IsTrial())
        {

            var remainingDays = license.DaysRemaining();
            _trialLabel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _trialPeriod.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            _trialLabel.text = "Trial Remaining Days :";
            _trialPeriod.text = remainingDays.ToString();
        }

        return root;
    }

    private void OnBtnReset()
    {
        _target.AppLicenseManager.ClearLocalStorage();
    }

    private void OnBtnRequestDemo()
    {
        var email = _txtEmail.text;
        var trialKey = _target.AppLicenseManager.GetTrialKey(email);
        var licenseKey = _target.AppLicenseManager.ActivateLicense(trialKey);
        
    }

    private void OnSubmitClick()
    {
        var key = _txtKeyCode.text;
        _target.AppLicenseManager.ActivateLicense(key);
    }
}