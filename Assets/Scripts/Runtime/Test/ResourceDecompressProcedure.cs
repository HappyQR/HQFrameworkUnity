using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Coroutine;
using HQFramework.Procedure;
using UnityEngine;

public class ResourceDecompressProcedure : ProcedureBase
{
    ICoroutineManager coroutineManager;
    int coroutineID;

    protected override void OnEnter()
    {
        HQDebugger.Log("ResourceDecompressProcedure Enter");

        coroutineManager = HQFrameworkEngine.GetModule<ICoroutineManager>();
        coroutineManager.RepeatInvoke(0.02f, () =>
        {
            HQDebugger.Log("Repeat...");
        });
        coroutineID = coroutineManager.StartCoroutine(TestCoroutine());
        coroutineManager.AddCoroutinePauseEvent(coroutineID, (info) =>
        {
            HQDebugger.LogInfo("Pause : " + info.id);
        });

        coroutineManager.AddCoroutineStopEvent(coroutineID, (info) =>
        {
            HQDebugger.LogInfo("Stop : " + info.id);
        });

        coroutineManager.AddCoroutineResumeEvent(coroutineID, (info) =>
        {
            HQDebugger.LogInfo("Resume : " + info.id);
        });
    }

    protected override void OnUpdate()
    {
        // HQDebugger.Log("ResourceDecompressProcedure Update");

        if (Input.GetKeyDown(KeyCode.P))
        {
            coroutineManager.PauseCoroutine(coroutineID);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            coroutineManager.ResumeCoroutine(coroutineID);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            coroutineManager.StopCoroutine(coroutineID);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            coroutineManager.StopCoroutines(0);
        }
    }

    protected override void OnExit()
    {
        HQDebugger.Log("ResourceDecompressProcedure Exit");
    }

    protected override void OnShutdown()
    {
        HQDebugger.Log("ResourceDecompressProcedure Shutdown");
    }

    private IEnumerator TestCoroutine()
    {
        HQDebugger.LogInfo("Hello HQ Framework");

        yield return new YieldSecondsRealtime(3);

        while (true)
        {
            HQDebugger.LogInfo("I'm in...");
            yield return null;
        }
    }

    private IEnumerator TestCoroutine2()
    {
        HQDebugger.LogInfo("Hello HQ Framework");

        yield return new YieldSecondsRealtime(3);

        while (true)
        {
            HQDebugger.LogInfo("I'm in...");
            yield return null;
        }
    }
}
