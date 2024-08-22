using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Procedure;
using UnityEngine;

public class ResourceDecompressProcedure : ProcedureBase
{
    protected override void OnEnter()
    {
        HQDebugger.Log("ResourceDecompressProcedure Enter");
        HQDebugger.LogInfo(Application.buildGUID);
    }

    protected override void OnUpdate()
    {
        // HQDebugger.Log("ResourceDecompressProcedure Update");
    }

    protected override void OnExit()
    {
        HQDebugger.Log("ResourceDecompressProcedure Exit");
    }

    protected override void OnShutdown()
    {
        HQDebugger.Log("ResourceDecompressProcedure Shutdown");
    }
}
