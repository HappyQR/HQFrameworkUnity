// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using Aliyun.OSS;
// using Aliyun.OSS.Common;
// using System.IO;
// using System;
// using System.Net;
// using System.Net.Http;
// using HQFramework.Resource;

// namespace HQFramework.Editor
// {
//     public class AssetsUploader
//     {
//         private static OssClient client;

//         private static string manifestUrl = "AssetFramework/AssetModuleManifest.json";
//         private static string bucketName = "happyq-test";

//         public static void Init()
//         {
//             if (client == null)
//             {
//                 string[] key_id = File.ReadAllText(Path.Combine(Application.dataPath, "../Build/Aliyun.txt")).Split('|');
//                 string accessId = key_id[0];
//                 string accessKey = key_id[1];
//                 string endpoint = "https://oss-cn-beijing.aliyuncs.com";
//                 string region = "cn-beijing";
//                 ClientConfiguration conf = new ClientConfiguration();
//                 conf.SignatureVersion = SignatureVersion.V4;
//                 conf.ConnectionTimeout = 3000;
//                 conf.EnalbeMD5Check = true;
//                 client = new OssClient(endpoint, accessId, accessKey, conf);
//                 client.SetRegion(region);
//             }
//         }


//         public static async void SyncAssetsWithServer()
//         {
//             AssetBuildOption buildOption = AssetBuildUtility.GetDefaultOption();
//             AssetModuleManifest localManifest = AssetBuildUtility.GetCurrentAssetsManifest().Item1;

//             using HttpClient httpClient = new HttpClient();
//             httpClient.Timeout = TimeSpan.FromSeconds(5);
//             using HttpResponseMessage manifestResponseMsg = await httpClient.GetAsync(buildOption.manifestUploadUrl);
//             if (manifestResponseMsg.IsSuccessStatusCode)
//             {
//                 string msgJson = await manifestResponseMsg.Content.ReadAsStringAsync();
//                 AssetModuleManifest remoteManifest = EditorJsonHelper.ToObject<AssetModuleManifest>(msgJson);

//                 if (localManifest == null)
//                 {
//                     Debug.LogError("local assets manifest doesn't exist.");
//                 }
//                 else
//                 {
//                     try
//                     {
//                         SyncAssets(remoteManifest, localManifest);
//                     }
//                     catch (Exception ex)
//                     {
//                         throw ex;
//                     }
//                     finally
//                     {
//                         EditorUtility.ClearProgressBar();
//                     }
//                 }
//             }
//             else if (manifestResponseMsg.StatusCode == HttpStatusCode.NotFound)
//             {
//                 //Debug.Log("server manifest doesn't exist, first time to upload.");
//                 try
//                 {
//                     UploadAllModule(localManifest);
//                 }
//                 catch (Exception ex)
//                 {
//                     throw ex;
//                 }
//                 finally
//                 {
//                     EditorUtility.ClearProgressBar();
//                 }
//             }
//             else
//             {
//                 Debug.Log(manifestResponseMsg.StatusCode);
//             }
//         }

//         public static void UploadAllModule(AssetModuleManifest localManifest)
//         {
//             AssetBuildOption buildOption = AssetBuildUtility.GetDefaultOption();
//             int uploadedModuleCount = 0;
//             foreach (var moduleInfo in localManifest.moduleDic.Values)
//             {
//                 string keyRoot = $"AssetFramework/{buildOption.genericVersion}/{moduleInfo.moduleName}/";
//                 int uploadedBundleCount = 0;
//                 foreach (var item in moduleInfo.bundleDic)
//                 {
//                     string localFile = Path.Combine(buildOption.bundleOutputDir,
//                                                     buildOption.genericVersion.ToString(),
//                                                     moduleInfo.moduleName,
//                                                     item.Key);
//                     string key = $"{keyRoot}{moduleInfo.currentPatchVersion}/{item.Key}";

//                     EditorUtility.DisplayProgressBar($"{uploadedModuleCount}/{localManifest.moduleDic.Count} Modules Uploaded, Current: {moduleInfo.moduleName}",
//                                                      $"{uploadedBundleCount}/{moduleInfo.bundleDic.Count} Bundles Uploaded, Current: {item.Key}",
//                                                      (float)uploadedBundleCount / (float)moduleInfo.bundleDic.Count);

//                     using PutObjectResult result = client.PutObject(bucketName, key, localFile);
//                     if (result.HttpStatusCode != HttpStatusCode.OK)
//                     {
//                         EditorUtility.ClearProgressBar();
//                         Debug.LogError(result.HttpStatusCode);
//                         return;
//                     }
//                     uploadedBundleCount++;
//                 }
//                 uploadedModuleCount++;
//             }

//             using PutObjectResult manifestResult = client.PutObject(bucketName, manifestUrl, Path.Combine(buildOption.manifestOutputDir, "AssetModuleManifest.json"));
//             if (manifestResult.HttpStatusCode != HttpStatusCode.OK)
//             {
//                 EditorUtility.ClearProgressBar();
//                 Debug.LogError(manifestResult.HttpStatusCode);
//                 return;
//             }

//             EditorUtility.ClearProgressBar();
//             Debug.Log("Sync with server successfully.");
//         }

//         public static void SyncAssets(AssetModuleManifest remoteManifest, AssetModuleManifest localManifest)
//         {
//             AssetBuildOption buildOption = AssetBuildUtility.GetDefaultOption();
//             if (remoteManifest.genericVersion != localManifest.genericVersion)
//             {
//                 UploadAllModule(localManifest);
//                 return;
//             }

//             List<AssetModuleInfo> uploadModuleList = new List<AssetModuleInfo>();

//             // handle obsolete modules according to business logic
//             // List<AssetModuleInfo> obsoleteModuleList = new List<AssetModuleInfo>();

//             foreach (var item in localManifest.moduleDic)
//             {
//                 if (remoteManifest.moduleDic.ContainsKey(item.Key) &&
//                     remoteManifest.moduleDic[item.Key].currentPatchVersion == item.Value.currentPatchVersion)
//                 {
//                     continue;
//                 }

//                 uploadModuleList.Add(item.Value);
//             }

//             if (uploadModuleList.Count == 0)
//             {
//                 Debug.Log("Check Clean, Done.");
//                 return;
//             }

//             int uploadedModuleCount = 0;

//             for (int i = 0; i < uploadModuleList.Count; i++)
//             {
//                 string keyRoot = $"AssetFramework/{buildOption.genericVersion}/{uploadModuleList[i].moduleName}/";
//                 int uploadedBundleCount = 0;
//                 foreach (var item in uploadModuleList[i].bundleDic)
//                 {
//                     string localFile = Path.Combine(buildOption.bundleOutputDir,
//                                                     buildOption.genericVersion.ToString(),
//                                                     uploadModuleList[i].moduleName,
//                                                     item.Key);
//                     string key = $"{keyRoot}{uploadModuleList[i].currentPatchVersion}/{item.Key}";

//                     EditorUtility.DisplayProgressBar($"{uploadedModuleCount}/{uploadModuleList.Count} Modules Uploaded, Current: {uploadModuleList[i].moduleName}",
//                                                      $"{uploadedBundleCount}/{uploadModuleList[i].bundleDic.Count} Bundles Uploaded, Current: {item.Key}",
//                                                      (float)uploadedBundleCount / (float)uploadModuleList[i].bundleDic.Count);

//                     using PutObjectResult result = client.PutObject(bucketName, key, localFile);
//                     if (result.HttpStatusCode != HttpStatusCode.OK)
//                     {
//                         EditorUtility.ClearProgressBar();
//                         Debug.LogError(result.HttpStatusCode);
//                         return;
//                     }
//                     uploadedBundleCount++;
//                 }
//                 uploadedModuleCount++;
//             }

//             using PutObjectResult manifestResult = client.PutObject(bucketName, manifestUrl, Path.Combine(buildOption.manifestOutputDir, "AssetModuleManifest.json"));
//             if (manifestResult.HttpStatusCode != HttpStatusCode.OK)
//             {
//                 EditorUtility.ClearProgressBar();
//                 Debug.LogError(manifestResult.HttpStatusCode);
//                 return;
//             }

//             EditorUtility.ClearProgressBar();
//             Debug.Log("Sync with server successfully.");
//         }
//     }
// }
