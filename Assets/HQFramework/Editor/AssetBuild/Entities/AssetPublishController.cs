using System.Collections.Generic;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    /// <summary>
    /// 1. configure the release notes(general, each module)
    /// 2. generate the asset module manifest(built-in manifest and remote manifest)
    /// 3. configure the built-in module and copy the built-in assets to package
    /// </summary>
    public sealed class AssetPublishController
    {
        private static IAssetPublishHelper publishHelper;

        public static void SetHelper(IAssetPublishHelper helper)
        {
            publishHelper = helper;
        }

        public static AssetPublishData PreprocessAssetArchive(AssetArchiveData archiveData, string releaseNote, int resourceVersion, int minimalSupportedPatchVersion, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic, string tag)
        {
            AssetPublishData result = new AssetPublishData();
            result.tag = tag;
            result.resourceVersion = resourceVersion;
            result.minimalSupportedVersion = minimalSupportedPatchVersion;
            result.releaseNote = releaseNote;
            result.moduleDic = new Dictionary<int, AssetModuleInfo>();
            for (int i = 0; i < archiveData.moduleCompileInfoList.Count; i++)
            {
                AssetModuleCompileInfo compileInfo = archiveData.moduleCompileInfoList[i];
                AssetModuleInfo module = new AssetModuleInfo();
                module.id = compileInfo.moduleID;
                module.moduleName = compileInfo.moduleName;
                module.isBuiltin = compileInfo.isBuiltin;
                module.currentPatchVersion = compileInfo.buildVersionCode;
                module.minimalSupportedPatchVersion = moduleMinimalSupportedVersionDic[compileInfo.moduleID];
                module.releaseNote = moduleReleaseNotesDic[compileInfo.moduleID];
                module.dependencies = compileInfo.dependencies;
                module.assetsDic = compileInfo.assetsDic;
                module.bundleDic = new Dictionary<string, AssetBundleInfo>();
                for (int j = 0; j < compileInfo.bundleList.Count; j++)
                {
                    AssetBundleCompileInfo bundleCompileInfo = compileInfo.bundleList[j];
                    AssetBundleInfo bundleInfo = new AssetBundleInfo();
                    bundleInfo.moduleID = compileInfo.moduleID;
                    bundleInfo.moduleName = compileInfo.moduleName;
                    bundleInfo.bundleName = bundleCompileInfo.bundleName;
                    bundleInfo.md5 = bundleCompileInfo.md5;
                    bundleInfo.size = bundleCompileInfo.size;
                    bundleInfo.dependencies = bundleCompileInfo.dependencies;
                    bundleInfo.bundleUrlRelatedToModule = publishHelper.GetBundleRelatedUrl(bundleInfo, module);
                    module.bundleDic.Add(bundleInfo.bundleName, bundleInfo);
                }
                module.moduleUrlRoot = publishHelper.GetModuleUrlRoot(module);
                result.moduleDic.Add(module.id, module);
            }

            return result;
        }

        public static bool PublishAssets(AssetPublishData publishData)
        {
            return false;
        }
    }
}
