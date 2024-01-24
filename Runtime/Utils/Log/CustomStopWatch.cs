#if !UNITY_EDITOR
#undef KINETIX_PROFILER
#endif

using System;
using System.Diagnostics;

namespace Kinetix.Internal.Utils
{
    [Serializable]
	internal class CustomStopWatch
	{
		private bool isRunning;
		private long start;
		private long end;
		public long jitter;
        public TimeSpan Elapsed => new TimeSpan(TickElapsed);
        protected long TickElapsed
		{
			get
			{
				if (isRunning)
				{
					return Stopwatch.GetTimestamp() - start - jitter;
				}
				else
				{
					return end - start - jitter;
				}
			}
		}

		public virtual void Start()
		{
			start = Stopwatch.GetTimestamp();
			isRunning = true;
		}

		public virtual void Stop()
		{
			isRunning = false;
			end = Stopwatch.GetTimestamp();
		}
	}
}
