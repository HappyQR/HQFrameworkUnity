using System.Collections;
using System.Collections.Generic;
using HQFramework;
using HQFramework.Coroutine;
using HQFramework.Procedure;
using UnityEngine;
using HQFramework.Download;

public class ResourceDecompressProcedure : ProcedureBase
{
    ICoroutineManager coroutineManager;
    IDownloadManager downloadManager;
    int coroutineID;
    int downloadID;

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

        
        downloadManager = HQFrameworkEngine.GetModule<IDownloadManager>();
        downloadManager.InitDownloadModule(10, 5);
        string url = "https://happyq-test.oss-cn-beijing.aliyuncs.com/AssetFramework/tropical_beach_day.png";
        string filePath = Application.persistentDataPath + "/tropical_beach_day.png";
        downloadID = downloadManager.AddDownload(url, filePath, true, true, 0, 0);

        downloadManager.AddDownloadCancelEvent(downloadID, (info) =>
        {
            HQDebugger.LogInfo("Download Cancel : " + info.id);
        });

        downloadManager.AddDownloadUpdateEvent(downloadID, (info) =>
        {
            HQDebugger.Log("Download Progress : " + (float)info.DownloadedSize / info.TotalSize);
        });

        downloadManager.AddDownloadPauseEvent(downloadID, (info) =>
        {
            HQDebugger.LogInfo("Download Paused : " + info.id);
        }); 

        downloadManager.AddDownloadResumeEvent(downloadID, (info) =>
        {
            HQDebugger.LogInfo("Download Resume : " + info.id);
        });

        downloadManager.AddDownloadErrorEvent(downloadID, (info) =>
        {
            HQDebugger.LogError("Download Error : " + info.ErrorMsg);
        });

        downloadManager.AddDownloadCompleteEvent(downloadID, (info) =>
        {
            HQDebugger.LogInfo("Download Done : " + info.id);
        });

        downloadManager.AddDownloadHashCheckEvent(downloadID, (info) =>
        {
            HQDebugger.LogInfo("Download Hash Check Result : " + info.Result + "\nLocal Hash : " + info.LocalHash + "\nRemote Hash : " + info.TargetHash);
        });
    }

    protected override void OnUpdate()
    {
        // HQDebugger.Log("ResourceDecompressProcedure Update");

        if (Input.GetKeyDown(KeyCode.P))
        {
            coroutineManager.PauseCoroutine(coroutineID);
            downloadManager.PauseDownload(downloadID);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            coroutineManager.ResumeCoroutine(coroutineID);
            downloadManager.ResumeDownload(downloadID);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            coroutineManager.StopCoroutine(coroutineID);
            downloadManager.StopDownload(downloadID);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            coroutineManager.StopCoroutines(0);
            downloadManager.StopDownloads(0);
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
