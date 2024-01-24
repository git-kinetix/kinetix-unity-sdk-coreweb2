#if !UNITY_EDITOR
#undef KINETIX_PROFILER
#endif

using System;

namespace Kinetix.Internal.Utils
{
    /// <summary>
    /// A watch that gets the average duration of each loop
    /// </summary>
    internal class LoopWatch : CustomStopWatch
	{
		private int loopCount;

		public TimeSpan Average => new TimeSpan( loopCount == 0 ? 0 : TickElapsed / loopCount );

		public override void Start()
        {
            base.Start();
			loopCount = 0;
        }

        public void Loop()
		{
			loopCount++;
		}

		public override void Stop()
		{
			base.Stop();
			loopCount += 1;
		}
	}
}
