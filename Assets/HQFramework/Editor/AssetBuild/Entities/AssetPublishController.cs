using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    /// <summary>
    /// 1. configure the release notes
    /// 2. generate the asset module manifest
    /// </summary>
    public sealed class AssetPublishController
    {
        private static IAssetPublishHelper publishHelper;

        public static void SetHelper(IAssetPublishHelper helper)
        {
            publishHelper = helper;
        }

        public static async Task<AssetModuleManifest> PublishAssets(AssetArchiveData archiveData, string releaseNote, string resourceVersion, int versionCode, int minimalSupportedVersionCode, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic, Action<int, int, string> uploadCallback, Action endCallback)
        {
            AssetPublishData publishData = PreprocessAssetArchive(archiveData, releaseNote, resourceVersion, versionCode, minimalSupportedVersionCode, moduleReleaseNotesDic, moduleMinimalSupportedVersionDic);
            AssetModuleManifest localManifest = publishHelper.GetBasicManifest();
            localManifest.versionCode = publishData.versionCode;
            localManifest.resourceVersion = publishData.resourceVersion;
            localManifest.minimalSupportedVersionCode = publishData.minimalSupportedVersionCode;
            localManifest.releaseNote = publishData.releaseNote;
            localManifest.isBuiltinManifest = false;
            localManifest.moduleDic = publishData.moduleDic;

            AssetModuleManifest remoteManifest = await publishHelper.GetRemoteManifestAsync();
            List<AssetBundleUploadItem> uploadList = new List<AssetBundleUploadItem>();
            foreach (AssetModuleInfo localModule in localManifest.moduleDic.Values)
            {
                if (!remoteManifest.moduleDic.ContainsKey(localModule.id))
                {
                    foreach (var bundle in localModule.bundleDic.Values)
                    {
                        AssetBundleUploadItem item = new AssetBundleUploadItem();
                        item.moduleInfo = localModule;
                        item.bundleInfo = bundle;
                        item.bundleFilePath = publishData.bundleFileMap[bundle.bundleName];
                        uploadList.Add(item);
                    }
                    continue;
                }

                AssetModuleInfo remoteModule = remoteManifest.moduleDic[localModule.id];
                foreach (AssetBundleInfo localBundle in localModule.bundleDic.Values)
                {
                    if (!remoteModule.bundleDic.ContainsKey(localBundle.bundleName) || 
                        remoteModule.bundleDic[localBundle.bundleName].md5 != localBundle.md5)
                    {
                        AssetBundleUploadItem item = new AssetBundleUploadItem();
                        item.moduleInfo = localModule;
                        item.bundleInfo = localBundle;
                        item.bundleFilePath = publishData.bundleFileMap[localBundle.bundleName];
                        uploadList.Add(item);
                    }
                }
            }

            if (uploadList.Count == 0)
            {
                return localManifest;
            }

            bool result = true;

            for (int i = 0; i < uploadList.Count; i++)
            {
                uploadCallback?.Invoke(i + 1, uploadList.Count + 1, uploadList[i].bundleInfo.bundleName);
                result = result && await publishHelper.UploadBundleAsync(uploadList[i]);
                if (!result)
                {
                    break;
                }
            }

            if (result)
            {
                uploadCallback?.Invoke(uploadList.Count + 1, uploadList.Count + 1, "AssetModuleManifest");
                result = result && await publishHelper.UploadManifestAsync(localManifest);
            }

            endCallback?.Invoke();

            return result ? localManifest : null;
        }

        private static AssetPublishData PreprocessAssetArchive(AssetArchiveData archiveData, string releaseNote, string resourceVersion, int versionCode, int minimalSupportedVersionCode, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic)
        {
            AssetPublishData result = new AssetPublishData();
            result.versionCode = versionCode;
            result.resourceVersion = resourceVersion;
            result.minimalSupportedVersionCode = minimalSupportedVersionCode;
            result.releaseNote = releaseNote;
            result.moduleDic = new Dictionary<int, AssetModuleInfo>();
            result.bundleFileMap = new Dictionary<string, string>();
            for (int i = 0; i < archiveData.moduleCompileInfoList.Count; i++)
            {
                AssetModuleCompileInfo compileInfo = archiveData.moduleCompileInfoList[i];
                AssetModuleInfo module = new AssetModuleInfo();
                module.id = compileInfo.moduleID;
                module.moduleName = compileInfo.moduleName;
                module.isBuiltin = compileInfo.isBuiltin;
                module.currentPatchVersion = compileInfo.buildVersionCode;
                module.dependencies = compileInfo.dependencies;
                module.assetsDic = compileInfo.assetsDic;
                module.bundleDic = new Dictionary<string, AssetBundleInfo>();
                for (int j = 0; j < compileInfo.bundleList.Count; j++)
                {
                    AssetBundleCompileInfo bundleCompileInfo = compileInfo.bundleList[j];
                    result.bundleFileMap.Add(bundleCompileInfo.bundleName, bundleCompileInfo.filePath);
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
                if (moduleMinimalSupportedVersionDic != null && moduleMinimalSupportedVersionDic.ContainsKey(compileInfo.moduleID))
                {
                    module.minimalSupportedPatchVersion = moduleMinimalSupportedVersionDic[compileInfo.moduleID];
                }
                if (moduleReleaseNotesDic != null && moduleReleaseNotesDic.ContainsKey(compileInfo.moduleID))
                {
                    module.releaseNote = moduleReleaseNotesDic[compileInfo.moduleID];
                }

                result.moduleDic.Add(module.id, module);
            }

            return result;
        }
    }
}
