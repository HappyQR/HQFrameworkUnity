using HQFramework;
using HQFramework.Procedure;
using HQFramework.Runtime;
using UnityEngine;
using HQFramework.Resource;

public class HotfixProcedure : ProcedureBase
{
    private ResourceComponent resourceComponent;

    private GameObject cube1;
    private GameObject cube2;
    private Texture2D tex1;

    protected override void OnEnter()
    {
        HQDebugger.Log("Hotfix Procedure Enter");

        resourceComponent = GameEntry.GetModule<ResourceComponent>();

        // int hotfixID = resourceComponent.LaunchHotfixCheck();
        // resourceComponent.AddHotfixCheckErrorEvent(hotfixID, (args) => HQDebugger.LogError(args.errorMessage));
        // resourceComponent.AddHotfixCheckCompleteEvent(hotfixID, (args) => HQDebugger.LogInfo("forceupdate : " + args.forceUpdate + "\ntotalsize : " + args.totalSize + "\nreleasenote : " + args.releaseNote));
        // resourceComponent.AddHotfixDownloadUpdateEvent(hotfixID, (args) => HQDebugger.LogInfo((float)args.DownloadedSize / args.TotalSize));
        // resourceComponent.AddHotfixDownloadErrorEvent(hotfixID, (args) => HQDebugger.LogError(args.ErrorMessage));
        // resourceComponent.AddHotfixDownloadCompleteEvent(hotfixID, (args) => HQDebugger.LogInfo("Hotfix Done."));
    }

    protected override void OnUpdate()
    {
        // BundleData[] data = resourceComponent.GetLoadedBundleData();
        // for (int i = 0; i < data.Length; i++)
        // {
        //     HQDebugger.LogInfo($"Bundle Name: {data[i].bundleName}, Bundle Ref Count: {data[i].refCount}, Ready: {data[i].ready}");
        // }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // resourceComponent.LaunchHotfix();

            resourceComponent.LoadAsset<Texture2D>("Assets/GameAssets/Public/Textures/skybox22.jpg", OnLoadTexComplete, null);

            resourceComponent.LoadAsset<GameObject>(2987955044, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(1937126282, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(2529943260, OnLoadAssetComplete, OnLoadAssetError);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            resourceComponent.ReleaseAsset(cube1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            resourceComponent.ReleaseAsset(cube2);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            resourceComponent.ReleaseAsset(tex1);
        }
    }

    private void OnLoadTexComplete(ResourceLoadCompleteEventArgs<Texture2D> args)
    {
        tex1 = args.asset;
    }

    private void OnLoadAssetComplete(ResourceLoadCompleteEventArgs<GameObject> args)
    {
        if (args.crc == 2987955044)
        {
            cube1 = args.asset;
            cube2 = resourceComponent.InstantiateAsset(args.asset);
        }
    }

    private void OnLoadAssetError(ResourceLoadErrorEventArgs args)
    {
        HQDebugger.LogError(args.errorMessage);
    }

    protected override void OnExit()
    {
        HQDebugger.Log("Hotfix Procedure Exit");
        resourceComponent = null;
    }
}