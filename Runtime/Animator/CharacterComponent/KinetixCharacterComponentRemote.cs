// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCharacterComponentRemote.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;
using Kinetix.Internal;
using Kinetix.Internal.Utils;
using Kinetix.Internal.Retargeting;
using System.Collections.Generic;

namespace Kinetix
{
	/// <summary>
	/// A remote character. It recieves poses from the network and apply them to the character.
	/// </summary>
	public class KinetixCharacterComponentRemote : KinetixCharacterComponent
	{
		const float OUTER_BLEND_DURATION = 0.35f;

		public SimulationSampler sampler;

		///<inheritdoc/>
		public override void Init(ServiceLocator _ServiceLocator, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			base.Init(_ServiceLocator, _KinetixAvatar, _RootMotionConfig);

			networkSampler.OnDeserializeData += NetworkSampler_OnDeserializeData;
		
			sampler = new SimulationSampler();
			sampler.OnQueueStart              += Sampler_OnQueueStart             ;
			sampler.OnQueueStop               += Sampler_OnQueueStop              ;
			sampler.OnAnimationStart          += Sampler_OnAnimationStart         ;
			sampler.OnAnimationStop           += Sampler_OnAnimationStop          ;
			sampler.OnPlayedFrame             += Sampler_OnPlayedFrame            ;
			sampler.RequestAdaptToInterpreter += Sampler_RequestAdaptToInterpreter;
			sampler.RequestAvatarPos          += Sampler_RequestAvatarPos         ;
			sampler.RequestAvatar             += Sampler_RequestAvatar            ;

			sampler.Effect.RegisterEffect(new SmoothFrameEffect());
			sampler.Effect.RegisterEffect(new ClipToClipBlendEffect());
			sampler.Effect.RegisterEffect(new AnimatorBlendEffect(OUTER_BLEND_DURATION));
			sampler.Effect.RegisterEffect(new RootMotionEffect(_RootMotionConfig));
		}

		#region Sampler play / stop
		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(string _AnimationIds) => PlayAnimation(_AnimationIds, AnimationTimeRange.Default);

		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(string _AnimationIds, AnimationTimeRange _TimeRange) => PlayAnimation(new AnimationIds(_AnimationIds), _TimeRange);

		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(string _AnimationIds, AnimationTimeRange _TimeRange, string _ForcedExtension = "") => PlayAnimation(new AnimationIds(_AnimationIds), _TimeRange, _ForcedExtension);

		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public async void PlayAnimation(AnimationIds _AnimationIds, AnimationTimeRange _TimeRange, string _ForcedExtension = "")
		{
			if (_AnimationIds?.UUID == null)
			{
				KinetixDebug.LogWarning("Animation ID cannot be null when Play Animation");
				return;
			}

			KinetixEmote emote = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetEmote(_AnimationIds);
			KinetixClip clip = await KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(emote, kinetixAvatar, SequencerPriority.VeryHigh, true, _ForcedExtension);

			if (clip == null)
			{
				KinetixDebug.LogWarning("Can't get the animation " + _AnimationIds.UUID);
				return;
			}

			sampler.Play(true, new KinetixClipWrapper(clip, _AnimationIds)).TimeRange = _TimeRange;
		}

		/// <summary>
		/// Play a sequence of animation on the current player
		/// </summary>
		/// <param name="_AnimationIdsArray">IDs of each animation in the play order</param>
		public async void PlayAnimationQueue(AnimationIds[] _AnimationIdsArray)
		{
			int loaded = 0;
			int animeCount = _AnimationIdsArray.Length;
			KinetixClipWrapper[] clips = new KinetixClipWrapper[animeCount];

			for (int i = 0; i < animeCount; i++)
			{
				var _AnimationIds = _AnimationIdsArray[i];

				KinetixEmote emote = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetEmote(_AnimationIds);
				KinetixClip clip = await KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(emote, kinetixAvatar, SequencerPriority.VeryHigh, true);

				if (clip == null)
				{
					KinetixDebug.LogWarning("Can't get the animation " + _AnimationIds.UUID);
					continue;
				}

				clips[i] = new KinetixClipWrapper(clip, _AnimationIds);
				loaded++;

				if (loaded == animeCount)
				{
					sampler.PlayClips(true, clips);
				}
			}
		}

		public void SetPause(bool _Paused)
		{
			if (_Paused)
				sampler.Pause();
			else
				sampler.Resume();
		}

		public void SetPlayRate(float _PlayRate) => sampler.SetPlayRate(_PlayRate);
		public float GetPlayRate() => sampler.GetPlayRate();

		public void SetElapsedTime(float _ElapsedTime) => sampler.ElapsedTime = _ElapsedTime;
		public float GetElapsedTime() => sampler.ElapsedTime;

		public void SetLoopAnimation(bool _Looping) => sampler.AnimationLoopMode = _Looping ? AnimationLoopMode.Loop : AnimationLoopMode.Default;
		public bool GetIsLoopingAnimation() => sampler.AnimationLoopMode != AnimationLoopMode.Default;

		private void StopAnimation()
		{
			sampler.SoftStop(OUTER_BLEND_DURATION);
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			sampler?.Update();
		}

		#endregion

		#region Old Network
		private void NetworkSampler_OnDeserializeData(KinetixNetworkData obj)
		{
			switch (obj.action)
			{
				case KinetixNetworkData.Action.Start:
					PlayAnimation(obj.id, AnimationTimeRange.Default);
					break;
				case KinetixNetworkData.Action.Stop:
					StopAnimation();
					break;
				case KinetixNetworkData.Action.MoveToTime:
					break;
			}
		}

		/// <summary>
		/// Apply a pose from the network on the current avatar.<br/>
		/// If <see cref="KinetixCharacterComponent.AutoPlay"/> is false, it shall be handled via the events
		/// </summary>
		/// <param name="data">The network pose of the animation</param>
		public void ApplySerializedData(byte[] data) => ApplySerializedData(new KinetixNetworkDataRaw(data));

		/// <summary>
		/// Apply a pose from the network on the current avatar.<br/>
		/// If <see cref="KinetixCharacterComponent.AutoPlay"/> is false, it shall be handled via the events
		/// </summary>
		/// <param name="data">The network pose of the animation</param>
		public void ApplySerializedData(KinetixNetworkDataRaw data) => networkSampler.RecieveData(data);
		#endregion

		#region Sampler Event
		private void Sampler_RequestAdaptToInterpreter(KinetixFrame obj)
		{
			obj.AdaptToInterpreter(poseInterpretor[0]);
		}

		private void Sampler_OnPlayedFrame(KinetixFrame obj)
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					obj.Sample(poseInterpretor[i]);
				}
			}

			Call_OnPlayedFrame();
		}

		private void Sampler_OnAnimationStart(KinetixClipWrapper _Clip)
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.AnimationStart(_Clip);
				}
			}

			Call_OnAnimationStart(_Clip.animationIds);
		}

		private void Sampler_OnAnimationStop(KinetixClipWrapper _Clip)
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.AnimationEnd(_Clip);
				}
			}

			Call_OnAnimationEnd(_Clip.animationIds);
		}

		private void Sampler_OnQueueStart()
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.QueueStart();
				}
			}
		}

		private void Sampler_OnQueueStop()
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.QueueEnd();
				}
			}
		}

		private SkeletonPool.PoolItem Sampler_RequestAvatar()
			=> RetargetTableCache.GetTableSync(kinetixAvatar.Avatar).GetClone();

		private KinetixPose Sampler_RequestAvatarPos()
		{
			if (poseInterpretor.Count == 0)
			{
				return new KinetixPose(new List<TransformData>(), new List<HumanBodyBones>(), null, default, default);
			}

			return poseInterpretor[0].GetPose();
		}
		#endregion

		public override void Dispose()
		{
			base.Dispose();

			networkSampler.OnDeserializeData -= NetworkSampler_OnDeserializeData;

			sampler.OnQueueStart              -= Sampler_OnQueueStart             ;
			sampler.OnQueueStop               -= Sampler_OnQueueStop              ;
			sampler.OnAnimationStart          -= Sampler_OnAnimationStart         ;
			sampler.OnAnimationStop           -= Sampler_OnAnimationStop          ;
			sampler.OnPlayedFrame             -= Sampler_OnPlayedFrame            ;
			sampler.RequestAdaptToInterpreter -= Sampler_RequestAdaptToInterpreter;
			sampler.RequestAvatarPos          -= Sampler_RequestAvatarPos         ;
			sampler.RequestAvatar             -= Sampler_RequestAvatar            ;

			sampler.Dispose();
		}

	}
}
