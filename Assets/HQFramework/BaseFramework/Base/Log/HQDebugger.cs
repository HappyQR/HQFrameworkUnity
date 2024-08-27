using System.Diagnostics;

namespace HQFramework
{
    public static class HQDebugger
    {
        private static ILogHelper logHelper;

        public const string ENABLE_LOG_SYMBOL = "LOG_ENABLE";

        public static void SetHelper(ILogHelper helper)
        {
            logHelper = helper;
        }

        [Conditional(ENABLE_LOG_SYMBOL)]
        public static void Log(object message, LogColor color = LogColor.None)
        {
            logHelper.Log(message, LogLevel.Debug, color);
        }

        [Conditional(ENABLE_LOG_SYMBOL)]
        public static void LogInfo(object message, LogColor color = LogColor.Green)
        {
            logHelper.Log(message, LogLevel.Info, color);
        }

        [Conditional(ENABLE_LOG_SYMBOL)]
        public static void LogWarning(object message)
        {
            logHelper.Log(message, LogLevel.Warning, LogColor.None);
        }

        [Conditional(ENABLE_LOG_SYMBOL)]
        public static void LogError(object message)
        {
            logHelper.Log(message, LogLevel.Error, LogColor.None);
        }

        [Conditional(ENABLE_LOG_SYMBOL)]
        public static void LogFatal(object message)
        {
            logHelper.Log(message, LogLevel.Fatal, LogColor.None);
        }
    }
}
