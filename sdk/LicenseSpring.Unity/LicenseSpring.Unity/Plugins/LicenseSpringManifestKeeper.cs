using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;

namespace LicenseSpring.Unity.Plugins
{
    public class LicenseSpringManifestKeeper : UnityEditor.AssetModificationProcessor
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
