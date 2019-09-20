using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


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
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Styles/LicenseSpringStyles.uss");
        root.styleSheets.Add(styleSheet);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Tools/Editor/LicenseSpringRegistration.uxml");
        VisualElement cloneTree = visualTree.CloneTree();
        root.Add(cloneTree);

        _btnRegister = cloneTree.Query<Button>("btnSubmit");
        _btnRequestDemo = cloneTree.Query<Button>("btnRequestTrial");

        _txtEmail = cloneTree.Query<TextField>("email");
        _txtSerial = cloneTree.Query<TextField>("cdkey");

        _lblStatusHeader = cloneTree.Query<Label>("lblStatusHeader");
        _lblStatusLicense = cloneTree.Query<Label>("lblStatusLicense");
    }

}