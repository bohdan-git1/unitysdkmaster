using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LicenseSpring.Unity.Plugins;

[CustomEditor(typeof(LicenseSpringUnityManager))]
public class LicenseSpringUnityEditor : Editor
{
    VisualElement _rootElement;

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        _rootElement = new VisualElement();

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        _rootElement.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/LicenseSpring/Editor/LicenseSpringUnityEditor.uss");
        _rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {


        return _rootElement;
    }

}