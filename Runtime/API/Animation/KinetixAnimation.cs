// // ----------------------------------------------------------------------------
// // <copyright file="KinetixAnimation.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
	public class KinetixAnimation
	{
		/// <summary>
		/// Event called when local Player is registered
		/// </summary>
		public event Action OnRegisteredLocalPlayer;

		/// <summary>
		/// Event called when a non-local Player is registered
		/// </summary>
		public event Action OnRegisteredCustomPlayer;

		/// <summary>
		/// Event called when animation is played from local Player
		/// </summary>
		public event Action<AnimationIds> OnPlayedAnimationLocalPlayer;
		
		/// <summary>
		/// Event called when animation queue is played from local Player
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
		/// Register the local Player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, _AvatarID);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, null, _Config);
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public void RegisterLocalPlayerAnimator(Animator _Animator, string _AvatarID, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerAnimator(_Animator, _AvatarID, _Config);
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, null, _PoseInterpreter);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, null, _PoseInterpreter, _Config);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, null, _PoseInterpreter);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, null, _PoseInterpreter, _Config);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, null, _ExportType);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, _AvatarID, _PoseInterpreter);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public void RegisterLocalPlayerCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Root, _RootTransform, _AvatarID, _PoseInterpreter, _Config);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, _AvatarID, _PoseInterpreter);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, _AvatarID, _PoseInterpreter, _Config);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register the local Player configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public void RegisterLocalPlayerCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, EExportType _ExportType)
		{
			KinetixAnimationBehaviour.RegisterLocalPlayerCustom(_Avatar, _RootTransform, _AvatarID, _ExportType);
			
			OnRegisteredLocalPlayer?.Invoke();
		}

		/// <summary>
		/// Register a local Character animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		public string RegisterAvatarAnimator(Animator _Animator)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator, null);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public string RegisterAvatarAnimator(Animator _Animator, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator, null, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		public string RegisterAvatarAnimator(Animator _Animator, string _AvatarID)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator, _AvatarID);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character animator with avatar setup to play animation on it 
		/// </summary>
		/// <param name="_Animator">Animator of your local character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_Config">Configuration for the root motion</param>
		public string RegisterAvatarAnimator(Animator _Animator, string _AvatarID, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarAnimator(_Animator, _AvatarID, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, null, _PoseInterpreter);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, null, _PoseInterpreter, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, null, _PoseInterpreter);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, null, _PoseInterpreter, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, EExportType _ExportType)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, null, _ExportType);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, _AvatarID, _PoseInterpreter);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom hierarchy and a pose interpreter
		/// </summary>
		/// <param name="_Root">The root of the skeleton's hierarchy. In T pose</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public string RegisterAvatarCustom(DataBoneTransform _Root, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Root, _RootTransform, _AvatarID, _PoseInterpreter, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, _AvatarID, _PoseInterpreter);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character with a custom pose interpreter
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">The root GameObject of your avatar</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_PoseInterpreter">The interpretor to apply poses to your avatar</param>
		/// <param name="_Config">Configuration of the root motion</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, IPoseInterpreter _PoseInterpreter, RootMotionConfig _Config)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, _AvatarID, _PoseInterpreter, _Config);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}

		/// <summary>
		/// Register a local Character configuration for custom animation system.
		/// </summary>
		/// <param name="_Avatar">Avatar of your character</param>
		/// <param name="_RootTransform">Root Transform of your character</param>
		/// <param name="_AvatarID">ID of the avatar uploaded on your devportal</param>
		/// <param name="_ExportType">The type of file for animations to export</param>
		public string RegisterAvatarCustom(Avatar _Avatar, Transform _RootTransform, string _AvatarID, EExportType _ExportType)
		{
			string playerUUID = KinetixAnimationBehaviour.RegisterAvatarCustom(_Avatar, _RootTransform, _AvatarID, _ExportType);
			
			OnRegisteredCustomPlayer?.Invoke();
			
			return playerUUID;
		}
		
		/// <summary>
		/// Unregister the local Player animator.
		/// </summary>
		public void UnregisterLocalPlayer()
		{
			KinetixAnimationBehaviour.UnregisterLocalPlayer();
		}

		/// <summary>
		/// Unregister a local Character animator.
		/// </summary>
		public void UnregisterAvatar(string _PlayerUUID)
		{
			KinetixAnimationBehaviour.UnregisterAvatar(_PlayerUUID);
		}
		
		/// <summary>
		/// Play animation on local Player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void PlayAnimationOnLocalPlayer(AnimationIds _AnimationIds)
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(_AnimationIds, OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on local Player
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		public void PlayAnimationOnLocalPlayer(string _EmoteID)
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on local Player
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		public void PlayAnimationOnLocalPlayer(string _EmoteID, string _ForceExtension = "")
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer, _ForceExtension);
		}

		/// <summary>
		/// Play animation on a local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_AnimationIds">IDs of the animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void PlayAnimationOnAvatar(string _PlayerUUID, AnimationIds _AnimationIds)
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, _AnimationIds, OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on a local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">ID of the animation</param>
		public void PlayAnimationOnAvatar(string _PlayerUUID, string _EmoteID)
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on a local Character with a specific file format (glb / kinanim)
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_ForceExtension">Type of file to use starting with a '.' character.<br/>Example: ".glb"</param>
		public void PlayAnimationOnAvatar(string _PlayerUUID, string _EmoteID, string _ForceExtension = "")
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), OnPlayedAnimationLocalPlayer, _ForceExtension);
		}

		/// <summary>
		/// Play animation on local Player with a specific time range
		/// </summary>
		/// <param name="_EmoteID">Id of the animation</param>
		/// <param name="_AnimationTimeRange">Range of the animation to be played</param>
		public void PlayAnimationOnLocalPlayer(string _EmoteID, AnimationTimeRange _AnimationTimeRange)
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(new AnimationIds(_EmoteID), _AnimationTimeRange, OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on local Player with a specific time range and a specific file format (glb / kinanim)
		/// </summary>
		/// <param name="_EmoteID">Id of the animation</param>
		/// <param name="_AnimationTimeRange">Range of the animation to be played</param>
		/// <param name="_ForceExtension">Type of file to use starting with a '.' character.<br/>Example: ".glb"</param>
		public void PlayAnimationOnLocalPlayer(string _EmoteID, AnimationTimeRange _AnimationTimeRange, string _ForceExtension = "")
		{
			KinetixAnimationBehaviour.PlayAnimationOnLocalPlayer(new AnimationIds(_EmoteID), _AnimationTimeRange, OnPlayedAnimationLocalPlayer, _ForceExtension);
		}

		/// <summary>
		/// Play animation on a local Avatar with a specific time range
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">Id of the animation</param>
		/// <param name="_AnimationTimeRange">Range of the animation to be played</param>
		public void PlayAnimationOnAvatar(string _PlayerUUID, string _EmoteID, AnimationTimeRange _AnimationTimeRange)
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), _AnimationTimeRange, OnPlayedAnimationLocalPlayer);
		}

		/// <summary>
		/// Play animation on a local Avatar with a specific time range and a specific file format (glb / kinanim)
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">Id of the animation</param>
		/// <param name="_AnimationTimeRange">Range of the animation to be played</param>
		/// <param name="_ForceExtension">Type of file to use starting with a '.' character.<br/>Example: ".glb"</param>
		public void PlayAnimationOnAvatar(string _PlayerUUID, string _EmoteID, AnimationTimeRange _AnimationTimeRange, string _ForceExtension = "")
		{
			KinetixAnimationBehaviour.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), _AnimationTimeRange, OnPlayedAnimationLocalPlayer, _ForceExtension);
		}

		/// <summary>
		/// Set the play rate of the local Player's kinetix animator
		/// </summary>
		/// <param name="_PlayRate">Rate at which the kinetix animator samples the animation</param>
		public void SetPlayRateOnLocalPlayer(float _PlayRate) 
		{
			KinetixAnimationBehaviour.SetPlayRateOnLocalPlayer(_PlayRate);
		}

		/// <summary>
		/// Get the play rate of the local Player's kinetix animator
		/// </summary>
		public void GetPlayRateOnLocalPlayer() 
		{
			KinetixAnimationBehaviour.GetPlayRateOnLocalPlayer();
		}

		/// <summary>
		/// Set the elapsed time of the local Player's kinetix animator
		/// </summary>
		/// <param name="_ElapsedTime">Current time of the kinetix animator</param>
		public void SetElapsedTimeOnLocalPlayer(float _ElapsedTime) 
		{
			KinetixAnimationBehaviour.SetElapsedTimeOnLocalPlayer(_ElapsedTime);
		}

		/// <summary>
		/// Get the current time of the local Player's kinetix animator
		/// </summary>
		public void GetElapsedTimeOnLocalPlayer() 
		{
			KinetixAnimationBehaviour.GetElapsedTimeOnLocalPlayer();
		}

		/// <summary>
		/// Set the play rate of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_PlayRate">Rate at which the kinetix animator samples the animation</param>
		public void SetPlayRateOnAvatar(string _PlayerUUID, float _PlayRate) 
		{
			KinetixAnimationBehaviour.SetPlayRateOnAvatar(_PlayerUUID, _PlayRate);
		}

		/// <summary>
		/// Get the play rate of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		public void GetPlayRateOnAvatar(string _PlayerUUID) 
		{
			KinetixAnimationBehaviour.GetPlayRateOnAvatar(_PlayerUUID);
		}

		/// <summary>
		/// Set the elapsed time of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_ElapsedTime">Current time of the kinetix animator</param>
		public void SetElapsedTimeOnAvatar(string _PlayerUUID, float _ElapsedTime) 
		{
			KinetixAnimationBehaviour.SetElapsedTimeOnAvatar(_PlayerUUID, _ElapsedTime);
		}

		/// <summary>
		/// Get the current time of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		public void GetElapsedTimeOnAvatar(string _PlayerUUID) 
		{
			KinetixAnimationBehaviour.GetElapsedTimeOnAvatar(_PlayerUUID);
		}

		/// <summary>
		/// Set the looping state of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_Looping">If true, the kinetix animator will loop indefinitely</param>
		public void SetLoopAnimationOnAvatar(string _PlayerUUID, bool _Looping)
		{
			KinetixAnimationBehaviour.SetLoopAnimationOnAvatar(_PlayerUUID, _Looping);
		}
		/// <summary>
		/// Set the looping state of the local Player's kinetix animator
		/// </summary>
		/// <param name="_Looping">If true, the kinetix animator will loop indefinitely</param>
		public void SetLoopAnimationOnLocalPlayer(bool _Looping)
		{
			KinetixAnimationBehaviour.SetLoopAnimationOnLocalPlayer(_Looping);
		}
		/// <summary>
		/// Set the looping state of a local Character's kinetix animator
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		public bool GetIsLoopingAnimationOnAvatar(string _PlayerUUID)
		{
			return KinetixAnimationBehaviour.GetIsLoopingAnimationOnAvatar(_PlayerUUID);
		}
		/// <summary>
		/// Get the looping state of the local Player's kinetix animator
		/// </summary>
		public bool GetIsLoopingAnimationOnLocalPlayer()
		{
			return KinetixAnimationBehaviour.GetIsLoopingAnimationOnLocalPlayer();
		}

		/// <summary>
		/// Play animations on local Player
		/// </summary>
		/// <param name="_Ids">IDs of the animations</param>
		/// <param name="_Loop">Loop the queue</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void PlayAnimationQueueOnLocalPlayer(AnimationIds[] _Ids, bool _Loop = false)
		{
			KinetixAnimationBehaviour.PlayAnimationQueueOnLocalPlayer(_Ids, _Loop, OnPlayedAnimationQueueLocalPlayer);
		}

		/// <summary>
		/// Play animations on local Player
		/// </summary>
		/// <param name="_EmoteIDs">ID of the animations</param>
		/// <param name="_Loop">Loop the queue</param>
		public void PlayAnimationQueueOnLocalPlayer(string[] _EmoteIDs, bool _Loop = false)
		{
			KinetixAnimationBehaviour.PlayAnimationQueueOnLocalPlayer(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _Loop, OnPlayedAnimationQueueLocalPlayer);
		}
		
		/// <summary>
		/// Get Retargeted KinetixClip for local Player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void GetRetargetedKinetixClipForLocalPlayer(AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedKinetixClipOnLocalPlayer(_AnimationIds, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Get Retargeted KinetixClip for local Player
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
		public void GetRetargetedKinetixClipForLocalPlayer(string _EmoteID, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedKinetixClipOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Get Retargeted KinetixClip for local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void GetRetargetedKinetixClipForAvatar(string _PlayerUUID, AnimationIds _AnimationIds, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedKinetixClipOnAvatar(_PlayerUUID, _AnimationIds, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Get Retargeted KinetixClip for local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
		public void GetRetargetedKinetixClipForAvatar(string _PlayerUUID, string _EmoteID, Action<KinetixClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedKinetixClipOnAvatar(_PlayerUUID, new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
		}
		
		/// <summary>
		/// Get Retargeted AnimationClip Legacy for local Player
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

		/// <summary>
		/// Get Retargeted AnimationClip Legacy for local Player
		/// </summary>
		/// <remarks>
		/// The animation clip memory isn't unhandeled by the sdk. You have to call the <see cref="UnityEngine.Object.Destroy"/> after using it.
		/// </remarks>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_OnSuccess">Callback on Success providing KinetixClip Legacy</param>
		/// <param name="_OnFailure">Callback on Failure</param>
		public void GetRetargetedAnimationClipLegacyForLocalPlayer(string _EmoteID, Action<AnimationClip> _OnSuccess, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.GetRetargetedAnimationClipLegacyOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Pause or Resume the kinetix animator on local Player
		/// </summary>
		/// <param name="_Paused">If true, pause the kinetix animator. If false, resume the kinetix animator</param>
		public void SetPauseOnLocalPlayer(bool _Paused)
		{
			KinetixAnimationBehaviour.SetPauseOnLocalPlayer(_Paused);
		}

		/// <summary>
		/// Pause or Resume the kinetix animator on a local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_Paused">If true, pause the kinetix animator. If false, resume the kinetix animator</param>
		public void SetPauseOnAvatar(string _PlayerUUID, bool _Paused)
		{
			KinetixAnimationBehaviour.SetPause(_PlayerUUID, _Paused);
		}

		/// <summary>
		/// Stop animation on local Player
		/// </summary>
		public void StopAnimationOnLocalPlayer()
		{
			KinetixAnimationBehaviour.StopAnimationOnLocalPlayer();
		}

		/// <summary>
		/// Stop animation on a local Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		public void StopAnimationOnAvatar(string _PlayerUUID)
		{
			KinetixAnimationBehaviour.StopAnimationOnAvatar(_PlayerUUID);
		}

		/// <summary>
		/// Load a local Player animation
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadLocalPlayerAnimation(AnimationIds _AnimationIds, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimation(_AnimationIds, _LockId, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load a local Player animation
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
		public void LoadLocalPlayerAnimation(string _EmoteID, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimation(new AnimationIds(_EmoteID), _LockId, _OnSuccess, _OnFailure);
		}
		
		/// <summary>
		/// Load local Player animations
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadLocalPlayerAnimations(AnimationIds[] _AnimationIds, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimations(_AnimationIds, _LockId, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load local Player animations
		/// </summary>
		/// <param name="_EmoteIDs">ID of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
		public void LoadLocalPlayerAnimations(string[] _EmoteIDs, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadLocalPlayerAnimations(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load a Character animation
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadAvatarAnimation(string _PlayerUUID, AnimationIds _AnimationIds, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimation(_PlayerUUID, _AnimationIds, _LockId, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load a Character animation
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animation</param>
		public void LoadAvatarAnimation(string _PlayerUUID, string _EmoteID, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimation(_PlayerUUID, new AnimationIds(_EmoteID), _LockId, _OnSuccess, _OnFailure);
		}

		/// <summary>
		/// Load a Character animations
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void LoadAvatarAnimations(string _PlayerUUID, AnimationIds[] _AnimationIds, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimations(_PlayerUUID, _AnimationIds, _LockId, _OnSuccess, _OnFailure);
		}


		/// <summary>
		/// Load a Character animations
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteIDs">ID of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		/// <param name="_OnSuccess">Callback when successfully loaded animations</param>
		public void LoadAvatarAnimations(string _PlayerUUID, string[] _EmoteIDs, string _LockId, Action _OnSuccess = null, Action _OnFailure = null)
		{
			KinetixAnimationBehaviour.LoadAvatarAnimations(_PlayerUUID, _EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId, _OnSuccess, _OnFailure);
		}
		
		/// <summary>
		/// Unload a local Player animation
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void UnloadLocalPlayerAnimation(AnimationIds _AnimationIds, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadLocalPlayerAnimation(_AnimationIds, _LockId);
		}

		/// <summary>
		/// Unload a local Player animation
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		public void UnloadLocalPlayerAnimation(string _EmoteID, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadLocalPlayerAnimation(new AnimationIds(_EmoteID), _LockId);
		}
		
		/// <summary>
		/// Unload local Player animations
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void UnloadLocalPlayerAnimations(AnimationIds[] _AnimationIds, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadLocalPlayerAnimations(_AnimationIds, _LockId);
		}

		/// <summary>
		/// Unload local Player animations
		/// </summary>
		/// <param name="_EmoteIDs">ID of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		public void UnloadLocalPlayerAnimations(string[] _EmoteIDs, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadLocalPlayerAnimations(_EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId);
		}

		/// <summary>
		/// Unload a Character animations
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_AnimationIds">IDs of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void UnloadAvatarAnimations(string _PlayerUUID, AnimationIds[] _AnimationIds, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadAvatarAnimations(_PlayerUUID, _AnimationIds, _LockId);
		}

		/// <summary>
		/// Unload a Character animations
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <param name="_EmoteIDs">ID of the animations</param>
		/// <param name="_LockId">Arbitrary Identifier of the component asking for the animation, to pass when unloading the animation</param>
		public void UnloadAvatarAnimations(string _PlayerUUID, string[] _EmoteIDs, string _LockId)
		{
			KinetixAnimationBehaviour.UnloadAvatarAnimations(_PlayerUUID, _EmoteIDs.Select(t => new AnimationIds(t)).ToArray(), _LockId);
		}

		/// <summary>
		/// Is animation available on local Player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		/// <returns>True if animation available on local Player</returns>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public bool IsAnimationAvailableOnLocalPlayer(AnimationIds _AnimationIds)
		{
			return KinetixAnimationBehaviour.IsAnimationAvailableOnLocalPlayer(_AnimationIds);
		}

		/// <summary>
		/// Is animation available on local Player
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <returns>True if animation available on local Player</returns>
		public bool IsAnimationAvailableOnLocalPlayer(string _EmoteID)
		{
			return KinetixAnimationBehaviour.IsAnimationAvailableOnLocalPlayer(new AnimationIds(_EmoteID));
		}

		/// <summary>
		/// Get notified when an animation is ready on local Player
		/// </summary>
		/// <param name="_Ids">IDs of the animation</param>
		/// <param name="_OnSuccess">Callback on animation ready</param>
		[Obsolete("Please use the overload with the EmoteID as string.", false)]
		public void GetNotifiedOnAnimationReadyOnLocalPlayer(AnimationIds _Ids, Action _OnSuccess)
		{
			KinetixAnimationBehaviour.GetNotifiedOnAnimationReadyOnLocalPlayer(_Ids, _OnSuccess);
		}

		/// <summary>
		/// Get notified when an animation is ready on local Player
		/// </summary>
		/// <param name="_EmoteID">ID of the animation</param>
		/// <param name="_OnSuccess">Callback on animation ready</param>
		public void GetNotifiedOnAnimationReadyOnLocalPlayer(string _EmoteID, Action _OnSuccess)
		{
			KinetixAnimationBehaviour.GetNotifiedOnAnimationReadyOnLocalPlayer(new AnimationIds(_EmoteID), _OnSuccess);
		}

		/// <summary>
		/// Returns the <see cref="KinetixCharacterComponent"/> of the current player
		/// </summary>
		/// <returns><see cref="KinetixCharacterComponentLocal"/></returns>
		public KinetixCharacterComponentLocal GetLocalKCC()
		{
			return KinetixAnimationBehaviour.GetLocalKCC();
		}

		/// <summary>
		/// Returns the <see cref="KinetixCharacterComponent"/> of a Character
		/// </summary>
		/// <param name="_PlayerUUID">UUID of the avatar</param>
		/// <returns><see cref="KinetixCharacterComponentLocal"/></returns>
		public KinetixCharacterComponentLocal GetAvatarKCC(string _PlayerUUID)
		{
			return KinetixAnimationBehaviour.GetAvatarKCC(_PlayerUUID);
		}

		/// <summary>
		/// Get the UUID of every Characters and the local Player
		/// </summary>
		/// <returns>List of UUID</returns>
		public List<string> GetPlayerList()
		{
			return KinetixAnimationBehaviour.GetPlayerList();
		}

		/// <summary>
		/// Check if the local Player has been registered
		/// </summary>
		/// <returns>True if the player has been registered</returns>
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
