using HQFramework;
using HQFramework.Download;
using HQFramework.Hotfix;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine;

public class HotfixProcedure : ProcedureBase
{
    private IHotfixManager hotfixManager;

    protected override void OnEnter()
    {
        HQDebugger.Log("Hotfix Procedure Enter");

        HQFrameworkEngine.GetModule<IDownloadManager>().InitDownloadModule(3, 5);

        hotfixManager = HQFrameworkEngine.GetModule<IHotfixManager>();
        hotfixManager.StartHotfixCheck();
        hotfixManager.onHotfixCheckError += (args) =>
        {
            HQDebugger.LogError(args.errorMessage);
        };
        hotfixManager.onHotfixCheckDone += (args) =>
        {
            HQDebugger.LogInfo("forceupdate : " + args.forceUpdate + "\ntotalsize : " + args.totalSize + "\nreleasenote : " + args.releaseNote);
        };
        hotfixManager.onHotfixUpdate += (args) =>
        {
            HQDebugger.LogInfo(args.Progress);
        };
        hotfixManager.onHotfixError += (args) =>
        {
            HQDebugger.LogError(args.ErrorMessage);
        };
        hotfixManager.onHotfixDone += () =>
        {
            HQDebugger.LogInfo("Hotfix Done.");
        };
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            hotfixManager.StartHotfix();
        }
    }

    protected override void OnExit()
    {
        HQDebugger.Log("Hotfix Procedure Exit");
        hotfixManager = null;
    }
}