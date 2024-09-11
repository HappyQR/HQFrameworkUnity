using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.IO;
using System;
using System.Net;
using System.Net.Http;
using HQFramework.Resource;

namespace HQFramework.Editor
{
    public class AssetsUploader
    {
        private static OssClient client;

        private static string manifestUrl = "HQFramework/Assets/AssetModuleManifest.json";
        private static string bucketName = "happyq-test";

        public static void Init()
        {
            if (client == null)
            {
                string[] key_id = File.ReadAllText(Path.Combine(Application.dataPath, "../Build/Aliyun.txt")).Split('|');
                string accessId = key_id[0];
                string accessKey = key_id[1];
                string endpoint = "https://oss-cn-beijing.aliyuncs.com";
                string region = "cn-beijing";
                ClientConfiguration conf = new ClientConfiguration();
                conf.SignatureVersion = SignatureVersion.V4;
                conf.ConnectionTimeout = 3000;
                conf.EnalbeMD5Check = true;
                client = new OssClient(endpoint, accessId, accessKey, conf);
                client.SetRegion(region);
            }
        }


        public static async void SyncAssetsWithServer()
        {
            Init();

            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            AssetRuntimeConfig assetConfig = AssetRuntimeConfigManager.GetDefaultConfig();
            string localManifestPath = Path.Combine(Application.dataPath, buildOption.bundleOutputDir, "AssetModuleManifest.json");
            AssetModuleManifest localManifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(File.ReadAllText(localManifestPath));

            using HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            using HttpResponseMessage manifestResponseMsg = await httpClient.GetAsync(assetConfig.hotfixManifestUrl);
            if (manifestResponseMsg.IsSuccessStatusCode)
            {
                string msgJson = await manifestResponseMsg.Content.ReadAsStringAsync();
                AssetModuleManifest remoteManifest = JsonUtilityEditor.ToObject<AssetModuleManifest>(msgJson);

                if (localManifest == null)
                {
                    Debug.LogError("local assets manifest doesn't exist.");
                }
                else
                {
                    try
                    {
                        SyncAssets(remoteManifest, localManifest);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        EditorUtility.ClearProgressBar();
                    }
                }
            }
            else if (manifestResponseMsg.StatusCode == HttpStatusCode.NotFound)
            {
                //Debug.Log("server manifest doesn't exist, first time to upload.");
                try
                {
                    UploadAllModule(localManifest);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                Debug.Log(manifestResponseMsg.StatusCode);
            }
        }

        public static void UploadAllModule(AssetModuleManifest localManifest)
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            int uploadedModuleCount = 0;
            foreach (var moduleInfo in localManifest.moduleDic.Values)
            {
                string keyRoot = $"HQFramework/Assets/{buildOption.resourceVersion}/{moduleInfo.moduleName}/";
                int uploadedBundleCount = 0;
                foreach (var item in moduleInfo.bundleDic)
                {
                    string localFile = Path.Combine(Application.dataPath,
                                                    buildOption.bundleOutputDir,
                                                    buildOption.resourceVersion.ToString(),
                                                    moduleInfo.moduleName,
                                                    moduleInfo.currentPatchVersion.ToString(),
                                                    item.Key);
                    string key = $"{keyRoot}{moduleInfo.currentPatchVersion}/{item.Key}";

                    EditorUtility.DisplayProgressBar($"{uploadedModuleCount}/{localManifest.moduleDic.Count} Modules Uploaded, Current: {moduleInfo.moduleName}",
                                                     $"{uploadedBundleCount}/{moduleInfo.bundleDic.Count} Bundles Uploaded, Current: {item.Key}",
                                                     (float)uploadedBundleCount / (float)moduleInfo.bundleDic.Count);

                    using PutObjectResult result = client.PutObject(bucketName, key, localFile);
                    if (result.HttpStatusCode != HttpStatusCode.OK)
                    {
                        EditorUtility.ClearProgressBar();
                        Debug.LogError(result.HttpStatusCode);
                        return;
                    }
                    uploadedBundleCount++;
                }
                uploadedModuleCount++;
            }

            using PutObjectResult manifestResult = client.PutObject(bucketName, manifestUrl, Path.Combine(Application.dataPath, buildOption.bundleOutputDir, "AssetModuleManifest.json"));
            if (manifestResult.HttpStatusCode != HttpStatusCode.OK)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError(manifestResult.HttpStatusCode);
                return;
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("Sync with server successfully.");
        }

        public static void SyncAssets(AssetModuleManifest remoteManifest, AssetModuleManifest localManifest)
        {
            AssetBuildOption buildOption = AssetBuildOptionManager.GetDefaultConfig();
            if (remoteManifest.resourceVersion != localManifest.resourceVersion)
            {
                UploadAllModule(localManifest);
                return;
            }

            List<AssetModuleInfo> uploadModuleList = new List<AssetModuleInfo>();

            // handle obsolete modules according to business logic
            // List<AssetModuleInfo> obsoleteModuleList = new List<AssetModuleInfo>();

            foreach (var item in localManifest.moduleDic)
            {
                if (remoteManifest.moduleDic.ContainsKey(item.Key) &&
                    remoteManifest.moduleDic[item.Key].currentPatchVersion == item.Value.currentPatchVersion)
                {
                    continue;
                }

                uploadModuleList.Add(item.Value);
            }

            if (uploadModuleList.Count == 0)
            {
                Debug.Log("Check Clean, Done.");
                return;
            }

            int uploadedModuleCount = 0;

            for (int i = 0; i < uploadModuleList.Count; i++)
            {
                string keyRoot = $"HQFramework/Assets/{buildOption.resourceVersion}/{uploadModuleList[i].moduleName}/";
                int uploadedBundleCount = 0;
                foreach (var item in uploadModuleList[i].bundleDic)
                {
                    string localFile = Path.Combine(Application.dataPath,
                                                    buildOption.bundleOutputDir,
                                                    buildOption.resourceVersion.ToString(),
                                                    uploadModuleList[i].moduleName,
                                                    uploadModuleList[i].currentPatchVersion.ToString(),
                                                    item.Key);
                    string key = $"{keyRoot}{uploadModuleList[i].currentPatchVersion}/{item.Key}";

                    EditorUtility.DisplayProgressBar($"{uploadedModuleCount}/{uploadModuleList.Count} Modules Uploaded, Current: {uploadModuleList[i].moduleName}",
                                                     $"{uploadedBundleCount}/{uploadModuleList[i].bundleDic.Count} Bundles Uploaded, Current: {item.Key}",
                                                     (float)uploadedBundleCount / (float)uploadModuleList[i].bundleDic.Count);

                    using PutObjectResult result = client.PutObject(bucketName, key, localFile);
                    if (result.HttpStatusCode != HttpStatusCode.OK)
                    {
                        EditorUtility.ClearProgressBar();
                        Debug.LogError(result.HttpStatusCode);
                        return;
                    }
                    uploadedBundleCount++;
                }
                uploadedModuleCount++;
            }

            using PutObjectResult manifestResult = client.PutObject(bucketName, manifestUrl, Path.Combine(Application.dataPath, buildOption.bundleOutputDir, "AssetModuleManifest.json"));
            if (manifestResult.HttpStatusCode != HttpStatusCode.OK)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError(manifestResult.HttpStatusCode);
                return;
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("Sync with server successfully.");
        }
    }
}
