namespace HQFramework.Coroutine
{
    public sealed class YieldSecondsRealtime: IAsyncOperation
    {
        private float waitUntilTime = -1f;

        //
        // Summary:
        //     The given amount of seconds that the yield instruction will wait for.
        public float waitTime { get; set; }

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
                    waitUntilTime = -1f;
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
        public YieldSecondsRealtime(float time)
        {
            waitTime = time;
        }
    }
}
