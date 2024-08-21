using Kinetix.Internal.Utils;
using System;

namespace Kinetix.Internal
{
    /// <summary>
    /// A <see cref="CustomStopWatch"/> for the service <see cref="EmoteDownloadSpeedService"/>.<br/>
    /// </summary>
    /// <example>
    /// Example implementation :<br/>
    /// \code
    /// DownloadSpeedRecorder recorder = serviceLocator.Get&lt;EmoteDownloadSpeedService&gt;().RecordDownload();
    /// recorder.Start( /* Size in bytes */ )
    /// 
    /// await /* Your download operation */
    /// 
    /// recorder.Stop(); //Data is automatically collected by the service
    /// recorder = null;
    /// \endcode
    /// </example>
    internal class DownloadSpeedRecorder : CustomStopWatch
	{
		public event Action<DownloadSpeedRecorder> OnStop;

		public long downloadByteCount;
		public bool IsRecording => isRunning;

		[Obsolete("Please use Start(long) method")]
#pragma warning disable CS0809 //Obsolete member overrides non-obsolete member.
		public override void Start()
		{
			KinetixLogger.LogWarning(nameof(DownloadSpeedRecorder), "Please use Start(long) method", true);
			Start(0);
		}
#pragma warning restore CS0809

		public void Start(long downloadByteCount)
		{
			this.downloadByteCount = downloadByteCount;
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
			OnStop?.Invoke(this);
		}
	}
}
