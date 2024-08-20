using System;

namespace HQFramework
{
    public static class TimeManager
    {
        public static float startTime { get; private set; }
        public static float logicTimeSinceStart { get; private set; }
        public static float realtimeSinceStart { get; private set; }
        public static float deltaTimeLogic { get; private set; }
        public static float deltaTimeRealtime { get; private set; }
        public static float logicTimeScale { get; private set; }

        internal static void Initialize()
        {
            startTime = (float)TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
            logicTimeScale = 1;
        }

        internal static void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            logicTimeSinceStart += elapseSeconds;
            realtimeSinceStart += realElapseSeconds;
            deltaTimeLogic = elapseSeconds;
            deltaTimeRealtime = realElapseSeconds;
            logicTimeScale = elapseSeconds / realElapseSeconds;
        }
    }
}
