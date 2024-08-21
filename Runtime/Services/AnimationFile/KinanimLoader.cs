using Kinetix.Internal.Kinanim;
using Kinetix.Internal.Kinanim.Compression;
using Kinetix.Internal.Retargeting.AnimationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Kinetix.Internal.Utils;

namespace Kinetix.Internal
{
	internal class KinanimLoader : AAnimLoader
	{
		const long HEADER_DOWNLOAD_SIZE = 1000 * 5;

		private readonly Dictionary<string, KinanimServiceData> kinanimIndexerByFilepath = new Dictionary<string, KinanimServiceData>();
		protected readonly EmoteDownloadSpeedService downloadSpeedService;

		public KinanimLoader(ServiceLocator _ServiceLocator) : base(_ServiceLocator)
		{
			downloadSpeedService = _ServiceLocator.Get<EmoteDownloadSpeedService>();
		}

		public override string Extension => KinanimData.FILE_EXTENSION;

		public override async Task<RuntimeRetargetFrameIndexer> Load(KinetixEmote emote, string filepath, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			EnsureDirectoryExists(filepath);

			if (kinanimIndexerByFilepath.TryGetValue(filepath, out KinanimServiceData data))
			{
				data.exporter?.Dispose();
				data.exporter = new KinanimExporter(new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write));
			}
			else
			{
				data = GetStartDataFromEmote(filepath, new FileStream(filepath, FileMode.Open), avatarId); //Import header and some frames
			}

			//Download remaining frames
			string url = emote.GetAnimationURLOrNull(KinanimData.FILE_EXTENSION, avatarId) ?? emote.GetAnimationURLOrNull(KinanimData.FILE_EXTENSION, null);

			if (data.indexer.dataSource.HighestImportedFrame < data.indexer.dataSource.Result.header.FrameCount - 1)
			{
				await
					AsyncDownloadRemainingFrames(data, url, null, true)
					.Catch((e) => KinetixDebug.LogException(e))
					.Then(() => { data.exporter.Dispose(); });
			}

			return await Task.FromResult<RuntimeRetargetFrameIndexer>(data.indexer);
		}

		public override async Task<RuntimeRetargetFrameIndexer> Download(KinetixEmote emote, string url, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			DownloadSpeedRecorder downloadTime = downloadSpeedService.RecordDownload();
			downloadTime.Start(HEADER_DOWNLOAD_SIZE);

			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, 0, HEADER_DOWNLOAD_SIZE);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);

			downloadTime.Stop();

			string filepath = GetFilePath(emote.Ids.UUID, avatarId);
			EnsureDirectoryExists(filepath);

			KinanimServiceData kinanim = GetStartDataFromEmote(filepath, new MemoryStream(response.bytes), avatarId); //Import header and some frames
			kinanim.exporter.WriteHeader(kinanim.indexer.dataSource.UncompressedHeader);

			//Write frames we created
			int maxFrame = kinanim.indexer.dataSource.compression?.MaxUncompressedFrame ?? kinanim.indexer.dataSource.HighestImportedFrame;
			int frameCount = kinanim.indexer.dataSource.Result.content.frames.Length;
			if (maxFrame >= frameCount)
			{
				maxFrame = frameCount - 1;
			}

			kinanim.exporter.content.WriteFrames(new ArraySegment<KinanimData.FrameData>(kinanim.indexer.dataSource.Result.content.frames, 0, maxFrame + 1).ToArray(), kinanim.indexer.dataSource.Result.header.hasBlendshapes, 0);

			//Download remaining frames
			_ = AsyncDownloadRemainingFrames(kinanim, url, null, false)
				.Catch(KinetixDebug.LogException)
				.Then(kinanim.exporter.Dispose);

			return kinanim.indexer;
		}

		private async Task AsyncDownloadRemainingFrames(KinanimServiceData kinanim, string url, CancellationTokenSource cancellationTokenDownload, bool downloadHeader)
		{
			if (downloadHeader)
				await GetServerHeader(kinanim.indexer, url, null);

			// Create a separate task that will load each chunk
			//
			// DL() :
			// Using KinanimDataIndexer
			//
			// For( each chunk (20 frames) )
			//    Requst HTTP
			//     |
			//    Server
			//     |
			//    byte[] / steam
			//    MemorySteam
			//    BinaryReader
			//    ReadFrames / 
			//	  KinanimDataIndexer.IndexFrames();

			int frameCount = kinanim.indexer.dataSource.Result.header.FrameCount;
			int minFrame, maxFrame = kinanim.indexer.dataSource.HighestImportedFrame;
			while (maxFrame < frameCount - 1)
			{
				minFrame = (kinanim.indexer.dataSource.compression?.MaxUncompressedFrame ?? kinanim.indexer.dataSource.HighestImportedFrame) + 1;

				await LoadBatchFrameKinanim(kinanim.indexer, url, cancellationTokenDownload); //Partial file download operation (20 frames)

				maxFrame = kinanim.indexer.dataSource.compression?.MaxUncompressedFrame ?? kinanim.indexer.dataSource.HighestImportedFrame;
				if (maxFrame >= frameCount)
				{
					maxFrame = frameCount - 1;
				}

				kinanim.exporter.OverrideHeader(kinanim.indexer.dataSource.UncompressedHeader); //Frame size changed so we need to rewrite the header
				kinanim.exporter.content.WriteFrames(new ArraySegment<KinanimData.FrameData>(kinanim.indexer.dataSource.Result.content.frames, minFrame, maxFrame - minFrame + 1).ToArray(), kinanim.indexer.dataSource.Result.header.hasBlendshapes, (ushort)minFrame);
			}
		}

		/// <summary>
		/// Import the datas of the kinanim from a stream (file or remote).<br/>s
		/// Datas must start with the header.
		/// </summary>
		/// <remarks>
		/// Stream is auto disposed
		/// </remarks>
		/// <param name="filepath"></param>
		/// <param name="readStream">*Auto disposed</param>
		/// <returns></returns>
		private KinanimServiceData GetStartDataFromEmote(string filepath, Stream readStream, string avatarId)
		{
			EnsureDirectoryExists(filepath);

			KinanimServiceData data;
			//Import Header and extra frames
			KinanimImporter importer = new KinanimImporter(new InterpoCompression());
			BinaryReader byteReader = new BinaryReader(readStream);
			importer.ReadHeader(byteReader);
			importer.ReadFrames(byteReader);
			byteReader.Dispose();
			readStream.Dispose();

			//Set local dictionary with : KinanimDataIndexer
			KinanimDataIndexer indexer = new KinanimDataIndexer(importer, !string.IsNullOrEmpty(avatarId));
			indexer.Init();
			indexer.UpdateIndexCount();
			data = new KinanimServiceData(indexer, new KinanimExporter(new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write)));

			if (!(readStream is FileStream))
				data.exporter.OverrideHeader(data.indexer.dataSource.UncompressedHeader); //Frame size changed so we need to rewrite the header

			kinanimIndexerByFilepath[filepath] = data;
			return data;
		}

		private async Task GetServerHeader(KinanimDataIndexer indexer, string url, CancellationTokenSource cancellationToken)
		{
			DownloadSpeedRecorder downloadTime = downloadSpeedService.RecordDownload();
			downloadTime.Start(indexer.dataSource.Result.header.BinarySize);

			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, 0, indexer.dataSource.Result.header.BinarySize);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);

			downloadTime.Stop();

			//Import frames
			MemoryStream stream = new MemoryStream(response.bytes);
			indexer.dataSource.MoveHeaderToUncompressedHeader();
			indexer.dataSource.ReadHeader(stream);
			indexer.UpdateIndexCount();
		}

		private async Task<(int frameMin, int frameMax)> LoadBatchFrameKinanim(KinanimDataIndexer indexer, string url, CancellationTokenSource cancellationToken)
		{
			(long byteMin, long byteMax, int minFrame, int maxFrame) = downloadSpeedService.CalculateFramesDownloadByteRange(indexer.dataSource);

			DownloadSpeedRecorder downloadTime = downloadSpeedService.RecordDownload();
			downloadTime.Start(byteMax - byteMin + 1);
			
			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, byteMin, byteMax);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);
			downloadTime.Stop();

			//Import frames
			MemoryStream stream = new MemoryStream(response.bytes);
			indexer.dataSource.ReadFrames(stream);
			indexer.UpdateIndexCount();

#if DEV_KINETIX
			KinetixLogger.LogDebug("Kinanim,Download", $"downloaded frames {minFrame}-{maxFrame} (byte={byteMin}-{byteMax})", true);
#endif
			return (minFrame, maxFrame);
		}
	}
}
