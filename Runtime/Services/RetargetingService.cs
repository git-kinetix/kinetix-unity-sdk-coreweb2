using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kinetix.Internal.Kinanim;
using Kinetix.Internal.Kinanim.Compression;
using Kinetix.Internal.Retargeting;
using Kinetix.Internal.Retargeting.AnimationData;
using Kinetix.Internal.Utils;
using Kinetix.Utils;
using UnityEngine;

namespace Kinetix.Internal
{
    public class KinanimServiceData
	{
		public KinanimDataIndexer indexer;
		public KinanimExporter exporter;

		public KinanimServiceData(KinanimDataIndexer indexer, KinanimExporter exporter)
		{
			this.indexer = indexer;
			this.exporter = exporter;
		}
	}

	public class RetargetingService : IKinetixService
	{
		private readonly Dictionary<KinetixEmoteAvatarPair, EmoteRetargetedData> retargetedEmoteByAvatar;
		private readonly Dictionary<KinetixEmoteAvatarPair, List<Action>>        OnEmoteRetargetedByAvatar;
		private readonly ServiceLocator                                          serviceLocator;

		public RetargetingService(ServiceLocator _ServiceLocator)
		{
			retargetedEmoteByAvatar   = new Dictionary<KinetixEmoteAvatarPair, EmoteRetargetedData>();
			OnEmoteRetargetedByAvatar = new Dictionary<KinetixEmoteAvatarPair, List<Action>>();

			serviceLocator = _ServiceLocator;
		}

		/// <summary>
		/// If the emote / avatar pair is in cache, returns it
		/// </summary>
		/// <param name="_Emote"></param>
		/// <param name="_Avatar"></param>
		/// <returns></returns>
		public bool HasAnimationRetargeted(KinetixEmote _Emote, KinetixAvatar _Avatar)
		{
			KinetixEmoteAvatarPair pair = new KinetixEmoteAvatarPair() { Emote = _Emote, Avatar = _Avatar };

			if (!retargetedEmoteByAvatar.ContainsKey(pair))
				return false;

			bool hasAnimationRetargeted = retargetedEmoteByAvatar[pair].clipsByType.Values.ToList().Exists(retargetedEmote => retargetedEmote.HasClip());
			return hasAnimationRetargeted;
		}

		#region Notifications

		/// <summary>
		/// Allows for callback registration for when a emote / avatar combination will be retargeted
		/// </summary>
		/// <param name="_Emote"></param>
		/// <param name="_Avatar"></param>
		/// <param name="_OnSucceed"></param>
		public void RegisterCallbacksOnRetargetedByAvatar(KinetixEmote _Emote, KinetixAvatar _Avatar, Action _OnSucceed)
		{
			KinetixEmoteAvatarPair pair = new KinetixEmoteAvatarPair() { Emote = _Emote, Avatar = _Avatar };

			if (HasAnimationRetargeted(_Emote, _Avatar))
			{
				_OnSucceed?.Invoke();
				return;
			}

			if (!OnEmoteRetargetedByAvatar.ContainsKey(pair))
				OnEmoteRetargetedByAvatar.Add(pair, new List<Action>());

			OnEmoteRetargetedByAvatar[pair].Add(_OnSucceed);
		}

		/// <summary>
		/// Callback calling for a emote / avatar combination
		/// </summary>
		/// <param name="_Emote"></param>
		/// <param name="_Avatar"></param>
		private void NotifyCallbackOnRetargetedByAvatar(KinetixEmote _Emote, KinetixAvatar _Avatar)
		{
			KinetixEmoteAvatarPair pair = new KinetixEmoteAvatarPair() { Emote = _Emote, Avatar = _Avatar };

			if (!OnEmoteRetargetedByAvatar.ContainsKey(pair))
				return;

			for (int i = 0; i < OnEmoteRetargetedByAvatar[pair].Count; i++)
			{
				OnEmoteRetargetedByAvatar[pair][i]?.Invoke();
			}

			OnEmoteRetargetedByAvatar.Remove(pair);
		}

		#endregion

		/// <summary>
		/// Get an KinetixClip retargeted for a specific avatar
		/// </summary>
		/// <param name="_Avatar">The Avatar</param>
		/// <param name="_Priority">The priority in the retargeting queue</param>
		/// <param name="_Force">Force the retargeting even if we exceed memory.
		/// /!\ Be really cautious with this parameter as we keep a stable memory management.
		/// It is only use to for the emote played by the local player.
		/// </param>
		/// <param name="_AwaitAll">If true, wait for the entire animation to load</param>
		/// <returns>The KinetixClip for the specific Avatar</returns>
		public async Task<TResponseType> GetRetargetedClipByAvatar<TResponseType, THandler>(KinetixEmote _Emote, KinetixAvatar _Avatar, SequencerPriority _Priority, bool _Force, string _ExtensionForced = "", bool _AwaitAll = false)
			where THandler
			: ARetargetExport<TResponseType>, new()
		{
			//------------------------//
			// Check storage          //
			//------------------------//
			if (!_Force && serviceLocator.Get<MemoryService>().HasStorageExceedMemoryLimit())
				throw new Exception("Not enough storage space available to retarget : " + _Emote.Ids.UUID);

			if (!_Force && serviceLocator.Get<MemoryService>().HasRAMExceedMemoryLimit())
				throw new Exception("Not enough RAM space to retarget : " + _Emote.Ids.UUID);

			//------------------------//
			// Avatar pair            //
			//------------------------//
			KinetixEmoteAvatarPair pair             = new KinetixEmoteAvatarPair() { Emote = _Emote, Avatar = _Avatar };
			EmoteRetargetingClipResult<TResponseType> castedClipResult = null;

			//--------------------------------//
			// Is retargeting / Is retargeted //
			//--------------------------------//
			if (retargetedEmoteByAvatar.ContainsKey(pair) && retargetedEmoteByAvatar[pair].clipsByType.ContainsKey(typeof(TResponseType)))
			{
				castedClipResult = (EmoteRetargetingClipResult<TResponseType>)retargetedEmoteByAvatar[pair].clipsByType[typeof(TResponseType)];

				await castedClipResult.Task.Task;

				return castedClipResult.Clip;
			}

			//===============================================================//
			// Cancel token                                                  //
			//===============================================================//
			// In addition to the CancellationTokenSource of the operation   //
			// we add a SequencerCancel									     //
			// used by the retargeting system itself.					     //
			//---------------------------------------------------------------//
			SequencerCancel         sequencerCancelToken      = new SequencerCancel();
			CancellationTokenSource cancellationTokenDownload = new CancellationTokenSource();

			//===============================================================//
			// Store task / result                                           //
			//===============================================================//
			// Adding a new retarget data in the dictionary if it wasn't.    //
			//---------------------------------------------------------------//
			if (!retargetedEmoteByAvatar.ContainsKey(pair))
			{
				retargetedEmoteByAvatar.Add(pair, new EmoteRetargetedData()
				{
					CancellationTokenFileDownload = cancellationTokenDownload,
					SequencerCancelToken          = sequencerCancelToken
				});
			}

			// Adding a result info and a TCS for this process
			// Note: this 'if' might be useless since 'clipsByType[typeof(TResponseType)]' can only be defined if there is an ongoing progess / an emote done
			if (!retargetedEmoteByAvatar[pair].clipsByType.ContainsKey(typeof(TResponseType)))
			{
				TaskCompletionSource<EmoteRetargetingResponse<TResponseType>> tcsObj = new TaskCompletionSource<EmoteRetargetingResponse<TResponseType>>();
				retargetedEmoteByAvatar[pair].clipsByType[typeof(TResponseType)] = new EmoteRetargetingClipResult<TResponseType>(EProgressStatus.PENDING, tcsObj);
			}

			//===============================================================//
			// Seamless Get File (and retarget method)                       //
			//===============================================================//

			RuntimeRetargetFrameIndexer indexer;
			bool rt3k;
			try
			{
				AAnimLoader[] loaders = null;
				bool isForced = _ExtensionForced != string.Empty;
				
				if (isForced)
				{
					loaders = serviceLocator.Get<LoadAnimService>().GetLoadersForExtension(_ExtensionForced);
				}
				
				(indexer, rt3k) = await serviceLocator.Get<LoadAnimService>().GetFrameIndexer(_Emote, cancellationTokenDownload, _Avatar.AvatarID, loaders, isForced);
				
				if (indexer == null)
					throw new Exception("Couldn't find any download URL");
			}
			catch (OperationCanceledException e)
			{
				_Emote.FilePath = string.Empty;

				if (retargetedEmoteByAvatar.ContainsKey(pair)) //We're in async and another method can remove the emote
					retargetedEmoteByAvatar.Remove(pair);
				throw e;
			}
			catch (Exception e)
			{
				// Independent of the wanted return type, if we can't find the GLB file 
				// we want to empty the cache as it is a global problem with the emote
				_Emote.FilePath = string.Empty;

				if (retargetedEmoteByAvatar.ContainsKey(pair)) //We're in async and another method can remove the emote
					retargetedEmoteByAvatar.Remove(pair);

				throw e;
			}

			// If a previous operation got a result,
			// give that result instead of launching a new op
			if (retargetedEmoteByAvatar[pair].clipsByType.ContainsKey(typeof(TResponseType)))
			{
				castedClipResult = (EmoteRetargetingClipResult<TResponseType>)retargetedEmoteByAvatar[pair].clipsByType[typeof(TResponseType)];

				if (castedClipResult != null && castedClipResult.Clip != null)
				{
					return castedClipResult.Clip;
				}
			}


			try
			{
				// Now we can create the retargeting operation itself, that will smartly handle the retargeting of the emote
				EmoteRetargetingResponse<TResponseType> response = await RequestOperationExecution<TResponseType, THandler>(pair, _Priority, indexer, sequencerCancelToken, rt3k, _AwaitAll);
			
				if (castedClipResult == null && retargetedEmoteByAvatar[pair].clipsByType.ContainsKey(typeof(TResponseType)))
				{
					castedClipResult = (EmoteRetargetingClipResult<TResponseType>)retargetedEmoteByAvatar[pair].clipsByType[typeof(TResponseType)];
				}

				// If the current call is the first to resolve the operation
				if (castedClipResult != null && castedClipResult.Clip == null)
				{
					// Once the operation is finished we cache the result
					castedClipResult.Clip   = response.RetargetedClip;
					castedClipResult.Status = EProgressStatus.COMPLETED;

					retargetedEmoteByAvatar[pair].clipsByType[typeof(TResponseType)] = castedClipResult;
					retargetedEmoteByAvatar[pair].SizeInBytes                        = response.EstimatedClipSize;

					serviceLocator.Get<MemoryService>().AddRamAllocation(response.EstimatedClipSize);
					serviceLocator.Get<MemoryService>().OnFileStopBeingUsed(_Emote.Ids.UUID);
				}

				// And invoke callbacks in case they were awaited by UI before the core was initialized
				NotifyCallbackOnRetargetedByAvatar(_Emote, _Avatar);

				// Qualify the task as done
				return castedClipResult.Clip;
			}
			catch (OperationCanceledException e)
			{
				if (retargetedEmoteByAvatar.ContainsKey(pair) && retargetedEmoteByAvatar[pair].clipsByType.Count == 0)
					retargetedEmoteByAvatar.Remove(pair);

				throw e;
			}
			catch (Exception e)
			{
				if (retargetedEmoteByAvatar.ContainsKey(pair))
					retargetedEmoteByAvatar.Remove(pair);

				throw e;
			}
		}

		/// <summary>
		/// Requests the operation itself
		/// </summary>
		/// <param name="_EmoteAvatarPair"
		/// <param name="_Priority"></param>
		/// <param name="_Indexer"></param>
		/// <param name="_RT3K"></param>
		/// <param name="_CancelToken"></param>
		/// <param name="_AwaitAll">If true, wait for the entire animation to load</param>
		/// <typeparam name="TResponseType"></typeparam>
		/// <typeparam name="THandler"></typeparam>
		/// <returns></returns>
		private async Task<EmoteRetargetingResponse<TResponseType>> RequestOperationExecution<TResponseType, THandler>(KinetixEmoteAvatarPair _EmoteAvatarPair, SequencerPriority _Priority, RuntimeRetargetFrameIndexer _Indexer, SequencerCancel _CancelToken, bool _RT3K, bool _AwaitAll)
			where THandler : ARetargetExport<TResponseType>, new()
		{
			EmoteRetargetingConfig                    emoteRetargetingConfig = new EmoteRetargetingConfig(_EmoteAvatarPair.Emote, _EmoteAvatarPair.Avatar, _Priority, _Indexer, _CancelToken, _RT3K, _AwaitAll);
			EmoteRetargeting<TResponseType, THandler> emoteRetargeting       = new EmoteRetargeting<TResponseType, THandler>(emoteRetargetingConfig);

			retargetedEmoteByAvatar[_EmoteAvatarPair].CancellationTokenFileDownload = emoteRetargeting.CancellationTokenSource;

			EmoteRetargetingResponse<TResponseType> response = await OperationManagerShortcut.Get().RequestExecution(emoteRetargeting);

			EmoteRetargetingClipResult<TResponseType> castedClipResult = (EmoteRetargetingClipResult<TResponseType>)retargetedEmoteByAvatar[_EmoteAvatarPair].clipsByType[typeof(TResponseType)];
			
			castedClipResult.Task.SetResult(response);

			return response;
		}

		#region Memory
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_EmoteAvatarPair"></param>
		public void ClearAvatar(KinetixEmoteAvatarPair _EmoteAvatarPair)
		{
			ClearAvatar(_EmoteAvatarPair.Emote, _EmoteAvatarPair.Avatar);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_Emote"></param>
		/// <param name="_Avatar"></param>
		public void ClearAvatar(KinetixEmote _Emote, KinetixAvatar _Avatar)
		{
			if (_Avatar == null)
				return;

			KinetixEmoteAvatarPair pair = new KinetixEmoteAvatarPair() { Emote = _Emote, Avatar = _Avatar };

			if (!retargetedEmoteByAvatar.ContainsKey(pair))
				return;

			if (OnEmoteRetargetedByAvatar.ContainsKey(pair))
				OnEmoteRetargetedByAvatar.Remove(pair);

            CancellationTokenSource cancelTokenFileDownload = retargetedEmoteByAvatar[pair].CancellationTokenFileDownload;
            SequencerCancel sequencerToken = retargetedEmoteByAvatar[pair].SequencerCancelToken;

			if (cancelTokenFileDownload != null && !cancelTokenFileDownload.IsCancellationRequested)
				cancelTokenFileDownload.Cancel();

			if (sequencerToken != null && !sequencerToken.canceled)
				sequencerToken.Cancel();

			EmoteRetargetedData retargetedData = retargetedEmoteByAvatar[pair];
			foreach (EmoteRetargetingClipResult emoteRetargetingClipResult in retargetedData.clipsByType.Values)
			{
				emoteRetargetingClipResult.Dispose();
			}

			serviceLocator.Get<MemoryService>().RemoveRamAllocation(retargetedData.SizeInBytes);

			KinetixDebug.Log("[UNLOADED] Animation : " + pair.Emote.Ids);

			retargetedEmoteByAvatar.Remove(pair);
			_Emote.FilePath = null;
		}

		public void ClearAllAvatars(KinetixAvatar[] _AvoidAvatars = null)
		{
			List<KinetixAvatar> avoidAvatars = new List<KinetixAvatar>();
			if (_AvoidAvatars != null)
			{
				avoidAvatars = _AvoidAvatars.ToList();
			}

			Dictionary<KinetixEmoteAvatarPair, EmoteRetargetedData> retargetedEmoteByAvatarCopy =
				new Dictionary<KinetixEmoteAvatarPair, EmoteRetargetedData>(retargetedEmoteByAvatar);
			foreach (KeyValuePair<KinetixEmoteAvatarPair, EmoteRetargetedData> kvp in retargetedEmoteByAvatarCopy)
			{
				if (!avoidAvatars.Exists(avatar => avatar.Equals(kvp.Key.Avatar)))
					ClearAvatar(kvp.Key.Emote, kvp.Key.Avatar);
			}
		}

		#endregion
	}
}
