using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AuthorLogin : EditorWindow
{
    VisualElement   _rootElement;
    VisualTreeAsset _elementTree;
    StyleSheet      _elementStyleSheet;


    [MenuItem("License Spring/Publisher/Login")]
    public static void ShowExample()
    {
        AuthorLogin wnd = GetWindow<AuthorLogin>();
        wnd.titleContent = new GUIContent("AuthorLogin");
        wnd.maxSize = new Vector2(350, 450);
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        _rootElement = rootVisualElement;

        // Import UXML
        _elementTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Publisher/Editor/AuthorLogin.uxml");
        var uiTree = _elementTree.CloneTree();
        _rootElement.Add(uiTree);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        _elementStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Publisher/Editor/AuthorLogin.uss");
        _rootElement.styleSheets.Add(_elementStyleSheet);



    }


}