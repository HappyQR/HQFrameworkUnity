using System.Collections;
using System.IO;
using HQFramework;
using HQFramework.Procedure;
using HQFramework.Resource;
using HQFramework.Runtime;
using UnityEngine.Networking;

public class ResourceDecompressProcedure : ProcedureBase
{
    private ResourceComponent resourceManager;

    protected override void OnEnter()
    {
        HQDebugger.Log("ResourceDecompressProcedure Enter");

        resourceManager = GameEntry.GetModule<ResourceComponent>();
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
}
