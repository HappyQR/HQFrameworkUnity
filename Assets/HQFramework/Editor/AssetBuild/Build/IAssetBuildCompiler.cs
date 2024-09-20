using UnityEditor;
using UnityEngine;

namespace HQFramework.Editor
{
    public interface IAssetBuildCompiler
    {
        AssetBundleManifest CompileAssets(AssetBundleBuild[] builds, AssetBuildConfig buildConfig);
    }
}
