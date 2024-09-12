using System.Collections;
using System.IO;
using HQFramework;
using HQFramework.Coroutine;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine.Networking;

public class ResourceDecompressProcedure : ProcedureBase
{
    private IResourceManager resourceManager;
    private ICoroutineManager coroutineManager;
    private string assetManifestFileName = "AssetModuleManifest.json";

    protected override void OnEnter()
    {
        HQDebugger.Log("ResourceDecompressProcedure Enter");

        JsonLitHelper jsonLitHelper = new JsonLitHelper();
        SerializeManager.SetJsonHelper(jsonLitHelper);

        DefaultResourceHelper helper = new DefaultResourceHelper();
        resourceManager = HQFrameworkEngine.GetModule<IResourceManager>();
        resourceManager.SetHelper(helper);

        coroutineManager = HQFrameworkEngine.GetModule<ICoroutineManager>();
        coroutineManager.StartCoroutine(DecompressBuiltinResource());
    }

    private IEnumerator DecompressBuiltinResource()
    {
        string localManifestFilePath = Path.Combine(resourceManager.PersistentDir, assetManifestFileName);
        if (File.Exists(localManifestFilePath))
        {
            SwitchProcedure<HotfixProcedure>();
            yield break;
        }

        string lcoalManifestUrl = "file://" + Path.Combine(resourceManager.BuiltinDir, assetManifestFileName);
        using UnityWebRequest localManifestRequest = UnityWebRequest.Get(lcoalManifestUrl);
        localManifestRequest.SendWebRequest();
        while (!localManifestRequest.isDone)
        {
            yield return null;
        }
        string manifestJson = localManifestRequest.downloadHandler.text;
        AssetModuleManifest localManifest = SerializeManager.JsonToObject<AssetModuleManifest>(manifestJson);
        foreach (var module in localManifest.moduleDic.Values)
        {
            string moudleDir = Path.Combine(resourceManager.PersistentDir, module.moduleName);
            string moduleUrl = "file://" + Path.Combine(resourceManager.BuiltinDir, module.moduleName);
            if (!Directory.Exists(moudleDir))
            {
                Directory.CreateDirectory(moudleDir);
            }
            foreach (var bundle in module.bundleDic.Values)
            {
                string bundlePath = Path.Combine(moudleDir, bundle.bundleName);
                string bundleUrl = Path.Combine(moduleUrl, bundle.bundleName);
                using UnityWebRequest bundleRequest = UnityWebRequest.Get(bundleUrl);
                bundleRequest.SendWebRequest();
                while (!bundleRequest.isDone)
                {
                    yield return null;
                }
                File.WriteAllBytes(bundlePath, bundleRequest.downloadHandler.data);
            }
        }
        File.WriteAllText(localManifestFilePath, manifestJson);

        SwitchProcedure<HotfixProcedure>();
    }

    protected override void OnExit()
    {
        resourceManager = null;
        coroutineManager = null;
        assetManifestFileName = null;
        HQDebugger.Log("ResourceDecompressProcedure Exit");
    }
}
