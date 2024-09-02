namespace HQFramework.Coroutine
{
    public class YieldSecondsLogic : IAsyncOperation
    {
        private float waitUntilTime = -1f;

        //
        // Summary:
        //     The given amount of seconds that the yield instruction will wait for.
        private readonly float waitTime;

        public bool isDone
        {
            get
            {
                if (waitUntilTime < 0f)
                {
                    waitUntilTime = TimeManager.realtimeSinceStart + waitTime;
                }

                bool flag = TimeManager.realtimeSinceStart < waitUntilTime;
                if (!flag)
                {
                    waitUntilTime = -1;
                }

                return !flag;
            }
        }

        //
        // Summary:
        //     Creates a yield instruction to wait for a given number of seconds using unscaled
        //     time.
        //
        // Parameters:
        //   time:
        public YieldSecondsLogic(float time)
        {
            waitTime = time * TimeManager.logicTimeScale;
        }
    }
}
