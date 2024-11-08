using System;
using System.Collections.Generic;
using System.IO;
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

        public static HQAssetManifest PackBuiltinAssets(List<AssetModuleCompileInfo> moduleCompileInfoList)
        {
            if (moduleCompileInfoList == null || moduleCompileInfoList.Count == 0)
                return null;
            if (Directory.Exists(publishHelper.AssetsBuiltinDir))
            {
                Directory.Delete(publishHelper.AssetsBuiltinDir, true);
            }
            Directory.CreateDirectory(publishHelper.AssetsBuiltinDir);
            HQAssetManifest manifest = publishHelper.GetBasicManifest();
            manifest.isBuiltinManifest = true;
            manifest.resourceVersion = "0.0.0";
            manifest.moduleDic = new Dictionary<int, HQAssetModuleConfig>();
            for (int i = 0; i < moduleCompileInfoList.Count; i++)
            {
                AssetModuleCompileInfo compileInfo = moduleCompileInfoList[i];
                HQAssetModuleConfig module = new HQAssetModuleConfig();
                module.id = compileInfo.moduleID;
                module.moduleName = compileInfo.moduleName;
                module.isBuiltin = compileInfo.isBuiltin;
                module.currentPatchVersion = compileInfo.buildVersionCode;
                module.dependencies = compileInfo.dependencies;
                module.assetsDic = compileInfo.assetsDic;
                module.bundleDic = new Dictionary<string, HQAssetBundleConfig>();
                for (int j = 0; j < compileInfo.bundleList.Count; j++)
                {
                    AssetBundleCompileInfo bundleCompileInfo = compileInfo.bundleList[j];
                    HQAssetBundleConfig bundleInfo = new HQAssetBundleConfig();
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
                manifest.moduleDic.Add(module.id, module);

                publishHelper.PackBuiltinModule(compileInfo);
            }

            return manifest;
        }

        public static async Task<HQAssetManifest> PublishAssets(AssetArchiveData archiveData, string releaseNote, string resourceVersion, int versionCode, int minimalSupportedVersionCode, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic, Action<int, int, string> uploadCallback, Action endCallback)
        {
            AssetPublishData publishData = PreprocessAssetArchive(archiveData, releaseNote, resourceVersion, versionCode, minimalSupportedVersionCode, moduleReleaseNotesDic, moduleMinimalSupportedVersionDic);
            HQAssetManifest localManifest = publishHelper.GetBasicManifest();
            localManifest.versionCode = publishData.versionCode;
            localManifest.resourceVersion = publishData.resourceVersion;
            localManifest.minimalSupportedVersionCode = publishData.minimalSupportedVersionCode;
            localManifest.releaseNote = publishData.releaseNote;
            localManifest.isBuiltinManifest = false;
            localManifest.moduleDic = publishData.moduleDic;

            HQAssetManifest remoteManifest = await publishHelper.GetRemoteManifestAsync();
            List<AssetBundleUploadItem> uploadList = new List<AssetBundleUploadItem>();
            foreach (HQAssetModuleConfig localModule in localManifest.moduleDic.Values)
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

                HQAssetModuleConfig remoteModule = remoteManifest.moduleDic[localModule.id];
                foreach (HQAssetBundleConfig localBundle in localModule.bundleDic.Values)
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
                uploadCallback?.Invoke(uploadList.Count + 1, uploadList.Count + 1, "Manifest");
                result = result && await publishHelper.UploadManifestAsync(localManifest);
            }

            endCallback?.Invoke();
            publishHelper.AssetUploader.Dispose();

            return result ? localManifest : null;
        }

        private static AssetPublishData PreprocessAssetArchive(AssetArchiveData archiveData, string releaseNote, string resourceVersion, int versionCode, int minimalSupportedVersionCode, Dictionary<int, string> moduleReleaseNotesDic, Dictionary<int, int> moduleMinimalSupportedVersionDic)
        {
            AssetPublishData result = new AssetPublishData();
            result.versionCode = versionCode;
            result.resourceVersion = resourceVersion;
            result.minimalSupportedVersionCode = minimalSupportedVersionCode;
            result.releaseNote = releaseNote;
            result.moduleDic = new Dictionary<int, HQAssetModuleConfig>();
            result.bundleFileMap = new Dictionary<string, string>();
            for (int i = 0; i < archiveData.moduleCompileInfoList.Count; i++)
            {
                AssetModuleCompileInfo compileInfo = archiveData.moduleCompileInfoList[i];
                HQAssetModuleConfig module = new HQAssetModuleConfig();
                module.id = compileInfo.moduleID;
                module.moduleName = compileInfo.moduleName;
                module.isBuiltin = compileInfo.isBuiltin;
                module.currentPatchVersion = compileInfo.buildVersionCode;
                module.dependencies = compileInfo.dependencies;
                module.assetsDic = compileInfo.assetsDic;
                module.bundleDic = new Dictionary<string, HQAssetBundleConfig>();
                for (int j = 0; j < compileInfo.bundleList.Count; j++)
                {
                    AssetBundleCompileInfo bundleCompileInfo = compileInfo.bundleList[j];
                    result.bundleFileMap.Add(bundleCompileInfo.bundleName, bundleCompileInfo.filePath);
                    HQAssetBundleConfig bundleInfo = new HQAssetBundleConfig();
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
