using HQFramework;
using HQFramework.Procedure;
using HQFramework.Runtime;
using UnityEngine;
using HQFramework.Resource;

public class HotfixProcedure : ProcedureBase
{
    private ResourceComponent resourceComponent;

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            // resourceComponent.LaunchHotfix();

            resourceComponent.LoadAsset<GameObject>(2987955044, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(1937126282, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(2529943260, OnLoadAssetComplete, OnLoadAssetError);
        }
    }

    private void OnLoadAssetComplete(ResourceLoadCompleteEventArgs<GameObject> args)
    {
        Object.Instantiate(args.asset);
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