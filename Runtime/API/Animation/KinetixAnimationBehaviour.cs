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
        public static void RegisterLocalPlayerAnimator(Animator _Animator)
        {
            RegisterLocalPlayerAnimator(_Animator, null);
        }

        /// <summary>
        /// Register the Local Player Animator
        /// </summary>
        /// <param name="_Animator">Animator of the Local Player</param>
        /// <param name="_Config">Configuration of the root motion</param>
        public static void RegisterLocalPlayerAnimator(Animator _Animator, RootMotionConfig _Config)
        {
            if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
                KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
            {
                KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
                return;
            }
            
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Config, true);
            
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
        }

        /// <summary>
        /// Register the Local Player with a custom hierarchy
        /// </summary>
        /// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
        /// <param name="_RootTransform">The root GameObject of your avatar</param>
        /// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
        public static void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
        {
            RegisterLocalPlayerCustom(_Root, _RootTransform, _PoseInterpreter, null);
        }

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
            if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
                KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
            {
                KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
                return;
            }
            
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, _Config, true);
            
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
        }

        /// <summary>
        /// Register the Local Player Avatar and Root Transform
        /// </summary>
        /// <param name="_Avatar">Avatar of the Local Player</param>
        /// <param name="_RootTransform">Root Transform of the Local Player</param>
        /// <param name="_ExportType">Type of Export File</param>
        public static void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
        {
            if (KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer != null &&
                KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.KAvatar != null)
            {
                KinetixDebug.LogWarning("A local player was already registered. Please call KinetixCore.Animation.UnregisterLocalPlayer");
                return;
            }
            
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _ExportType, true);
            
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().SetLocalPlayer(playerUUID);
        }

        /// <summary>
        /// Register the Local Player Animator
        /// </summary>
        /// <param name="_Animator">Animator of the Local Player</param>
        public static string RegisterAvatarAnimator(Animator _Animator)
        {
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, false);

            return playerUUID;
        }

        /// <summary>
        /// Register the Local Player Animator
        /// </summary>
        /// <param name="_Animator">Animator of the Local Player</param>
        /// <param name="_Config">Configuration of the root motion</param>
        public static string RegisterAvatarAnimator(Animator _Animator, RootMotionConfig _Config)
        {
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Animator, _Config, false);

            return playerUUID;
        }

        /// <summary>
        /// Register the Local Player with a custom hierarchy
        /// </summary>
        /// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
        /// <param name="_RootTransform">The root GameObject of your avatar</param>
        /// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
        public static string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
        {
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, false);

            return playerUUID;
        }

		/// <summary>
		/// Register the Local Player with a custom hierarchy
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public static string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Root, _RootTransform, _PoseInterpreter, _Config, false);

            return playerUUID;
        }

        /// <summary>
        /// Register the Local Player Avatar and Root Transform
        /// </summary>
        /// <param name="_Avatar">Avatar of the Local Player</param>
        /// <param name="_RootTransform">Root Transform of the Local Player</param>
        /// <param name="_ExportType">Type of Export File</param>
        public static string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
        {
            string playerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().AddPlayerCharacterComponent(_Avatar, _RootTransform, _ExportType, false);

            return playerUUID;
        }

        /// <summary>
        /// Unregister the local player
        /// </summary>
        public static void UnregisterLocalPlayer()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().UnregisterLocalPlayer();
        }

        #endregion

        #region Animation playing

        public static void PlayAnimationOnLocalPlayer(AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.PlayAnimation(_Ids, _OnPlayedAnimation);
        }

        public static void PlayAnimationOnAvatar(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().PlayAnimation(_PlayerUUID, _Ids, _OnPlayedAnimation);
        }
        
        public static void PlayAnimationQueueOnLocalPlayer(AnimationIds[] _Ids, bool _Loop = false, Action<AnimationIds[]> _OnPlayedAnimation = null)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.PlayAnimationQueue(_Ids, _Loop, _OnPlayedAnimation);
        }

        public static void StopAnimationOnLocalPlayer()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.StopAnimation();
        }

        #endregion

        #region Animation loading / unloading / retargeting

        public static void GetRetargetedKinetixClipOnLocalPlayer(AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetRetargetedKinetixClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);
        }
        
        public static void GetRetargetedAnimationClipLegacyOnLocalPlayer(AnimationIds _AnimationIds, Action<AnimationClip> _OnSuccess, Action _OnFailure)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.GetRetargetedAnimationClipLegacy(_AnimationIds, _OnSuccess, _OnFailure);
        }  

        public static void LoadLocalPlayerAnimation(AnimationIds _Ids, string _LockId, Action _OnSuccess)
        {
            string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer?.LoadPlayerAnimation(_Ids, _LockId + "_" + PlayerUUID, _OnSuccess);
        }
        
        public static void LoadLocalPlayerAnimations(AnimationIds[] _Ids, string _LockId, Action _OnSuccess)
        {
            string PlayerUUID = KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.UUID;

            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(PlayerUUID)?.LoadPlayerAnimations(_Ids, _LockId + "_" + PlayerUUID, _OnSuccess);
        }

        public static void LoadAvatarAnimation(string _PlayerUUID, AnimationIds _Ids, string _LockId, Action _OnSuccess)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(_PlayerUUID)?.LoadPlayerAnimation(_Ids, _LockId + "_" + _PlayerUUID, _OnSuccess);
        }
        
        public static void LoadAvatarAnimations(string _PlayerUUID, AnimationIds[] _Ids, string _LockId, Action _OnSuccess)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().GetPlayerManager(_PlayerUUID)?.LoadPlayerAnimations(_Ids, _LockId + "_" + _PlayerUUID, _OnSuccess);
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
