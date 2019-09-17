using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace LicenseSpring.Unity.Plugins
{
    public static class TagManager
    {
        private static UnityEngine.Object[]             tagAssets;
        private static SerializedProperty   tagsCollection;

        static TagManager()
        {
            tagAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            SerializedObject tagObjects = new SerializedObject(tagAssets[0]);
            tagsCollection = tagObjects.FindProperty("tags");

            
        }
    }
}
