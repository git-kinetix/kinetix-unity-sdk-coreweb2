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

		public KinanimLoader(ServiceLocator _ServiceLocator) : base(_ServiceLocator)
		{
		}

		public override string Extension => KinanimData.FILE_EXTENSION;

		public override async Task<RuntimeRetargetFrameIndexer> Load(KinetixEmote emote, string filepath, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			EnsureDirectoryExists(filepath);

			if (kinanimIndexerByFilepath.TryGetValue(filepath, out KinanimServiceData data))
			{
				if (data.exporter != null) data.exporter.Dispose();
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
			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, 0, HEADER_DOWNLOAD_SIZE);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);
			
			string filepath = GetFilePath(emote.Ids.UUID, avatarId);
			EnsureDirectoryExists(filepath);

			KinanimServiceData data = GetStartDataFromEmote(filepath, new MemoryStream(response.bytes), avatarId); //Import header and some frames
			data.exporter.WriteHeader(data.indexer.dataSource.UncompressedHeader);

			//Write frames we created
			int maxFrame = data.indexer.dataSource.compression?.MaxUncompressedFrame ?? data.indexer.dataSource.HighestImportedFrame;
			int frameCount = data.indexer.dataSource.Result.content.frames.Length;
			if (maxFrame >= frameCount)
			{
				maxFrame = frameCount - 1;
			}

			data.exporter.content.WriteFrames(new ArraySegment<KinanimData.FrameData>(data.indexer.dataSource.Result.content.frames, 0, maxFrame + 1).ToArray(), data.indexer.dataSource.Result.header.hasBlendshapes, 0);

			//Download remaining frames
			_ = AsyncDownloadRemainingFrames(data, url, null, false)
				.Catch(KinetixDebug.LogException)
				.Then(data.exporter.Dispose);

			return data.indexer;
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

			int remainingFrames = kinanim.indexer.dataSource.Result.header.FrameCount - (kinanim.indexer.dataSource.HighestImportedFrame + 1);
			int chunkCount = Mathf.CeilToInt(remainingFrames / (float)InterpoCompression.DEFAULT_BATCH_SIZE);
			for (int chunk = 0; chunk < chunkCount; chunk++)
			{
				int minFrame = (kinanim.indexer.dataSource.compression?.MaxUncompressedFrame ?? kinanim.indexer.dataSource.HighestImportedFrame) + 1;
				await LoadBatchFrameKinanim(kinanim.indexer, url, InterpoCompression.DEFAULT_BATCH_SIZE, cancellationTokenDownload); //Partial file download operation (20 frames)

				int maxFrame = kinanim.indexer.dataSource.compression?.MaxUncompressedFrame ?? kinanim.indexer.dataSource.HighestImportedFrame;
				int frameCount = kinanim.indexer.dataSource.Result.content.frames.Length;
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
			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, 0, indexer.dataSource.Result.header.BinarySize);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);

			//Import frames
			MemoryStream stream = new MemoryStream(response.bytes);
			indexer.dataSource.MoveHeaderToUncompressedHeader();
			indexer.dataSource.ReadHeader(stream);
			indexer.UpdateIndexCount();
		}

		private async Task LoadBatchFrameKinanim(KinanimDataIndexer indexer, string url, int frameCount, CancellationTokenSource cancellationToken)
		{
			int minFrame = indexer.dataSource.HighestImportedFrame + 1;
			int maxFrame = minFrame + frameCount - 1;
			// -1 explaination :
			//   let's say frameCount = 3 and minFrame = 5
			//   you download these frames:
			//   5, 6, 7
			//   maxFrame = 7 = 5 + 3 - 1

			KinanimData.KinanimHeader header = indexer.dataSource.Result.header;
			ushort totalFrameCount = header.FrameCount;
			if (maxFrame >= totalFrameCount)
				maxFrame = totalFrameCount - 1;

			//Calculate size
			long byteMin = indexer.dataSource.Result.header.BinarySize - 1,
				 byteMax = byteMin;
			for (int i = 0; i <= maxFrame; i++) //NOTE: can optimise with variables by keeping track of the byte count
			{
				if (i < minFrame)
				{
					byteMin += header.FrameSizes[i];
					byteMax = byteMin;
				}
				else
				{
					byteMax += header.FrameSizes[i];
				}
			}

			byteMax -= 1; //We work using lenght but downloads works based on position

			ByteRangeDownloaderConfig fileDownloadOperationConfig = new ByteRangeDownloaderConfig(url, byteMin, byteMax);
			ByteRangeDownloader fileDownloadOperation = new ByteRangeDownloader(fileDownloadOperationConfig);

			ByteRangeDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);

			//Import frames
			MemoryStream stream = new MemoryStream(response.bytes);
			indexer.dataSource.ReadFrames(stream);
			indexer.UpdateIndexCount();

#if DEV_KINETIX
			KinetixLogger.LogDebug("Kinanim,Download", $"downloaded frames {minFrame}-{maxFrame} (byte={byteMin}-{byteMax})", true);
#endif
		}
	}
}
