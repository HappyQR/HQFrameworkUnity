using HQFramework;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine;

public class ResourceLoadProcedure : ProcedureBase
{
    private ResourceComponent resourceComponent;

    private GameObject cube1;
    private Texture2D tex1;

    protected override void OnEnter()
    {
        resourceComponent = GameEntry.GetModule<ResourceComponent>();
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // resourceComponent.LoadAsset<Texture2D>("Assets/GameAssets/Public/Textures/skybox22.jpg", OnLoadTexComplete, null);
            // resourceComponent.LoadAsset<GameObject>(2987955044, OnLoadAssetComplete, OnLoadAssetError);
            // resourceComponent.LoadAsset<GameObject>(1937126282, OnLoadAssetComplete, OnLoadAssetError);
            // resourceComponent.LoadAsset<GameObject>(2529943260, OnLoadAssetComplete, OnLoadAssetError);

            resourceComponent.InstantiateAsset<GameObject>(2987955044, OnInstantiateComplete, OnInstantiateError);
            resourceComponent.LoadAsset<Texture2D>("Assets/GameAssets/Public/Textures/skybox22.jpg", OnInstantiateTexComplete, OnInstantiateError);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            resourceComponent.ReleaseAsset(cube1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            resourceComponent.ReleaseAsset(tex1);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AssetBundleInfo[] bundleData = resourceComponent.GetLoadedBundleInfo();
            AssetItemInfo[] assetData = resourceComponent.GetLoadedAssetInfo();

            HQDebugger.Log(null);

            for (int i = 0; i < bundleData.Length; i++)
            {
                HQDebugger.LogInfo($"Bundle Name: {bundleData[i].bundleName}, Bundle Ref Count: {bundleData[i].refCount}, Status: {bundleData[i].status}");
            }

            for (int i = 0; i < assetData.Length; i++)
            {
                HQDebugger.LogInfo($"Asset Name: {assetData[i].assetPath}, Asset Ref Count: {assetData[i].refCount}, Status: {assetData[i].status}");
            }
        }
    }

    protected override void OnExit()
    {
        resourceComponent = null;
    }

    // private void OnLoadTexComplete(ResourceLoadCompleteEventArgs<Texture2D> args)
    // {
    //     tex1 = args.asset;
    // }

    // private void OnLoadAssetComplete(ResourceLoadCompleteEventArgs<GameObject> args)
    // {
    //     Object.Instantiate(args.asset);
    // }

    // private void OnLoadAssetError(ResourceLoadErrorEventArgs args)
    // {
    //     HQDebugger.LogError(args.errorMessage);
    // }

    private void OnInstantiateTexComplete(ResourceLoadCompleteEventArgs<Texture2D> args)
    {
        tex1 = args.asset;
    }

    private void OnInstantiateComplete(ResourceLoadCompleteEventArgs<GameObject> args)
    {
        cube1 = args.asset;
    }

    private void OnInstantiateError(ResourceLoadErrorEventArgs args)
    {
        HQDebugger.LogError(args.errorMessage);
    }
}
