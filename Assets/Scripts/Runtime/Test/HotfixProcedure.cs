using HQFramework;
using HQFramework.Procedure;
using HQFramework.Runtime;
using UnityEngine;

public class HotfixProcedure : ProcedureBase
{
    private ResourceComponent resourceComponent;

    protected override void OnEnter()
    {
        HQDebugger.Log("Hotfix Procedure Enter");

        resourceComponent = GameEntry.GetModule<ResourceComponent>();

        int hotfixID = resourceComponent.LaunchHotfixCheck();
        resourceComponent.AddHotfixCheckErrorEvent(hotfixID, (args) => HQDebugger.LogError(args.errorMessage));
        resourceComponent.AddHotfixCheckCompleteEvent(hotfixID, (args) => 
        {
            HQDebugger.LogInfo("forceupdate : " + args.forceUpdate + "\ntotalsize : " + args.totalSize + "\nreleasenote : " + args.releaseNote);

            if (args.isLatest)
            {
                HQDebugger.LogInfo("Hotfix Check Clean.");
                // SwitchProcedure<ResourceLoadProcedure>();
                SwitchProcedure<UITestProcedure>();
            }
        });
        resourceComponent.AddHotfixDownloadUpdateEvent(hotfixID, (args) => HQDebugger.LogInfo((float)args.DownloadedSize / args.TotalSize));
        resourceComponent.AddHotfixDownloadErrorEvent(hotfixID, (args) => HQDebugger.LogError(args.ErrorMessage));
        resourceComponent.AddHotfixDownloadCompleteEvent(hotfixID, (args) => 
        {
            HQDebugger.LogInfo("Hotfix Done.");
            // SwitchProcedure<ResourceLoadProcedure>();
            SwitchProcedure<UITestProcedure>();
        });
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            resourceComponent.LaunchHotfix();
        }
    }

    protected override void OnExit()
    {
        HQDebugger.Log("Hotfix Procedure Exit");
        resourceComponent = null;
    }
}