using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine;

public class ResourceLoadProcedure : ProcedureBase
{
    private ResourceComponent resourceComponent;

    private GameObject cube1;
    private GameObject cube2;
    private Texture2D tex1;

    protected override void OnEnter()
    {
        resourceComponent = GameEntry.GetModule<ResourceComponent>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        // BundleData[] data = resourceComponent.GetLoadedBundleData();
        // for (int i = 0; i < data.Length; i++)
        // {
        //     HQDebugger.LogInfo($"Bundle Name: {data[i].bundleName}, Bundle Ref Count: {data[i].refCount}, Ready: {data[i].ready}");
        // }

        if (Input.GetKeyDown(KeyCode.A))
        {
            resourceComponent.LoadAsset<Texture2D>("Assets/GameAssets/Public/Textures/skybox22.jpg", OnLoadTexComplete, null);
            resourceComponent.LoadAsset<GameObject>(2987955044, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(1937126282, OnLoadAssetComplete, OnLoadAssetError);
            resourceComponent.LoadAsset<GameObject>(2529943260, OnLoadAssetComplete, OnLoadAssetError);
        }

        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     resourceComponent.ReleaseAsset(cube1);
        //     cube1 = null;
        // }

        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     resourceComponent.ReleaseAsset(cube2);
        //     cube2 = null;
        // }

        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     resourceComponent.ReleaseAsset(tex1);
        //     tex1 = null;
        // }
    }

    protected override void OnExit()
    {
        resourceComponent = null;
    }

    private void OnLoadTexComplete(ResourceLoadCompleteEventArgs<Texture2D> args)
    {
        tex1 = args.asset;
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
        HQDebugger.LogError(args.errorMessage);
    }
}
