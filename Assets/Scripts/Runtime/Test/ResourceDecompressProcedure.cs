using System.Collections;
using System.IO;
using HQFramework;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceDecompressProcedure : ProcedureBase
{
    private ResourceComponent resourceManager;

    protected override void OnEnter()
    {
        HQDebugger.Log("ResourceDecompressProcedure Enter");

        resourceManager = GameEntry.GetModule<ResourceComponent>();
        resourceManager.LoadAsset<GameObject>(2987955044, OnLoadAssetComplete, OnLoadAssetError);
        resourceManager.DecompressBuiltinAssets(OnDecompressComplete);
    }

    private void OnDecompressComplete()
    {
        SwitchProcedure<HotfixProcedure>();
    }

    protected override void OnExit()
    {
        resourceManager = null;
        HQDebugger.Log("ResourceDecompressProcedure Exit");
    }

    private void OnLoadAssetComplete(ResourceLoadCompleteEventArgs<GameObject> args)
    {
        // if (args.crc == 2987955044)
        // {
        //     // cube1 = args.asset;
        //     cube2 = resourceComponent.InstantiateAsset(args.asset);
        // }
        Object.Instantiate(args.asset);
    }

    private void OnLoadAssetError(ResourceLoadErrorEventArgs args)
    {
        HQDebugger.LogError($"{args.assetPath}, {args.errorMessage}");
    }
}
