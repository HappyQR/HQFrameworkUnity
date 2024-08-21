using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HQFramework.Unity
{
    public class UnityLogHelper : ILogHelper, IDisposable
    {
        private ConcurrentQueue<string> logMsgQueue;
        private AutoResetEvent resetEvent;
        private StreamWriter logWritter;
        private string logFilePath;
        private bool logThreadAlive;
        private bool disposed;

        public UnityLogHelper()
        {
            InitLogEvent();
        }

        [Conditional(HQDebugger.ENABLE_LOG_SYMBOL)]
        private void InitLogEvent()
        {
            logMsgQueue = new ConcurrentQueue<string>();
            string logFileDir = Path.Combine(Application.persistentDataPath, "Log");
            string fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".log";
            if (!Directory.Exists(logFileDir))
            {
                Directory.CreateDirectory(logFileDir);
            }
            logFilePath = Path.Combine(logFileDir, fileName);
            logWritter = new StreamWriter(logFilePath);
            logThreadAlive = true;
            resetEvent = new AutoResetEvent(false);
            Thread logThread = new Thread(WriteLogAsync);
            logThread.Start();
            Application.logMessageReceivedThreaded += OnLogReceived;
            Application.quitting += OnApplicationQuit;
        }

        public void Log(object message, LogLevel level, LogColor color)
        {
            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(GetColorLog(message.ToString(), color));
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case LogLevel.Fatal:
                    UnityEngine.Debug.LogAssertion(message);
                    break;
            }
        }

        private string GetColorLog(string msg, LogColor color)
        {
            if (color == LogColor.None)
            {
                return msg;
            }
            switch (color)
            {
                case LogColor.Blue:
                    msg = $"<color=#0000FF>{msg}</color>";
                    break;
                case LogColor.Cyan:
                    msg = $"<color=#00FFFF>{msg}</color>";
                    break;
                case LogColor.Darkblue:
                    msg = $"<color=#8FBC8F>{msg}</color>";
                    break;
                case LogColor.Green:
                    msg = $"<color=#00FF00>{msg}</color>";
                    break;
                case LogColor.Orange:
                    msg = $"<color=#FFA500>{msg}</color>";
                    break;
                case LogColor.Red:
                    msg = $"<color=#FF0000>{msg}</color>";
                    break;
                case LogColor.Yellow:
                    msg = $"<color=#FFFF00>{msg}</color>";
                    break;
                case LogColor.Magenta:
                    msg = $"<color=#FF00FF>{msg}</color>";
                    break;
            }
            return msg;
        }

        private void OnLogReceived(string condition, string stackTrace, LogType type)
        {
            StringBuilder logEntry = new StringBuilder(condition.Length + stackTrace.Length + 100);
            logEntry.Append(type.ToString());
            logEntry.Append(" >>> ");
            logEntry.Append(condition);
            logEntry.Append("\nStack Trace >>> ");
            logEntry.Append(stackTrace);
            logEntry.Append("\n");
            logMsgQueue.Enqueue(logEntry.ToString());
            resetEvent.Set();
        }

        private void OnApplicationQuit()
        {
            Application.logMessageReceivedThreaded -= OnLogReceived;
            logThreadAlive = false;
            resetEvent.Set();
        }

        private void WriteLogAsync()
        {
            while (logThreadAlive || !logMsgQueue.IsEmpty)
            {
                resetEvent.WaitOne();
                if (logMsgQueue.TryDequeue(out string msg))
                {
                    logWritter.Write(msg);
                    logWritter.Flush();
                }
            }
            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                logWritter.Dispose();
                resetEvent.Dispose();
                disposed = true;
            }
        }
    }
}
