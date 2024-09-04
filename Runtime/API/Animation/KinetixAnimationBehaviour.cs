// // ----------------------------------------------------------------------------
// // <copyright file="KinetixAnimationBehaviour.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Kinetix.Internal.Cache;
using UnityEngine;

namespace Kinetix.Internal
{
	internal static class KinetixAnimationBehaviour
	{
		#region Registering / Unregistering

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		public static void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID = null)
		{
			RegisterLocalPlayerAnimator(_Animator, _AvatarID, _Animator.avatar, null);
		}
		

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID, Avatar _Avatar, RootMotionConfig _Config)
		{
			if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
				KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
			{
				KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
				return;
			}

			if (_Avatar == null)
				_Avatar = _Animator.avatar;
			
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Avatar, _Config, true);
			
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Avatar">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public static void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			RegisterLocalPlayerCustom(_Avatar, _RootTransform, _AvatarID, _PoseInterpreter, null);
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Avatar">Avatar of the Local Player</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
				KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
			{
				KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
				return;
			}
			
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _PoseInterpreter, _Config, true);
			
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public static void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			RegisterLocalPlayerCustom(_Root, _RootTransform, _AvatarID, _PoseInterpreter, null);
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
				KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
			{
				KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
				return;
			}
			
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, _Config, true);
			
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
		}

		/// <summary>
		/// Register the Local Player Avatar and Root Transform
		/// </summary>
		/// <param name="_Avatar">Avatar of the Local Player</param>
		/// <param name="_RootTransform">Root Transform of the Local Player</param>
		/// <param name="_ExportType">Type of Export File</param>
		public static void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, EExportType _ExportType)
		{
			if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
				KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
			{
				KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
				return;
			}
			
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _ExportType, true);

			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
		}

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		public static string RegisterAvatarAnimator(Animator _Animator, string _AvatarID)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static string RegisterAvatarAnimator(Animator _Animator, string _AvatarID, RootMotionConfig _Config)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Config, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		public static string RegisterAvatarAnimator(Animator _Animator, string _AvatarID, Avatar _Avatar)
		{
			if (_Avatar == null)
				_Avatar = _Animator.avatar;

			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Avatar, null, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player Animator
		/// </summary>
		/// <param name="_Animator">Animator of the Local Player</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static string RegisterAvatarAnimator(Animator _Animator, string _AvatarID, Avatar _Avatar, RootMotionConfig _Config)
		{
			if (_Avatar == null)
				_Avatar = _Animator.avatar;
				
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Avatar, _Config, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public static string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, _Config, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public static string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _PoseInterpreter, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _PoseInterpreter, _Config, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Register the Local Player Avatar and Root Transform
		/// </summary>
		/// <param name="_Avatar">Avatar of the Local Player</param>
		/// <param name="_RootTransform">Root Transform of the Local Player</param>
		/// <param name="_ExportType">Type of Export File</param>
		public static string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, EExportType _ExportType)
		{
			string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _ExportType, false);
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(playerUUID).SetAvatarID(_AvatarID);

			return playerUUID;
		}

		/// <summary>
		/// Unregister the local player
		/// </summary>
		public static void UnregisterLocalPlayer()
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().UnregisterLocalPlayer();
		}

		/// <summary>
		/// Unregister a player
		/// </summary>
		public static void UnregisterAvatar(string _PlayerUUID)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().UnregisterPlayer(_PlayerUUID);
		}

		#endregion

		//===========================//
		// Frame controller          //
		//===========================//
		#region Frame controller
		public static void PlayAnimationOnLocalPlayer(AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.PlayAnimation(_Ids, _OnPlayedAnimation, _ForcedExtension);
		}
		public static void PlayAnimationOnLocalPlayer(AnimationIds _Ids, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.PlayAnimation(_Ids, _AnimationTimeRange, _OnPlayedAnimation, _ForcedExtension);
		}

		public static void PlayAnimationOnAvatar(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().PlayAnimation(_PlayerUUID, _Ids, _OnPlayedAnimation, _ForcedExtension);
		}
		public static void PlayAnimationOnAvatar(string _PlayerUUID, AnimationIds _Ids, AnimationTimeRange _AnimationTimeRange, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension = "")
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().PlayAnimation(_PlayerUUID, _Ids, _AnimationTimeRange, _OnPlayedAnimation, _ForcedExtension);
		}
		
		public static void PlayAnimationQueueOnLocalPlayer(AnimationIds[] _Ids, bool _Loop = false, Action<AnimationIds[]> _OnPlayedAnimation = null)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.PlayAnimationQueue(_Ids, _Loop, _OnPlayedAnimation);
		}

		public static void SetBlendshapeActiveOnAvatar(string _PlayerUUID, bool _Active)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetBlendshapeActive(_PlayerUUID, _Active);
		}
		public static bool GetBlendshapeActiveOnAvatar(string _PlayerUUID)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetBlendshapeActive(_PlayerUUID);
		}
		public static void SetBlendshapeActiveOnLocalPlayer(bool _Active)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetBlendshapeActive(_Active);
		}
        public static bool GetBlendshapeActiveOnLocalPlayer()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetBlendshapeActive();
        }

        public static void SetPauseOnLocalPlayer(bool _Paused)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetPause(_Paused);
		}

		public static void SetPause(string _PlayerUUID, bool _Paused)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetPause(_PlayerUUID, _Paused);
		}

		public static void StopAnimationOnLocalPlayer()
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.StopAnimation();
		}

		public static void StopAnimationOnAvatar(string _PlayerUUID)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().StopAnimation(_PlayerUUID);
		}

		public static void SetLoopAnimationOnAvatar(string _PlayerUUID, bool _Looping)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLoopAnimation(_PlayerUUID, _Looping);
		}
		public static void SetLoopAnimationOnLocalPlayer(bool _Looping)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetLoopAnimation(_Looping);
		}
		public static bool GetIsLoopingAnimationOnAvatar(string _PlayerUUID)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIsLoopingAnimation(_PlayerUUID);
		}
		public static bool GetIsLoopingAnimationOnLocalPlayer()
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIsLoopingAnimation();
		}


		public static void SetPlayRateOnLocalPlayer(float _PlayRate)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetPlayRate(_PlayRate);
		}
		public static void GetPlayRateOnLocalPlayer() 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetPlayRate();
		}
		public static void SetElapsedTimeOnLocalPlayer(float _ElapsedTime) 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetElapsedTime(_ElapsedTime);
		}
		public static void GetElapsedTimeOnLocalPlayer() 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetElapsedTime();
		}
		public static void SetPlayRateOnAvatar(string _PlayerUUID, float _PlayRate)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetPlayRate(_PlayerUUID, _PlayRate);
		}
		public static void GetPlayRateOnAvatar(string _PlayerUUID) 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayRate(_PlayerUUID);
		}
		public static void SetElapsedTimeOnAvatar(string _PlayerUUID, float _ElapsedTime) 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetElapsedTime(_PlayerUUID, _ElapsedTime);
		}
		public static void GetElapsedTimeOnAvatar(string _PlayerUUID) 
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetElapsedTime(_PlayerUUID);
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
		public static void RegisterIkEventOnLocalPlayer(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.RegisterIkEvent(_OnBeforeIkEffect);
		}

		public static void UnregisterIkEventOnLocalPlayer(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UnregisterIkEvent(_OnBeforeIkEffect);
		}

		public static void UnregisterAllIkEventsOnLocalPlayer()
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UnregisterAllIkEvents();
		}

		/* GET */
		public static Vector3 GetIKHintPositionOnLocalPlayer(AvatarIKHint _Hint)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKHintPosition(_Hint);
		}

		public static Vector3 GetIKPositionOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKPosition(_Goal);
		}

		public static float GetIKPositionWeightOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKPositionWeight(_Goal);
		}

		public static Quaternion GetIKRotationOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKRotation(_Goal);
		}

		public static float GetIKRotationWeightOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKRotationWeight(_Goal);
		}

		public static bool GetIKAdjustHipsOnLocalPlayer(AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetIKAdjustHips(_Goal);
		}

		/* SET */
		public static void SetIKHintPositionOnLocalPlayer(AvatarIKHint _Hint, Vector3 _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKHintPosition(_Hint, _Value);
		}

		public static void SetIKPositionOnLocalPlayer(AvatarIKGoal _Goal, Vector3 _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKPosition(_Goal, _Value);
		}

		public static void SetIKPositionWeightOnLocalPlayer(AvatarIKGoal _Goal, float _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKPositionWeight(_Goal, _Value);
		}

		public static void SetIKRotationOnLocalPlayer(AvatarIKGoal _Goal, Quaternion _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKRotation(_Goal, _Value);
		}

		public static void SetIKRotationWeightOnLocalPlayer(AvatarIKGoal _Goal, float _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKRotationWeight(_Goal, _Value);
		}

		public static void SetIKAdjustHipsOnLocalPlayer(AvatarIKGoal _Goal, bool _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetIKAdjustHips(_Goal, _Value);
		}


		/* == ==  == == */
		/* == Avatar == */
		/* == ==  == == */

		/* Event */
		public static void RegisterIkEventOnAvatar(string _PlayerUUID, IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
        {
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().RegisterIkEvent(_PlayerUUID, _OnBeforeIkEffect);
		}

		public static void UnregisterIkEventOnAvatar(string _PlayerUUID, IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect)
        {
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().UnregisterIkEvent(_PlayerUUID, _OnBeforeIkEffect);
        }
        public static void UnregisterAllIkEventsOnAvatar(string _PlayerUUID)
        {
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().UnregisterAllIkEvents(_PlayerUUID);
		}

        /* GET */
		public static Vector3 GetIKHintPositionOnAvatar(string _PlayerUUID, AvatarIKHint _Hint)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKHintPosition(_PlayerUUID, _Hint);
        }

		public static Vector3 GetIKPositionOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKPosition(_PlayerUUID, _Goal);
        }

		public static float GetIKPositionWeightOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKPositionWeight(_PlayerUUID, _Goal);
        }

		public static Quaternion GetIKRotationOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKRotation(_PlayerUUID, _Goal);
        }

		public static float GetIKRotationWeightOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKRotationWeight(_PlayerUUID, _Goal);
        }

		public static bool GetIKAdjustHipsOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetIKAdjustHips(_PlayerUUID, _Goal);
		}

		/* SET */
		public static void SetIKHintPositionOnAvatar(string _PlayerUUID, AvatarIKHint _Hint, Vector3 _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKHintPosition(_PlayerUUID, _Hint, _Value);
        }

		public static void SetIKPositionOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal, Vector3 _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKPosition(_PlayerUUID, _Goal, _Value);
        }

		public static void SetIKPositionWeightOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal, float _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKPositionWeight(_PlayerUUID, _Goal, _Value);
        }

		public static void SetIKRotationOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal, Quaternion _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKRotation(_PlayerUUID, _Goal, _Value);
        }

		public static void SetIKRotationWeightOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal, float _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKRotationWeight(_PlayerUUID, _Goal, _Value);
		}

		public static void SetIKAdjustHipsOnAvatar(string _PlayerUUID, AvatarIKGoal _Goal, bool _Value)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetIKAdjustHips(_PlayerUUID, _Goal, _Value);
		}

		#endregion

		//=====================================//
		// Mask                                //
		//=====================================//
		#region Mask
		public static void SetMaskOnLocalPlayer(KinetixMask _Mask)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.SetMask(_Mask);
        }

        public static KinetixMask GetMaskOnLocalPlayer()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetMask();
        }

        public static void SetMaskOnAvatar(string _PlayerUUID, KinetixMask _Mask)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetMask(_PlayerUUID, _Mask);
		}

		public static KinetixMask GetMaskOnAvatar(string _PlayerUUID)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetMask(_PlayerUUID);
		}
		#endregion

		//=====================================//
		// Anim load / Unload / Retarget       //
		//=====================================//
		#region Animation loading / unloading / retargeting

		public static void GetRetargetedKinetixClipOnLocalPlayer(AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetRetargetedKinetixClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);
		}
		
		public static void GetRetargetedAnimationClipLegacyOnLocalPlayer(AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetRetargetedAnimationClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);
		}
		
		public static void GetRetargetedAnimationClipLegacyOnAvatar(string _PlayerUUID, AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetRetargetedAnimationClipLegacyForAvatar(_PlayerUUID, _AnimationIds, _OnSuccess, _OnFailure);
		}


		public static void GetRetargetedKinetixClipOnAvatar(string _PlayerUUID, AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetRetargetedKinetixClipLegacyForAvatar(_PlayerUUID, _AnimationIds, _OnSuccess, _OnFailure);
		}

		public static void LoadLocalPlayerAnimation(AnimationIds _Ids, string _LockId, Action _OnSuccess, Action _OnFailure)
		{
			string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer?.LoadPlayerAnimation(_Ids, _LockId + "_" + PlayerUUID, _OnSuccess, _OnFailure);
		}
		
		public static void LoadLocalPlayerAnimations(AnimationIds[] _Ids, string _LockId, Action _OnSuccess, Action _OnFailure)
		{
			string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(PlayerUUID)?.LoadPlayerAnimations(_Ids, _LockId + "_" + PlayerUUID, _OnSuccess, _OnFailure);
		}

		public static void LoadAvatarAnimation(string _PlayerUUID, AnimationIds _Ids, string _LockId, Action _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(_PlayerUUID)?.LoadPlayerAnimation(_Ids, _LockId + "_" + _PlayerUUID, _OnSuccess, _OnFailure);
		}
		
		public static void LoadAvatarAnimations(string _PlayerUUID, AnimationIds[] _Ids, string _LockId, Action _OnSuccess, Action _OnFailure)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(_PlayerUUID)?.LoadPlayerAnimations(_Ids, _LockId + "_" + _PlayerUUID, _OnSuccess, _OnFailure);
		}
		
		public static void UnloadLocalPlayerAnimation(AnimationIds _Ids, string _LockId)
		{
			string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer?.UnloadPlayerAnimation(_Ids, _LockId + "_" + PlayerUUID);
		}
		
		public static void UnloadLocalPlayerAnimations(AnimationIds[] _Ids, string _LockId)
		{
			string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer?.UnloadPlayerAnimations(_Ids, _LockId + "_" + PlayerUUID);
		}

		public static void UnloadAvatarAnimations(string _PlayerUUID, AnimationIds[] _Ids, string _LockId)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(_PlayerUUID)?.UnloadPlayerAnimations(_Ids, _LockId + "_" + _PlayerUUID);
		}

		#endregion


		//=====================================//
		// Other                               //
		//=====================================//
		public static bool IsAnimationAvailableOnLocalPlayer(AnimationIds _Ids)
		{
			if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer == null)
				return false;

			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.IsAnimationAvailable(_Ids);
		}

		public static void GetNotifiedOnAnimationReadyOnLocalPlayer(AnimationIds _Ids, Action _OnSucceed)
		{
			KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer?.GetNotifiedOnAnimationReadyOnPlayer(_Ids, _OnSucceed);
		}

		public static KinetixCharacterComponentLocal GetLocalKCC()
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetLocalKCC();
		}

		public static KinetixCharacterComponentLocal GetAvatarKCC(string _PlayerUUID)
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetAvatarKCC(_PlayerUUID);
		}

		public static List<string> GetPlayerList()
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().Players;
		}

		public static bool IsLocalPlayerRegistered()
		{
			return KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.IsLocalPlayerRegistered();
		}

    }
}
