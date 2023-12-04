// // ----------------------------------------------------------------------------
// // <copyright file="KinetixAnimation.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Kinetix.Internal.Cache;
using UnityEngine;

namespace Kinetix.Internal
{
	public class KinetixAnimation
	{
		/// <summary>
		/// Event called when Local player is registered
		/// </summary>
		public event Action OnRegisteredLocalPlayer;

		/// <summary>
		/// Event called when a non-local player is registered
		/// </summary>
		public event Action OnRegisteredCustomPlayer;

		/// <summary>
		/// Event called when animation is played from local player
		/// </summary>
		public event Action<AnimationIds> OnPlayedAnimationLocalPlayer;
		
		/// <summary>
		/// Event called when animation queue is played from local player
		/// </summary>
		public event Action<AnimationIds[]> OnPlayedAnimationQueueLocalPlayer;
		
		/// <summary>
		/// Event called when animation start on animator
		/// </summary>
		/// <param>UUID of the animation</param>
		public event Action<AnimationIds> OnAnimationStartOnLocalPlayerAnimator;

		/// <summary>
		/// Event called when animation end on animator
		/// </summary>
		/// <param>UUID of the animation</param>
		public event Action<AnimationIds> OnAnimationEndOnLocalPlayerAnimator;
		
		/// <summary>
		/// Register the local player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator);
			
			OnRegisteredLocalPlayer?.Invoke();
		}
        
        /// <summary>
        /// Register the local player animator with avatar setup to play animation on it 
        /// </summary>
        /// <param name="_Animator">Animator of your local character</param>
        public void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID)
        {
            KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, _AvatarID);
			
            OnRegisteredLocalPlayer?.Invoke();
        }

		/// <summary>
		/// Register the local player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, null, _Config);
            OnRegisteredLocalPlayer?.Invoke();
        }
        
        /// <summary>
        /// Register the local player animator with avatar setup to play animation on it 
        /// </summary>
        /// <param name="_Animator">Animator of your local character</param>
        /// <param name="_Config">Configuration for the root motion</param>
        public void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID, RootMotionConfig _Config)
        {
            KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, _AvatarID, _Config);
            OnRegisteredLocalPlayer?.Invoke();
        }

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, _PoseInterpreter);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, _PoseInterpreter, _Config);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local player configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, _ExportType);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		public string RegisterAvatarAnimator(Animator _Animator)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register the local player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public string RegisterAvatarAnimator(Animator _Animator, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, _PoseInterpreter);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, _PoseInterpreter, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register the local player configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, _ExportType);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}
		
		/// <summary>
		/// Unregister the local player animator.
		/// </summary>
		public void UnregisterLocalPlayer()
		{
			KinetixAnimationBehaviour.UnregisterLocalPlayer();
		}

		/// <summary>
		/// Unregister a player animator.
		/// </summary>
		public void UnregisterAvatar(string _PlayerUUID)
		{
			KinetixAnimationBehaviour.UnregisterAvatar(_PlayerUUID);
		}
		
		/// <summary>
		/// Play animation on local player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void PlayAnimationOnLocalPlayer(AnimationIds _AnimationIds)
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(_AnimationIds, OnPlayedAnimationLocalPlayer);
		}
        
        public void PlayAnimationOnLocalPlayer(string _EmoteID)
        {
            KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer);
        }
        
		/// <summary>
		/// Play animation on local player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void PlayAnimationOnAvatar(string _PlayerUUID, AnimationIds _AnimationIds)
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, _AnimationIds, OnPlayedAnimationLocalPlayer);
		}
        
        public void PlayAnimationOnAvatar(string _PlayerUUID, string _EmoteID)
        {
            KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer);
        }
		
		/// <summary>
		/// Play animations on local player
		/// </summary>
		/// <param name="_Ids">IDs of the animations</param>
		/// <param name="_Loop">Loop the queue</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void PlayAnimationQueueOnLocalPlayer(AnimationIds[] _Ids, bool _Loop = false)
		{
			KinetixAnimationBehaviour.PlayAnimationQueueOnLocalPlayer(_Ids, _Loop, OnPlayedAnimationQueueLocalPlayer);
		}
        
        public void PlayAnimationQueueOnLocalPlayer(string[] _EmoteIDs, bool _Loop = false)
        {
            KinetixAnimationBehaviour.PlayAnimationQueueOnLocalPlayer(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _Loop, OnPlayedAnimationQueueLocalPlayer);
        }
		
		/// <summary>
		/// Get Retargeted KinetixClip for local player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void GetRetargetedKinetixClipForLocalPlayer(AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedKinetixClipOnLocalPlayer(_AnimationIds, _OnSuccess, _OnFailure);
		}

        public void GetRetargetedKinetixClipForLocalPlayer(string _EmoteID, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
        {
            KinetixAnimationBehaviour.GetRetargetedKinetixClipOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
        }
        
		/// <summary>
		/// Get Retargeted AnimationClip Legacy for local player
		/// </summary>
		/// <remarks>
		/// The animation clip memory isn't unhandeled by the sdk. You have to call the <see cref="UnityEngine.Object.Destroy"/> after using it.
		/// </remarks>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void GetRetargetedAnimationClipLegacyForLocalPlayer(AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedAnimationClipLegacyOnLocalPlayer(_AnimationIds, _OnSuccess, _OnFailure);
		}
        
        public void GetRetargetedAnimationClipLegacyForLocalPlayer(string _EmoteID, Action<AnimationClip> _OnSuccess, Action _OnFailure = null)
        {
            KinetixAnimationBehaviour.GetRetargetedAnimationClipLegacyOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
        }

		/// <summary>
		/// Stop animation on local player
		/// </summary>
		public void StopAnimationOnLocalPlayer()
		{
			KinetixAnimationBehaviour.StopAnimationOnLocalPlayer();
		}

		/// <summary>
		/// Load a local player animation
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void LoadLocalPlayerAnimation(AnimationIds _AnimationIds, string _LockId, Action _OnSuccess = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimation(_AnimationIds, _LockId, _OnSuccess);
		}
        
        public void LoadLocalPlayerAnimation(string _EmoteID, string _LockId, Action _OnSuccess = null)
        {
            KinetixAnimationBehaviour.LoadLocalPlayerAnimation(new AnimationIds(_EmoteID), _LockId, _OnSuccess);
        }
		
		/// <summary>
		/// Load local player animations
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void LoadLocalPlayerAnimations(AnimationIds[] _AnimationIds, string _LockId, Action _OnSuccess = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimations(_AnimationIds, _LockId, _OnSuccess);
		}
        
        public void LoadLocalPlayerAnimations(string[] _EmoteIDs, string _LockId, Action _OnSuccess = null)
        {
            KinetixAnimationBehaviour.LoadLocalPlayerAnimations(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId, _OnSuccess);
        }

		/// <summary>
		/// Load a player animation
		/// </summary>
		/// <param name="_PlayerUUID">UUID of player</param>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void LoadAvatarAnimation(string _PlayerUUID, AnimationIds _AnimationIds, string _LockId, Action _OnSuccess = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimation(_PlayerUUID, _AnimationIds, _LockId, _OnSuccess);
		}

        public void LoadAvatarAnimation(string _PlayerUUID, string _EmoteID, string _LockId, Action _OnSuccess = null)
        {
            KinetixAnimationBehaviour.LoadAvatarAnimation(_PlayerUUID, new AnimationIds(_EmoteID), _LockId, _OnSuccess);
        }
		
		/// <summary>
		/// Load player animations
		/// </summary>
		/// <param name="_PlayerUUID">UUID of player</param>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void LoadAvatarAnimations(string _PlayerUUID, AnimationIds[] _AnimationIds, string _LockId, Action _OnSuccess = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimations(_PlayerUUID, _AnimationIds, _LockId, _OnSuccess);
		}
        
        public void LoadAvatarAnimations(string _PlayerUUID, string[] _EmoteIDs, string _LockId, Action _OnSuccess = null)
        {
            KinetixAnimationBehaviour.LoadAvatarAnimations(_PlayerUUID, _EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId, _OnSuccess);
        }
		
		/// <summary>
		/// Unload a local player animation
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void UnloadLocalPlayerAnimation(AnimationIds _AnimationIds, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadLocalPlayerAnimation(_AnimationIds, _LockId);
		}
        
        public void UnloadLocalPlayerAnimation(string _EmoteID, string _LockId)
        {
            KinetixAnimationBehaviour.UnloadLocalPlayerAnimation(new AnimationIds(_EmoteID), _LockId);
        }
		
		/// <summary>
		/// Unload local player animations
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void UnloadLocalPlayerAnimations(AnimationIds[] _AnimationIds, string _LockId)
		{
            KinetixAnimationBehaviour.UnloadLocalPlayerAnimations(_AnimationIds, _LockId);
		}
        
        public void UnloadLocalPlayerAnimations(string[] _EmoteIDs, string _LockId)
        {
            KinetixAnimationBehaviour.UnloadLocalPlayerAnimations(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId);
        }

		/// <summary>
		/// Unload player animations
		/// </summary>
		/// <param name="_PlayerUUID">Player UUID</param>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void UnloadAvatarAnimations(string _PlayerUUID, AnimationIds[] _AnimationIds, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadAvatarAnimations(_PlayerUUID, _AnimationIds, _LockId);
		}
        
        public void UnloadAvatarAnimations(string _PlayerUUID, string[] _EmoteIDs, string _LockId)
        {
            KinetixAnimationBehaviour.UnloadAvatarAnimations(_PlayerUUID, _EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId);
        }

		/// <summary>
		/// Is animation available on local player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <returns>True if animation available on local player</returns>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public bool IsAnimationAvailableOnLocalPlayer(AnimationIds _AnimationIds)
		{
			return KinetixAnimationBehaviour.IsAnimationAvailableOnLocalPlayer(_AnimationIds);
		}
        
        public bool IsAnimationAvailableOnLocalPlayer(string _EmoteID)
        {
            return KinetixAnimationBehaviour.IsAnimationAvailableOnLocalPlayer(new AnimationIds(_EmoteID));
        }

		/// <summary>
		/// Get notified when an animation is ready on local player
		/// </summary>
		/// <param name="_Ids">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on animation ready</param>
        [Obsolete("Please use the overload with the EmoteID as string.", false)]
        public void GetNotifiedOnAnimationReadyOnLocalPlayer(AnimationIds _Ids, Action _OnSuccess)
		{
			KinetixAnimationBehaviour.GetNotifiedOnAnimationReadyOnLocalPlayer(_Ids, _OnSuccess);
		}
        
        public void GetNotifiedOnAnimationReadyOnLocalPlayer(string _EmoteID, Action _OnSuccess)
        {
            KinetixAnimationBehaviour.GetNotifiedOnAnimationReadyOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess);
        }

		/// <summary>
		/// Returns the KinetixCharacterComponent
		/// </summary>
		/// <returns></returns>
		public KinetixCharacterComponentLocal GetLocalKCC()
		{
			return KinetixAnimationBehaviour.GetLocalKCC();
		}

		public List<string> GetPlayerList()
		{
			return KinetixAnimationBehaviour.GetPlayerList();
		}
        public bool IsLocalPlayerRegistered()
        {
            return KinetixAnimationBehaviour.IsLocalPlayerRegistered();
        }

		#region Internal

		public KinetixAnimation()
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().OnAnimationStartOnLocalPlayerAnimator += AnimationStartOnLocalPlayerAnimator;
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().OnAnimationEndOnLocalPlayerAnimator   += AnimationEndOnLocalPlayerAnimator;
		}

		private void AnimationStartOnLocalPlayerAnimator(AnimationIds _AnimationIds)
		{
			OnAnimationStartOnLocalPlayerAnimator?.Invoke(_AnimationIds);
		}

		private void AnimationEndOnLocalPlayerAnimator(AnimationIds _AnimationIds)
		{
			OnAnimationEndOnLocalPlayerAnimator?.Invoke(_AnimationIds);
		}

		#endregion
	}
}
