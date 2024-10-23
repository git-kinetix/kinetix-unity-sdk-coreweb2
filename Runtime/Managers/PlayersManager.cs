using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Kinetix.Internal.Cache;
using UnityEngine;

[assembly: InternalsVisibleTo("Kinetix.Test.Unit.Runtime")]

namespace Kinetix.Internal
{
	internal class PlayersManager : AKinetixManager
	{
		public Action<AnimationIds> OnAnimationStartOnLocalPlayerAnimator;
		public Action<AnimationIds> OnAnimationEndOnLocalPlayerAnimator;

		public PlayerManager LocalPlayer
		{
			get
			{
				if (localPlayer == null)
					localPlayer = new PlayerManager(serviceLocator, config);

				return localPlayer;
			}
		}

		/// <summary>
		/// Returns the list of player UUIDs
		/// </summary>
		/// <value></value>
		public List<string> Players
		{
			get { return players.Select(x => x.UUID).ToList(); }
		}

		private bool playAutomaticallyOnAnimator;

		private List<PlayerManager>      players;
		private KinetixCoreConfiguration config;
		private PlayerManager            localPlayer;

		public PlayersManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config)
		{
			config  = _Config;
			players = new List<PlayerManager>();
		}

		protected override void Initialize(KinetixCoreConfiguration _Config)
		{
			playAutomaticallyOnAnimator = _Config.PlayAutomaticallyAnimationOnAnimators;
		}


		#region Player Registration

		public string AddPlayerCharacterComponent(Animator _Animator, bool _LocalPlayer = false)
		{
			return AddPlayerCharacterComponent(_Animator, null, _LocalPlayer);
		}

		public string AddPlayerCharacterComponent(Animator _Animator, RootMotionConfig _RootMotionConfig, bool _LocalPlayer)
		{
			return AddPlayerCharacterComponent(_Animator, _Animator.avatar, _RootMotionConfig, _LocalPlayer);
		}


		public string AddPlayerCharacterComponent(Animator _Animator, Avatar _Avatar, RootMotionConfig _RootMotionConfig, bool _LocalPlayer)
		{
			PlayerManager newPlayer = _LocalPlayer ? LocalPlayer : new PlayerManager(serviceLocator, config);
			newPlayer.AddPlayerCharacterComponent(_Animator, _Avatar, _RootMotionConfig);

			if (!players.Contains(newPlayer))
				players.Add(newPlayer);
				
			return newPlayer.UUID;
		}

		public string AddPlayerCharacterComponent(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, bool _LocalPlayer)
		{
			return AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, null, _LocalPlayer);
		}

		public string AddPlayerCharacterComponent(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _RootMotionConfig, bool _LocalPlayer)
		{
			PlayerManager newPlayer = _LocalPlayer ? LocalPlayer : new PlayerManager(serviceLocator, config);
			newPlayer.AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, _RootMotionConfig);
			if (!players.Contains(newPlayer))
				players.Add(newPlayer);
			return newPlayer.UUID;
		}

		public string AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, bool _LocalPlayer)
		{
			return AddPlayerCharacterComponent(_Avatar, _RootTransform, _PoseInterpreter, null, _LocalPlayer);
		}

		public string AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _RootMotionConfig, bool _LocalPlayer)
		{
			PlayerManager newPlayer = _LocalPlayer ? LocalPlayer : new PlayerManager(serviceLocator, config);
			newPlayer.AddPlayerCharacterComponent(_Avatar, _RootTransform, _PoseInterpreter, _RootMotionConfig);
			if (!players.Contains(newPlayer))
				players.Add(newPlayer);
			return newPlayer.UUID;
		}

		public string AddPlayerCharacterComponent(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType, bool _LocalPlayer)
		{
			PlayerManager newPlayer = _LocalPlayer ? LocalPlayer : new PlayerManager(serviceLocator, config);
			newPlayer.AddPlayerCharacterComponent(_Avatar, _RootTransform, _ExportType);
			if (!players.Contains(newPlayer))
				players.Add(newPlayer);
			return newPlayer.UUID;
		}

		/// <summary>
		/// Dispose of the current local player
		/// </summary>
		public void UnregisterLocalPlayer()
		{
			localPlayer.UnregisterPlayerComponent();
			players.Remove(localPlayer);
			localPlayer = null;
		}

		/// <summary>
		/// Dispose of the designated player
		/// </summary>
		public void UnregisterPlayer(int _PlayerIndex)
		{
			PlayerManager manager = GetPlayerManager(_PlayerIndex);

			manager.UnregisterPlayerComponent();
			players.Remove(manager);
		}

		/// <summary>
		/// Dispose of the designated player
		/// </summary>
		public void UnregisterPlayer(string _PlayerUUID)
		{
			PlayerManager manager = GetPlayerManager(_PlayerUUID);

			manager.UnregisterPlayerComponent();
			players.Remove(manager);
		}

		#endregion

		internal PlayerManager GetPlayerManager(int _PlayerIndex)
		{
			return players[_PlayerIndex];
		}

		internal PlayerManager GetPlayerManager(string _PlayerUUID)
		{
			return players.Find((player) => player.UUID == _PlayerUUID);
		}

		#region Frame Controller

		public void PlayAnimation(int _PlayerIndex, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
			=> PlayAnimation(_PlayerIndex, _Ids, AnimationTimeRange.Default, _OnPlayedAnimation);
		public void PlayAnimation(int _PlayerIndex, AnimationIds _Ids, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation)
		{
			if (_PlayerIndex > players.Count)
			{
				KinetixDebug.LogWarning("Asked for a player index superior to players list count.");
				return;
			}

			players[_PlayerIndex].PlayAnimation(_Ids, _AnimationTimeRange, _OnPlayedAnimation);
		}

		public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
			=> PlayAnimation(_PlayerUUID, _Ids, AnimationTimeRange.Default, _OnPlayedAnimation);
		public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);

			player.PlayAnimation(_Ids, _AnimationTimeRange, _OnPlayedAnimation);
		}

		public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension)
			=> PlayAnimation(_PlayerUUID, _Ids, AnimationTimeRange.Default, _OnPlayedAnimation, _ForcedExtension);
		public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);

			player.PlayAnimation(_Ids, _AnimationTimeRange, _OnPlayedAnimation, _ForcedExtension);
		}

		public void StopAnimation(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.StopAnimation();
		}

		public void SetBlendshapeActive(string _PlayerUUID, bool _Active)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetBlendshapeActive(_Active);
		}
		public bool GetBlendshapeActive(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetBlendshapeActive();
        }
        public void SetBlendshapeActiveOnLocalPlayer(bool _Active) => localPlayer.SetBlendshapeActive(_Active);
		public bool GetBlendshapeActiveOnLocalPlayer() => localPlayer.GetBlendshapeActive();

		public void SetPause(string _PlayerUUID, bool _Paused)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetPause(_Paused);
		}

		public void SetPauseOnLocalPlayer(bool _Paused)
		{
			localPlayer.SetPause(_Paused);
		}
		
		public void SetPlayRateOnLocalPlayer(float _PlayRate)
		{
			localPlayer.SetPlayRate(_PlayRate);
		}
		
		public void SetPlayRate(string _PlayerUUID, float _PlayRate) 
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetPlayRate(_PlayRate);
		}
		
		public void GetPlayRateOnLocalPlayer()
		{
			localPlayer.GetPlayRate();
		}
		
		public float GetPlayRate(string _PlayerUUID) 
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetPlayRate();
		}
		
		public void SetElapsedTimeOnLocalPlayer(float _ElapsedTime)
		{
			localPlayer.SetElapsedTime(_ElapsedTime);
		}
		
		public void SetElapsedTime(string _PlayerUUID, float _ElapsedTime) 
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetElapsedTime(_ElapsedTime);
		}
		
		public void GetElapsedTimeOnLocalPlayer()
		{
			localPlayer.GetElapsedTime();
		}
		
		public float GetElapsedTime(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetElapsedTime();
		}
		
		public void SetLoopAnimation(string _PlayerUUID, bool _Looping) 
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetLoopAnimation(_Looping);
		}

		public void SetLoopAnimationOnLocalPlayer(bool _Looping)
		{
			localPlayer.SetLoopAnimation(_Looping);
		}

		public bool GetIsLoopingAnimation(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIsLoopingAnimation();
		}
		
		public bool GetIsLoopingAnimationOnLocalPlayer()
		{
			return localPlayer.GetIsLoopingAnimation();
		}
		#endregion

		//===========================//
		// IK Controller             //
		//===========================//
		#region IK Controller

		/* == == ==  == == == */
		/* == Local Player == */
		/* == == ==  == == == */

		/* Event */
		public void RegisterIkEventOnLocalPlayer(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			localPlayer.RegisterIkEvent(_OnBeforeIkEffect);
		}

		public void UnregisterIkEventOnLocalPlayer(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			localPlayer.UnregisterIkEvent(_OnBeforeIkEffect);
		}

		public void UnregisterAllIkEventsOnLocalPlayer()
		{
			localPlayer.UnregisterAllIkEvents();
		}

		/* GET */
		public Vector3 GetIKHintPositionOnLocalPlayer(AvatarIKHint _Hint)
		{
			return localPlayer.GetIKHintPosition(_Hint);
		}

		public Vector3 GetIKPositionOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return localPlayer.GetIKPosition(_Goal);
		}

		public float GetIKPositionWeightOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return localPlayer.GetIKPositionWeight(_Goal);
		}

		public Quaternion GetIKRotationOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return localPlayer.GetIKRotation(_Goal);
		}

		public float GetIKRotationWeightOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return localPlayer.GetIKRotationWeight(_Goal);
		}

		public bool GetIKAdjustHipsOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return localPlayer.GetIKAdjustHips(_Goal);
		}

		/* SET */
		public void SetIKHintPositionOnLocalPlayer(AvatarIKHint _Hint, Vector3 _Value)
		{
			localPlayer.SetIKHintPosition(_Hint, _Value);
		}

		public void SetIKPositionOnLocalPlayer(AvatarIKGoal _Goal, Vector3 _Value)
		{
			localPlayer.SetIKPosition(_Goal, _Value);
		}

		public void SetIKPositionWeightOnLocalPlayer(AvatarIKGoal _Goal, float _Value)
		{
			localPlayer.SetIKPositionWeight(_Goal, _Value);
		}

		public void SetIKRotationOnLocalPlayer(AvatarIKGoal _Goal, Quaternion _Value)
		{
			localPlayer.SetIKRotation(_Goal, _Value);
		}

		public void SetIKRotationWeightOnLocalPlayer(AvatarIKGoal _Goal, float _Value)
		{
			localPlayer.SetIKRotationWeight(_Goal, _Value);
		}

		public void SetIKAdjustHipsOnLocalPlayer(AvatarIKGoal _Goal, bool _Value)
		{
			localPlayer.SetIKAdjustHips(_Goal, _Value);
		}

		/* == ==  == == */
		/* == Avatar == */
		/* == ==  == == */

		/* Event */
		public void RegisterIkEvent(string _PlayerUUID, IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.RegisterIkEvent(_OnBeforeIkEffect);
		}

		public void UnregisterIkEvent(string _PlayerUUID, IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.UnregisterIkEvent(_OnBeforeIkEffect);
		}

		public void UnregisterAllIkEvents(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.UnregisterAllIkEvents();
		}

		/* GET */
		public Vector3 GetIKHintPosition(string _PlayerUUID, AvatarIKHint _Hint)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKHintPosition(_Hint);
		}

		public Vector3 GetIKPosition(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKPosition(_Goal);
		}

		public float GetIKPositionWeight(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKPositionWeight(_Goal);
		}

		public Quaternion GetIKRotation(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKRotation(_Goal);
		}

		public float GetIKRotationWeight(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKRotationWeight(_Goal);
		}

		public bool GetIKAdjustHips(string _PlayerUUID, AvatarIKGoal _Goal)
		{

			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetIKAdjustHips(_Goal);
		}

		/* SET */
		public void SetIKHintPosition(string _PlayerUUID, AvatarIKHint _Hint, Vector3 _Value)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKHintPosition(_Hint, _Value);
		}

		public void SetIKPosition(string _PlayerUUID, AvatarIKGoal _Goal, Vector3 _Value)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKPosition(_Goal, _Value);
		}

		public void SetIKPositionWeight(string _PlayerUUID, AvatarIKGoal _Goal, float _Value)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKPositionWeight(_Goal, _Value);
		}

		public void SetIKRotation(string _PlayerUUID, AvatarIKGoal _Goal, Quaternion _Value)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKRotation(_Goal, _Value);
		}

		public void SetIKRotationWeight(string _PlayerUUID, AvatarIKGoal _Goal, float _Value)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKRotationWeight(_Goal, _Value);
		}

		public void SetIKAdjustHips(string _PlayerUUID, AvatarIKGoal _Goal, bool _Value)
		{

			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetIKAdjustHips(_Goal, _Value);
		}
		#endregion

		//=====================================//
		// Mask                                //
		//=====================================//
		#region Mask
		public void SetMaskOnLocalPlayer(KinetixMask _Mask) => localPlayer.SetMask(_Mask);
		public KinetixMask GetMaskOnLocalPlayer() => localPlayer.GetMask();

        public void SetMask(string _PlayerUUID, KinetixMask _Mask)
        {
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.SetMask(_Mask);
        }

        public KinetixMask GetMask(string _PlayerUUID)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			return player.GetMask();
        }
        #endregion

        //=====================================//
        // Other                               //
        //=====================================//
        public KinetixCharacterComponentLocal GetLocalKCC()
		{
			return localPlayer?.GetKCC();
		}
		
		public KinetixCharacterComponentLocal GetAvatarKCC(string _PlayerUUID)
		{
			PlayerManager player = players.FirstOrDefault(Linq);
			return 
				player == null ? 
					throw new ArgumentException($"Avatar \"{_PlayerUUID}\" doesn't exist") 
					: 
					player.GetKCC();
			bool Linq(PlayerManager _Player) => _Player.UUID == _PlayerUUID;

		}

		public void GetRetargetedAnimationClipLegacyForAvatar(string _PlayerUUID, AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.GetRetargetedAnimationClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);

		}

		public void GetRetargetedKinetixClipLegacyForAvatar(string _PlayerUUID, AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure)
		{
			PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);
			player.GetRetargetedKinetixClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);

		}

		#region Local Player

		/// <summary>
		/// Sets the local player with a player's index
		/// </summary>
		/// <param name="_PlayerIndex"></param>
		public void SetLocalPlayer(int _PlayerIndex)
		{
			UnregisterLocalPlayerEvents();

			localPlayer = players[_PlayerIndex];

			RegisterLocalPlayerEvents();
		}

		/// <summary>
		/// Sets the local player with a player's UUID
		/// </summary>
		/// <param name="_PlayerUUID"></param>
		public void SetLocalPlayer(string _PlayerUUID)
		{
			UnregisterLocalPlayerEvents();

			localPlayer = players.Find((player) => player.UUID == _PlayerUUID);
			;

			RegisterLocalPlayerEvents();
		}

		#endregion


		#region Events

		private void RegisterLocalPlayerEvents()
		{
			localPlayer.OnAnimationStartOnPlayerAnimator += AnimationStartOnLocalPlayerAnimator;
			localPlayer.OnAnimationEndOnPlayerAnimator   += AnimationEndOnLocalPlayerAnimator;
		}

		private void UnregisterLocalPlayerEvents()
		{
			if (localPlayer != null)
			{
				localPlayer.OnAnimationStartOnPlayerAnimator -= AnimationStartOnLocalPlayerAnimator;
				localPlayer.OnAnimationEndOnPlayerAnimator   -= AnimationEndOnLocalPlayerAnimator;
			}
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
