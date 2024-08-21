using Kinetix.Internal.Kinanim;
using Kinetix.Internal.Kinanim.Compression;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Math = System.Math;

namespace Kinetix.Internal
{
	public struct DownloadUsage
	{
		/// <summary>
		/// Total number of simultaneous request
		/// </summary>
		public int numberOfDownloads;
		/// <summary>
		/// Total quantity of requested data in bytes
		/// </summary>
		public long totalBytes;

		public DownloadUsage(int _NumberOfDownloads, long _TotalBytes)
		{
			this.numberOfDownloads = _NumberOfDownloads;
			this.totalBytes = _TotalBytes;
		}
	}

	/// <summary>
	/// Service that calculates download speed of emotes.
	/// </summary>
	public class EmoteDownloadSpeedService : IKinetixService
	{
		public const float BYTE_TO_KILOBYTE = 1080f;
		public const float KILOBYTE_TO_BYTE = 1f / BYTE_TO_KILOBYTE;
		public const int MAX_RECORD_COUNT = 50;

		public const int MIN_FRAME_COUNT = InterpoCompression.DEFAULT_BATCH_SIZE;
		/// <summary>
		/// Max duration of a request in seconds
		/// </summary>
		public const double MAX_WAIT_TIME = 2;
		/// <summary>
		/// Minimum of request to consider when calculating bytes to download
		/// </summary>
		public const int MINIMUM_DOWNLOAD_REQUEST_HANDLING = 10;
		
		/// <summary>
		/// Recorded rates in bytes/seconds
		/// </summary>
		private readonly List<double> records = new List<double>();
		private readonly List<DownloadSpeedRecorder> recorders = new List<DownloadSpeedRecorder>();
		private int recordsReplaceIndex = 0;
		private bool isUsageDirty = true;

		/// <summary>
		/// Average rate in bytes/seconds
		/// </summary>
		private double average;
		private long usage;
		private double bandwidthLimit;

		private List<(long min, long max, int frameCount)> debugCalculatedRanges = new List<(long min, long max, int frameCount)>();

		public (long min, long max, int minFrame, int maxFrame) CalculateFramesDownloadByteRange(KinanimImporter importer)
		{
			CalculateStats();
			DownloadUsage usage = CalculateUsage();

			//Get quantity of bytes to download
			int requestCount = Mathf.Max(MINIMUM_DOWNLOAD_REQUEST_HANDLING, usage.numberOfDownloads);
			long maxByteCount = (long)Math.Floor(bandwidthLimit / requestCount);

			//Calculate byte positions
			long byteMin = importer.Result.header.BinarySize - 1,
				 byteMax = byteMin;

			KinanimData.KinanimHeader header = importer.Result.header;
			ushort totalFrameCount = header.FrameCount;
			int minFrame = importer.HighestImportedFrame + 1;

			int maxFrame = -1;
			while (++maxFrame < totalFrameCount && ((byteMax - byteMin) < maxByteCount ||  (maxFrame-minFrame) < MIN_FRAME_COUNT))
			{
				if (maxFrame < minFrame)
				{
					byteMin += header.FrameSizes[maxFrame];
					byteMax = byteMin;
				}
				else
				{
					byteMax += header.FrameSizes[maxFrame];
				}
			}
			--maxFrame;
			byteMax -= 1; //We work using lenght but downloads works based on position

			debugCalculatedRanges.Add((byteMin,  byteMax, maxFrame-minFrame+1));
			return (byteMin, byteMax, minFrame, maxFrame);
		}

		public double CalculateAverage()
		{
			CalculateStats();

			return average;
		}

		/// <summary>
		/// Calculate currently downloading bytes
		/// </summary>
		/// <returns></returns>
		public DownloadUsage CalculateUsage()
		{
			CalculateStats();

			return new DownloadUsage(recorders.Count, usage);
		}

		internal DownloadSpeedRecorder RecordDownload()
		{
			DownloadSpeedRecorder toReturn = new DownloadSpeedRecorder();
			recorders.Add(toReturn);
			toReturn.OnStop += Recorder_OnStop;

			isUsageDirty = true;

			return toReturn;
		}

		private void Recorder_OnStop(DownloadSpeedRecorder _Recorder)
		{
			_Recorder.OnStop -= Recorder_OnStop;
			recorders.Remove(_Recorder);
			RegisterRate(_Recorder.downloadByteCount / _Recorder.Elapsed.TotalSeconds);
		}

		private void RegisterRate(double _Rate)
		{
			if (records.Count < MAX_RECORD_COUNT)
				records.Add(_Rate);
			else
			{
				records[recordsReplaceIndex] = _Rate;
				recordsReplaceIndex = (1 + recordsReplaceIndex) % MAX_RECORD_COUNT;
			}

			isUsageDirty = true;
		}

		private void CalculateStats()
		{
			if (!isUsageDirty)
				return;

			average        =   records.Count == 0 ? 0 : records.Average();
			usage          = recorders.Count == 0 ? 0 : recorders.Sum(SumUsage);
			bandwidthLimit = MAX_WAIT_TIME * average;

			isUsageDirty = false;

			static long SumUsage(DownloadSpeedRecorder recorder) => recorder.downloadByteCount;
		}

	}
}
