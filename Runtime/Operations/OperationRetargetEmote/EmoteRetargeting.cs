using System;
using System.Threading;
using System.Threading.Tasks;
using Kinetix.Internal.Retargeting;
using Kinetix.Internal.Utils;
using UnityEngine;

namespace Kinetix.Internal
{
	public class EmoteRetargeting<TResponseType, TExporter> : Operation<EmoteRetargetingConfig, EmoteRetargetingResponse<TResponseType>> where TExporter : ARetargetExport<TResponseType>, new()
	{
		public EmoteRetargeting(EmoteRetargetingConfig _Config): base(_Config) {}

		public override async Task Execute()
		{
			if (CancellationTokenSource.IsCancellationRequested || Config.CancellationSequencer.canceled)
			{
				CurrentTaskCompletionSource.TrySetCanceled();
				return;
			}

			CheckRetargeting();
			
			bool useWebRequest = false;

			if (!string.IsNullOrEmpty(Config.Avatar.AvatarID) && Config.IsAnimationRT3K)
			{
				RetargetTask<TResponseType, TExporter> retargetTask = GetPreRetargetTask(useWebRequest);
				RetargetTaskManager.AddTask(Config.Emote.Ids.UUID, retargetTask);
				TExporter export = await retargetTask.AwaitAll();

				if (CancellationTokenSource.IsCancellationRequested || Config.CancellationSequencer.canceled)
				{

					CurrentTaskCompletionSource.TrySetCanceled();
					return;
				}

				EmoteRetargetingResponse<TResponseType> emoteRetargetingResponse = new EmoteRetargetingResponse<TResponseType>
				{
					RetargetedClip = export.result,
					EstimatedClipSize = export.MemorySize
				};

				CurrentTaskCompletionSource.SetResult(emoteRetargetingResponse);
				return;
			}

			try
			{
				RetargetTask<TResponseType, TExporter> retargetTask = GetRetargetTask(useWebRequest);
				RetargetTaskManager.AddTask(Config.Emote.Ids.UUID, retargetTask);

				if (Config.AwaitAll)
				{
					await retargetTask
					.AwaitAll()
					.Then(RetargetCallBack);
				}
				else
				{
					await retargetTask
					.AwaitFrame(60)
					.Then(RetargetCallBack);
				}

				await CurrentTaskCompletionSource.Task;
			}
			catch (Exception e)
			{
				KinetixDebug.LogException(e);
			}

			async Task RetargetCallBack(TExporter exporter)
			{
				if (Config.AwaitAll)
				{
					await Task.Delay(20); //Give a bit of delay to release the file
				}

				TResponseType clip = exporter.result;
				long estimationSize = exporter.MemorySize;

				if (CancellationTokenSource.IsCancellationRequested || Config.CancellationSequencer.canceled)
				{
					CurrentTaskCompletionSource.TrySetCanceled();
					return;
				}

				EmoteRetargetingResponse<TResponseType> emoteRetargetingResponse = new EmoteRetargetingResponse<TResponseType>
				{
					RetargetedClip = clip,
					EstimatedClipSize = estimationSize
				};

				CurrentTaskCompletionSource.SetResult(emoteRetargetingResponse);

				KinetixDebug.Log("Retargeted : " + Config.Emote.FilePath);
			}
		}

		private RetargetTask<TResponseType, TExporter> GetRetargetTask(bool useWebRequest)
		{
			if (Config.Indexer != null)
				return RetargetingManager.GetRetargetedAnimationClip<TResponseType, TExporter>(Config.Avatar.Avatar, Config.Indexer, Config.Priority, Config.CancellationSequencer, useWebRequest: useWebRequest);

			return RetargetingManager.GetRetargetedAnimationClip<TResponseType, TExporter>(Config.Avatar.Avatar, Config.Path, Config.Priority, Config.CancellationSequencer, useWebRequest: useWebRequest);
		}

		private RetargetTask<TResponseType, TExporter> GetPreRetargetTask(bool useWebRequest)
		{
			if (Config.Indexer != null)
				return RetargetingManager.LoadPreRetargetedAnimation<TResponseType, TExporter>(Config.Avatar.Avatar, Config.Indexer, Config.CancellationSequencer, useWebRequest:useWebRequest);
			
			return RetargetingManager.LoadPreRetargetedAnimation<TResponseType, TExporter>(Config.Avatar.Avatar, Config.Path, Config.CancellationSequencer, useWebRequest: useWebRequest);
		}

		private async void CheckRetargeting()
		{
			while (!CurrentTaskCompletionSource.Task.IsCompleted)
			{
				try
				{
					await KinetixYield.Yield();
				}
				catch (TaskCanceledException e)
				{
#if DEV_KINETIX
					KinetixLogger.LogInfo("Cancel", "Canceled an operation. " + Environment.NewLine + e.StackTrace, true);
#endif
					CancellationTokenSource.Cancel();
				}

				if (CancellationTokenSource.IsCancellationRequested || Config.CancellationSequencer.canceled)
					CurrentTaskCompletionSource.TrySetCanceled();
			}
		}
		
		public override bool Compare(EmoteRetargetingConfig _Config)
		{
			bool path = 
				Config.Path == null && _Config.Path == null || 
				Config.Path != null && Config.Path.Equals(_Config.Path);

			bool emote =
				Config.Emote == null && _Config.Emote == null ||
				Config.Emote != null && Config.Emote.Equals(_Config.Emote);

			bool avatar = Config.Avatar.Equals(_Config.Avatar);

			bool indexer =
				Config.Indexer == null && _Config.Indexer == null ||
				Config.Indexer != null && Config.Indexer.GetType().Equals(_Config.Indexer.GetType());

			return path && emote && avatar && indexer;
		}

		public override IOperation<EmoteRetargetingConfig, EmoteRetargetingResponse<TResponseType>> Clone()
		{
			return new EmoteRetargeting<TResponseType, TExporter>(Config);
		}
	}
}

