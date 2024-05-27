// // ----------------------------------------------------------------------------
// // <copyright file="KinetixPlayerCache.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal.Retargeting;
using Kinetix.Internal.Utils;
using Kinetix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal.Cache
{
	internal class PlayerManager: AKinetixManager
	{
		public Action<AnimationIds> OnAnimationStartOnPlayerAnimator;
		public Action<AnimationIds> OnAnimationEndOnPlayerAnimator;

		//  Avatar
		public KinetixAvatar KAvatar;

		public readonly string UUID;

		// Callback to notify on retarget  avatar
		private Dictionary<AnimationIds, List<Action>> callbackOnRetargetedAnimationIdOnPlayer;

		// Character component to play automatically on animator
		private KinetixCharacterComponentLocal KinetixCharacterComponent;

		// Animation to preload before the  avatar was registered
		private List<AnimationIds> emotesToPreload;

		// Animations Ids downloaded and retargeted on  Avatar
		private List<AnimationIds> downloadedEmotesReadyToPlay;
		
		// Play Automatically on Animator
		private bool playAutomaticallyOnAnimator;

		public PlayerManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config) 
		{
			UUID = Guid.NewGuid().ToString();
		}

		protected override void Initialize(KinetixCoreConfiguration _Config)
		{
			playAutomaticallyOnAnimator = _Config.PlayAutomaticallyAnimationOnAnimators;
			
			callbackOnRetargetedAnimationIdOnPlayer = new Dictionary<AnimationIds, List<Action>>();
			downloadedEmotesReadyToPlay                  = new List<AnimationIds>();
			emotesToPreload                              = new List<AnimationIds>();
		}

		#region Registering

		public void AddPlayerCharacterComponent(Animator _Animator)
		{
			KAvatar            = CreateKinetixAvatar(_Animator.avatar, _Animator.transform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_Animator, KAvatar);
			OnRegisterPlayer();
		}

		public void AddPlayerCharacterComponent(Animator _Animator, RootMotionConfig _RootMotionConfig)
		{
			if (KAvatar != null)
				UnregisterPlayerComponent();

			KAvatar            = CreateKinetixAvatar(_Animator.avatar, _Animator.transform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_Animator, KAvatar, _RootMotionConfig);
			OnRegisterPlayer();
		}

		public void AddPlayerCharacterComponent(DataBoneTransform _Root, Transform _RootTransform,IPoseInterpreter _PoseInterpreter)
		{
			KAvatar            = CreateKinetixAvatar(_Root, _RootTransform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_PoseInterpreter, KAvatar);
			OnRegisterPlayer();
		}

		public void AddPlayerCharacterComponent(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _RootMotionConfig)
		{
			KAvatar            = CreateKinetixAvatar(_Root, _RootTransform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_PoseInterpreter, KAvatar, _RootMotionConfig);
			OnRegisterPlayer();
		}

		public void AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform,IPoseInterpreter _PoseInterpreter)
		{
			KAvatar            = CreateKinetixAvatar(_Avatar, _RootTransform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_PoseInterpreter, KAvatar);
			OnRegisterPlayer();
		}

		public void AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _RootMotionConfig)
		{
			KAvatar            = CreateKinetixAvatar(_Avatar, _RootTransform, EExportType.KinetixClip);
			KinetixCharacterComponent = AddKCCAndInit(_PoseInterpreter, KAvatar, _RootMotionConfig);
			OnRegisterPlayer();
		}
	   
		public void AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			if (KAvatar != null)
				UnregisterPlayerComponent();
			
			KAvatar = CreateKinetixAvatar(_Avatar, _RootTransform, _ExportType);
			OnRegisterPlayer();
		}

		public void SetAvatarID(string _AvatarID)
		{
			if (KAvatar != null)
				KAvatar.AvatarID = _AvatarID;
		}
		
		#endregion

		#region LOAD

		private KinetixAvatar CreateKinetixAvatar(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			return new KinetixAvatar()
			{
				Avatar = new AvatarData(_Avatar, _RootTransform), Root = _RootTransform, ExportType = _ExportType
			};
		}
		private KinetixAvatar CreateKinetixAvatar(DataBoneTransform _Root, Transform _RootTransform, EExportType _ExportType)
		{
			return new KinetixAvatar()
			{
				Avatar = new AvatarData(_Root, _RootTransform), Root = _RootTransform, ExportType = _ExportType
			};
		}
		
		public async void GetRetargetedKinetixClipLegacy(AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure)
		{
			bool awaitAll = KinetixCharacterComponent == null;
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_AnimationIds);

			try
			{
				KinetixClip clip = await serviceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(emote, KAvatar, SequencerPriority.Low, false, _AwaitAll: awaitAll);
				_OnSuccess?.Invoke(clip);
			}
			catch (OperationCanceledException)
			{
				KinetixDebug.Log("Loading animation operation was cancelled for emote : " + emote.Ids.UUID);
				_OnFailure?.Invoke();
			}
			catch (Exception e)
			{
				KinetixDebug.LogWarning($"Failed loading animation with id { emote.Ids.UUID } with error : " + e.Message);
				_OnFailure?.Invoke();
			}		
		}
		
		public async void GetRetargetedAnimationClipLegacy(AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_AnimationIds);

			try
			{
				AnimationClip clip = await serviceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<AnimationClip, AnimationClipExport>(emote, KAvatar, SequencerPriority.Low, false, _AwaitAll: true);
				
				clip.wrapMode = WrapMode.Default;

				_OnSuccess?.Invoke(clip);
			}
			catch (OperationCanceledException)
			{
				KinetixDebug.Log("Loading animation operation was cancelled for emote : " + emote.Ids.UUID);
				_OnFailure?.Invoke();
			}
			catch (Exception e)
			{
				KinetixDebug.LogWarning($"Failed loading animation with id { emote.Ids.UUID } with error : " + e.Message);
				_OnFailure?.Invoke();
			}
		}
		
		public void UnregisterPlayerComponent()
		{
			if (KinetixCharacterComponent != null)
			{
				KinetixCharacterComponent.Dispose();
				KinetixCharacterComponent = null;
			}
			
			ForceUnloadPlayerAnimations(downloadedEmotesReadyToPlay.ToArray());

			emotesToPreload.Clear();
			callbackOnRetargetedAnimationIdOnPlayer.Clear();
			downloadedEmotesReadyToPlay.Clear();
			KAvatar = null;

			KinetixCoreBehaviour.ManagerLocator?.Get<AccountManager>().OnUpdatedAccount?.Invoke();
		}

		public void LoadPlayerAnimation(AnimationIds _Ids, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);
			serviceLocator.Get<LockService>().Lock(new KinetixEmoteAvatarPair() { Emote = emote, Avatar = KAvatar}, _LockId);
			
			LoadPlayerAnimationInternal(emote, _OnSuccess, _OnFailure);
		}

		public void LoadPlayerAnimations(AnimationIds[] _Ids, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			for (int i = 0; i < _Ids.Length; i++)
			{
				LoadPlayerAnimation(_Ids[i], _LockId, _OnSuccess, _OnFailure);
			}
		}

		// To be called only in this class in case emote are preloaded with a lock
		private void LoadPlayerAnimations(AnimationIds[] _Ids, Action _OnSuccess = null, Action _OnFailure = null)
		{
			LoadPlayerAnimations(_Ids, "", _OnSuccess, _OnFailure);
		}

		private async void LoadPlayerAnimationInternal(KinetixEmote _KinetixEmote, Action _OnSuccess, Action _OnFailure)
		{
			if (KAvatar == null)
			{
				if (!emotesToPreload.Contains(_KinetixEmote.Ids))
					emotesToPreload.Add(_KinetixEmote.Ids);

				return;
			}

			downloadedEmotesReadyToPlay ??= new List<AnimationIds>();

			if (!downloadedEmotesReadyToPlay.Contains(_KinetixEmote.Ids))
				downloadedEmotesReadyToPlay.Add(_KinetixEmote.Ids);

			try
			{
				// TODO find a better way, this is not sexy at all
				object retargetingResult;
				
				if (KAvatar.ExportType == EExportType.KinetixClip)
				{
					retargetingResult = await serviceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(_KinetixEmote, KAvatar, SequencerPriority.Low, 
					false);
				}
				else
				{
					retargetingResult = await serviceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<AnimationClip, AnimationClipExport>(_KinetixEmote, KAvatar, SequencerPriority.Low, false);
				}

				if (retargetingResult != null)
				{
					serviceLocator.Get<MemoryService>().OnAnimationLoadedOnPlayer(_KinetixEmote.Ids);
					_OnSuccess?.Invoke();
				}
				else
				{
					_OnFailure?.Invoke();
				}
			}
			catch (OperationCanceledException)
			{
				KinetixDebug.Log("Loading animation operation was cancelled for emote : " + _KinetixEmote.Ids.UUID);
				_OnFailure?.Invoke();
			}
			catch (Exception e)
			{
#if DEV_KINETIX
				Debug.LogException(e);
#endif
				string message = e.Message;
				while (e.InnerException != null)
				{
					e = e.InnerException;
					message += "\r\n";
					message += e.Message;
				}
				KinetixDebug.LogWarning($"Failed loading animation with id { _KinetixEmote.Ids.UUID } with error : " + message);
				_OnFailure?.Invoke();
			}
		}
		#endregion

		#region UNLOAD
		public void UnloadPlayerAnimation(AnimationIds _Ids, string _LockId)
		{
			UnlockPlayerAnimation(_Ids, _LockId);
			RemovePlayerEmotesReadyToPlay(_Ids);
			
			serviceLocator.Get<MemoryService>().OnAnimationUnloadedOnPlayer();
		}

		public void UnloadPlayerAnimations(AnimationIds[] _Ids, string _LockId = "")
		{
			foreach (AnimationIds ids in _Ids)
			{
				UnlockPlayerAnimation(ids, _LockId);
				RemovePlayerEmotesReadyToPlay(ids);

				serviceLocator.Get<MemoryService>().OnAnimationUnloadedOnPlayer(); 
			}
		}

		public void ForceUnloadPlayerAnimations(AnimationIds[] _Ids)
		{
			foreach (AnimationIds ids in _Ids)
			{
				RemovePlayerEmotesReadyToPlay(ids);
				KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(ids);
				serviceLocator.Get<LockService>().ForceUnload(new KinetixEmoteAvatarPair(emote, KAvatar));

				serviceLocator.Get<MemoryService>().OnAnimationUnloadedOnPlayer();
			}
		}

		public void RemovePlayerEmotesToPreload(AnimationIds[] _Ids)
		{
			emotesToPreload ??= new List<AnimationIds>();
			for (int i = 0; i < _Ids.Length; i++)
			{
				if (emotesToPreload.Contains(_Ids[i]))
					emotesToPreload.Remove(_Ids[i]);
			}
		}
		
		private void RemovePlayerEmotesReadyToPlay(AnimationIds _Ids)
		{
			downloadedEmotesReadyToPlay ??= new List<AnimationIds>();
			if (downloadedEmotesReadyToPlay.Contains(_Ids))
				downloadedEmotesReadyToPlay.Remove(_Ids);
		}
		
		#endregion

		#region LOCKS
		
		public void LockPlayerAnimation(AnimationIds _Ids, string _LockId)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);

			serviceLocator.Get<LockService>().Lock(new KinetixEmoteAvatarPair(emote, KAvatar), _LockId);
		}
		
		public void LockPlayerAnimations(AnimationIds[] _Ids, string _LockId)
		{
			foreach (AnimationIds ids in _Ids)
			{
				LockPlayerAnimation(ids, _LockId);
			}
		}

		public void UnlockPlayerAnimation(AnimationIds _Ids, string _LockId)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);

			serviceLocator.Get<LockService>().Unlock(new KinetixEmoteAvatarPair(emote, KAvatar), _LockId);
		}
		
		public void UnlockPlayerAnimations(AnimationIds[] _Ids, string _LockId)
		{
			foreach (AnimationIds ids in _Ids)
			{
				UnlockPlayerAnimation(ids, _LockId);
			}
		}

		public void LockPlayerAnimation(string _PlayerUUID, AnimationIds _Ids, string _LockId)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);

			serviceLocator.Get<LockService>().Lock(new KinetixEmoteAvatarPair(emote, KAvatar), _LockId + "_" + _PlayerUUID);
		}
		
		public void LockPlayerAnimations(string _PlayerUUID, AnimationIds[] _Ids, string _LockId)
		{
			foreach (AnimationIds ids in _Ids)
			{
				LockPlayerAnimation(_PlayerUUID, ids, _LockId);
			}
		}

		public void UnlockPlayerAnimation(string _PlayerUUID, AnimationIds _Ids, string _LockId)
		{
			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);

			serviceLocator.Get<LockService>().Unlock(new KinetixEmoteAvatarPair(emote, KAvatar), _LockId + "_" + _PlayerUUID);
		}
		
		public void UnlockPlayerAnimations(string _PlayerUUID, AnimationIds[] _Ids, string _LockId)
		{
			foreach (AnimationIds ids in _Ids)
			{
				UnlockPlayerAnimation(_PlayerUUID, ids, _LockId + "_" + _PlayerUUID);
			}
		}

		#endregion
		
		public bool IsAnimationAvailable(AnimationIds _Ids)
		{
			return KAvatar != null && serviceLocator.Get<EmotesService>().GetEmote(_Ids).HasAnimationRetargeted(KAvatar);
		}

		public bool IsEmoteUsedByPlayer(AnimationIds _Ids)
		{
			return KAvatar != null && downloadedEmotesReadyToPlay.Contains(_Ids);
		}

		public void GetNotifiedOnAnimationReadyOnPlayer(AnimationIds _Ids, Action _OnSucceed)
		{
			try
			{
				if (KAvatar != null)
				{
					KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_Ids);
					serviceLocator.Get<RetargetingService>().RegisterCallbacksOnRetargetedByAvatar(emote, KAvatar, _OnSucceed);
				}
				else
				{
					if (!callbackOnRetargetedAnimationIdOnPlayer.ContainsKey(_Ids))
						callbackOnRetargetedAnimationIdOnPlayer.Add(_Ids, new List<Action>());
				
					callbackOnRetargetedAnimationIdOnPlayer[_Ids].Add(_OnSucceed);
				}
			}
			catch (Exception e)
			{
				KinetixDebug.LogWarning("Can't get notified on animation ready for  player : " + e.Message);
			}
		}

		private KinetixCharacterComponentLocal AddKCCAndInit(Animator _Animator, KinetixAvatar _KinetixAvatar)
		{
			KinetixCharacterComponentLocal kcc = new KinetixCharacterComponentLocal();

			kcc.RegisterPoseInterpreter(new AnimatorPoseInterpetor(_Animator, _KinetixAvatar.Avatar.avatar, _Animator.GetComponentsInChildren<SkinnedMeshRenderer>().GetARKitRenderers()));
			kcc.AutoPlay = true;

			kcc.Init(serviceLocator, _KinetixAvatar);
			kcc.OnAnimationStart += AnimationStartOnPlayerAnimator;
			kcc.OnAnimationEnd  += AnimationEndOnPlayerAnimator;
			return kcc;
		}

		private KinetixCharacterComponentLocal AddKCCAndInit(Animator _Animator, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			KinetixCharacterComponentLocal kcc = new KinetixCharacterComponentLocal();

			kcc.RegisterPoseInterpreter(new AnimatorPoseInterpetor(_Animator, _KinetixAvatar.Avatar.avatar, _Animator.GetComponentsInChildren<SkinnedMeshRenderer>().GetARKitRenderers()));
			kcc.AutoPlay = true;

			kcc.Init(serviceLocator, _KinetixAvatar, _RootMotionConfig);
			kcc.OnAnimationStart += AnimationStartOnPlayerAnimator;
			kcc.OnAnimationEnd  += AnimationEndOnPlayerAnimator;
			return kcc;
		}

		private KinetixCharacterComponentLocal AddKCCAndInit(IPoseInterpreter _PoseInterpreter, KinetixAvatar _KinetixAvatar)
		{
			KinetixCharacterComponentLocal kcc = new KinetixCharacterComponentLocal();

			kcc.RegisterPoseInterpreter(_PoseInterpreter);
			kcc.AutoPlay = true;

			kcc.Init(serviceLocator, _KinetixAvatar);
			kcc.OnAnimationStart += AnimationStartOnPlayerAnimator;
			kcc.OnAnimationEnd += AnimationEndOnPlayerAnimator;
			return kcc;
		}

		private KinetixCharacterComponentLocal AddKCCAndInit(IPoseInterpreter _PoseInterpreter, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			KinetixCharacterComponentLocal kcc = new KinetixCharacterComponentLocal();

			kcc.RegisterPoseInterpreter(_PoseInterpreter);
			kcc.AutoPlay = true;

			kcc.Init(serviceLocator, _KinetixAvatar, _RootMotionConfig);
			kcc.OnAnimationStart += AnimationStartOnPlayerAnimator;
			kcc.OnAnimationEnd  += AnimationEndOnPlayerAnimator;
			return kcc;
		}

		private void AnimationStartOnPlayerAnimator(AnimationIds ids)
		{
			OnAnimationStartOnPlayerAnimator?.Invoke(ids);
		}

		private void AnimationEndOnPlayerAnimator(AnimationIds ids)
		{
			OnAnimationEndOnPlayerAnimator?.Invoke(ids);
		}

		public AnimationIds[] GetDownloadedAnimationsReadyToPlay()
		{
			return downloadedEmotesReadyToPlay.ToArray();
		}

		public void SetLoopAnimation(bool _Looping) => KinetixCharacterComponent.SetLoopAnimation(_Looping);
		public bool GetIsLoopingAnimation() => KinetixCharacterComponent.GetIsLoopingAnimation();

		public void SetPause(bool _Paused) => KinetixCharacterComponent.SetPause(_Paused);
		public void SetPlayRate(float _PlayRate) => KinetixCharacterComponent.SetPlayRate(_PlayRate);
		public void GetPlayRate() => KinetixCharacterComponent.GetPlayRate();
		public void SetElapsedTime(float _ElapsedTime) => KinetixCharacterComponent.SetElapsedTime(_ElapsedTime);
		public void GetElapsedTime() => KinetixCharacterComponent.GetElapsedTime();

		public void PlayAnimation(AnimationIds _AnimationsIds, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
			=> PlayAnimation(_AnimationsIds, AnimationTimeRange.Default, _OnPlayedAnimation, _ForcedExtension);
		public void PlayAnimation(AnimationIds _AnimationsIds, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
		{
			if (KinetixCharacterComponent == null)
			{
				return;
			}

			if (playAutomaticallyOnAnimator)
				KinetixCharacterComponent.PlayAnimation(_AnimationsIds, _AnimationTimeRange, _ForcedExtension);
			else
				_OnPlayedAnimation?.Invoke(_AnimationsIds);
		}

		public void PlayAnimationQueue(AnimationIds[] _AnimationIdsArray, bool _Loop, Action<AnimationIds[]> _OnPlayedAnimations)
		{
			if (KinetixCharacterComponent == null)
			{
				KinetixDebug.LogWarning(" player was not registered");
				return;
			}

			if (playAutomaticallyOnAnimator)
				KinetixCharacterComponent.PlayAnimationQueue(_AnimationIdsArray);
			else
				_OnPlayedAnimations?.Invoke(_AnimationIdsArray);
		}

		public void StopAnimation()
		{
			if (KinetixCharacterComponent == null)
			{
				KinetixDebug.LogWarning(" player was not registered");
				return;
			}

			if (playAutomaticallyOnAnimator)
				KinetixCharacterComponent.StopAnimation();
		}
		
		private void OnRegisterPlayer()
		{
			foreach (KeyValuePair<AnimationIds, List<Action>> kvp in callbackOnRetargetedAnimationIdOnPlayer)
			{
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					try
					{
						KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(kvp.Key);
					}
					catch (Exception e)
					{
						KinetixDebug.LogWarning("Could not subscribe for retargeting callback: " + e.Message);
					}
				}
			}

			if (emotesToPreload.Count <= 0)
				return;
			
			LoadPlayerAnimations(emotesToPreload.ToArray());
		}


		public KinetixCharacterComponentLocal GetKCC()
		{
			return KinetixCharacterComponent;
		}

		public bool IsLocalPlayerRegistered()
		{
			return KAvatar != null;
		}
	}
}
