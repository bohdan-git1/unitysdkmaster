using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace LicenseSpring.Unity.Plugins
{
    /// <summary>
    /// License Spring Manifest Processor
    /// Editor Hook, License Spring Manifest Processor
    /// this class hook into exsiting unity editor event particularly when user modify asset from unity editor.
    /// </summary>
    public class LSManifestProcessor : UnityEditor.AssetModificationProcessor
    {
        public static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string asset, RemoveAssetOptions removeAssetOptions)
        {
            //Debug.Log("On Will Delete Hookup " + asset);

            //TODO : more explicit on item classification
            //prevent deletion of asset where path or filename contains certain word
            if (asset.Contains("LicenseSpring") || asset.Contains("ILicense") ||
                asset.Contains("License"))
                return AssetDeleteResult.FailedDelete;

            //ignore the rest..
            return AssetDeleteResult.DidDelete;
        }

    }
}