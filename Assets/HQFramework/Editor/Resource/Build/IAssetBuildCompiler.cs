using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public interface IAssetBuildCompiler
    {
        string AssetBuildCacheDir
        {
            set;
        }

        AssetBundleManifest CompileAssets(AssetBundleBuild[] builds, AssetBuildOption buildOption);
    }
}
