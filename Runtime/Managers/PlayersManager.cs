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
            PlayerManager newPlayer = _LocalPlayer ? LocalPlayer : new PlayerManager(serviceLocator, config);
            newPlayer.AddPlayerCharacterComponent(_Animator, _RootMotionConfig);
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

        public void PlayAnimation(int _PlayerIndex, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
        {
            if (_PlayerIndex > players.Count)
            {
                KinetixDebug.LogWarning("Asked for a player index superior to players list count.");
                return;
            }

            players[_PlayerIndex].PlayAnimation(_Ids, _OnPlayedAnimation);
        }

        public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation)
        {
            PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);

            player.PlayAnimation(_Ids, _OnPlayedAnimation);
        }

        public void PlayAnimation(string _PlayerUUID, AnimationIds _Ids, Action<AnimationIds> _OnPlayedAnimation, string _ForcedExtension)
        {
            PlayerManager player = players.Find((player) => player.UUID == _PlayerUUID);

            player.PlayAnimation(_Ids, _OnPlayedAnimation, _ForcedExtension);
        }

        public KinetixCharacterComponentLocal GetLocalKCC()
        {
            return localPlayer?.GetKCC();
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
