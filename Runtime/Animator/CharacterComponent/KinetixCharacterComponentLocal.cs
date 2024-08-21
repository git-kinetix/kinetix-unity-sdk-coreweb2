// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCharacterComponentLocal.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;
using Kinetix.Internal;
using Kinetix.Internal.Utils;
using Kinetix.Internal.Retargeting;
using System.Collections.Generic;

namespace Kinetix
{
	/// <summary>
	/// A local character. It can play animation directly on the avatar.<br/>
	/// It's also provide the <see cref="KinetixNetworkedPose"/> to be sent to the remote peers.
	/// </summary>
	public class KinetixCharacterComponentLocal : KinetixCharacterComponent
	{
		private readonly struct RootMotionContext : IDisposable
		{
			private readonly Transform root;
			private readonly Vector3 originalPos;

			public RootMotionContext(RootMotionEffect effect, Transform root, SimulationSampler simulationSampler)
			{
				this.root = root;
				originalPos = root.position;

				if (simulationSampler.GetIsPlaying())
					//Set root without rootMotion context
					effect.SetRootAtStartPosition(root);
			}

			public void Dispose()
			{
				//Set root back to previous pos
				root.position   = originalPos;
			}
		}

		const float OUTER_BLEND_DURATION = 0.35f;

		/// <summary>
		/// Called when networking data are available
		/// </summary>
		public event Action<KinetixNetworkDataRaw> OnNetworkingData;
		public event Action OnQueueStart;
		public event Action OnQueueStop;

		public SimulationSampler sampler;
		private KinetixFrame currentFrame;
		private KinetixNetworkDataRaw currentNetworkData;

		internal IKEffect ik;
		internal KinetixMaskEffect mask;
		internal RootMotionEffect rootMotion;

		/// <inheritdoc/>
		public override void Init(ServiceLocator _ServiceLocator, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			base.Init(_ServiceLocator, _KinetixAvatar, _RootMotionConfig);
			
			networkSampler.OnSerializeData += NetworkSampler_OnSerializeData;

			sampler = new SimulationSampler();
			sampler.OnQueueStart += Sampler_OnQueueStart;
			sampler.OnQueueStop += Sampler_OnQueueStop;
			sampler.OnAnimationStart += Sampler_OnAnimationStart;
			sampler.OnAnimationStop += Sampler_OnAnimationStop;
			sampler.OnPlayedFrame += Sampler_OnPlayedFrame;
			sampler.RequestAdaptToInterpreter += Sampler_RequestAdaptToInterpreter;
			sampler.RequestAvatarPos += Sampler_RequestAvatarPos;
			sampler.RequestAvatar += Sampler_RequestAvatar;


			sampler.Effect.RegisterEffect(new SmoothFrameEffect());
			sampler.Effect.RegisterEffect(new ClipToClipBlendEffect());
			sampler.Effect.RegisterEffect(new AnimatorBlendEffect(OUTER_BLEND_DURATION));
			sampler.Effect.RegisterEffect(mask = new KinetixMaskEffect());
			sampler.Effect.RegisterEffect(ik   = new IKEffect());
			sampler.Effect.RegisterEffect(rootMotion = new RootMotionEffect(_RootMotionConfig));

			currentFrame = null;
		}

		//=====================================//
		// Mask                                //
		//=====================================//
		public void SetMask(KinetixMask _Mask)
		{
			mask.mask = _Mask;
		}
		
		public KinetixMask GetMask()
		{
			return mask.mask;
		}

		//=====================================//
		// Pose / Data                         //
		//=====================================//
		#region Pose / Data
		/// <summary>
		/// Get the current frame animation if it exists.<br/>
		/// </summary>
		/// <returns>Returns the pose in _OnBeforeIkEffect sampleable format</returns>
		public KinetixFrame GetRawPose() => currentFrame;

		/// <summary>
		/// Get the data in _OnBeforeIkEffect format suitable for the network
		/// </summary>
		public KinetixNetworkDataRaw GetNetworkedData()
		{
			if (!IsDataAvailable())
				return null;

			return currentNetworkData;
		}

		/// <inheritdoc/>
		public bool IsDataAvailable() => currentNetworkData != null;
		#endregion

		//=====================================//
		// Frame controller                    //
		//=====================================//
		#region Frame controller
		public void SetBlendshapeActive(bool _Active)
		{
			sampler.blendshapeEnabled = _Active;
		}
		public bool GetBlendshapeActive()
		{
			return sampler.blendshapeEnabled;
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

		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(AnimationIds _AnimationIds, string _ForcedExtension = "") => PlayAnimation(_AnimationIds, AnimationTimeRange.Default, _ForcedExtension);

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

			KinetixEmote emote = serviceLocator.Get<EmotesService>().GetEmote(_AnimationIds);
			KinetixClip clip = await serviceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(emote, kinetixAvatar, SequencerPriority.VeryHigh, true, _ForcedExtension);

			if (clip == null)
			{
				KinetixDebug.LogWarning("Can't get the animation " + _AnimationIds.UUID);
				return;
			}

			sampler.Play(true, new KinetixClipWrapper(clip, _AnimationIds)).timeRange = _TimeRange;
		}

		public void StopAnimation()
		{
			sampler.SoftStop(OUTER_BLEND_DURATION);
		}

		/// <summary>
		/// Play _OnBeforeIkEffect sequence of animation on the current player
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
		#endregion

		//=====================================//
		// IK controller                       //
		//=====================================//
		#region IK controller
		/* Event */
		public void RegisterIkEvent(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect) => ik.OnBeforeIkEffect += _OnBeforeIkEffect;
		public void UnregisterIkEvent(IKEffect.DelegateBeforeIkEffect _OnBeforeIkEffect) => ik.OnBeforeIkEffect -= _OnBeforeIkEffect;
		public void UnregisterAllIkEvents() => ik.UnregisterAllEvents();

		/* GET */
		public Vector3    GetIKHintPosition(AvatarIKHint _Hint)   {using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); return GetMemberByHint(_Hint).GetPoleVector(kinetixAvatar.Root.gameObject); }
		public Vector3    GetIKPosition(AvatarIKGoal _Goal)       {using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); return GetMemberByGoal(_Goal).GetIKPosition(kinetixAvatar.Root.gameObject); }
		public float      GetIKPositionWeight(AvatarIKGoal _Goal) => GetMemberByGoal(_Goal).targetPositionWeight;
		public Quaternion GetIKRotation(AvatarIKGoal _Goal)       {using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); return GetMemberByGoal(_Goal).GetEndBoneRotation(kinetixAvatar.Root.gameObject); }
		public float      GetIKRotationWeight(AvatarIKGoal _Goal) => GetMemberByGoal(_Goal).targetRotationWeight;
		public bool       GetIKAdjustHips(AvatarIKGoal _Goal) => GetMemberByGoal(_Goal).clampHips;
		/* SET */
		public void SetIKHintPosition(AvatarIKHint _Hint, Vector3 _Value) {using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); GetMemberByHint(_Hint).SetPoleVector(kinetixAvatar.Root.gameObject, _Value); }
		public void SetIKPosition(AvatarIKGoal _Goal, Vector3 _Value)     {using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); GetMemberByGoal(_Goal).SetIKPosition(kinetixAvatar.Root.gameObject, _Value); }
		public void SetIKPositionWeight(AvatarIKGoal _Goal, float _Value) => GetMemberByGoal(_Goal).targetPositionWeight = _Value;
		public void SetIKRotation(AvatarIKGoal _Goal, Quaternion _Value) { using var _ = new RootMotionContext(rootMotion, kinetixAvatar.Root, sampler); GetMemberByGoal(_Goal).SetEndBoneRotation(kinetixAvatar.Root.gameObject, _Value);  }
		public void SetIKRotationWeight(AvatarIKGoal _Goal, float _Value) => GetMemberByGoal(_Goal).targetRotationWeight = _Value;
		public void SetIKAdjustHips(AvatarIKGoal _Goal, bool _Value) => GetMemberByGoal(_Goal).clampHips = _Value;

		private IKEffectMember GetMemberByHint(AvatarIKHint _Hint) => _Hint switch
		{
			AvatarIKHint.LeftKnee => ik.leftFoot,
			AvatarIKHint.RightKnee => ik.rightFoot,
			AvatarIKHint.LeftElbow => ik.leftHand,
			AvatarIKHint.RightElbow => ik.rightHand,
			_ => null
		};

		private IKEffectMember GetMemberByGoal(AvatarIKGoal _Goal) => _Goal switch
		{
			AvatarIKGoal.LeftFoot => ik.leftFoot,
			AvatarIKGoal.RightFoot => ik.rightFoot,
			AvatarIKGoal.LeftHand => ik.leftHand,
			AvatarIKGoal.RightHand => ik.rightHand,
			_ => null
		};
		#endregion

		//=====================================//
		// Other                               //
		//=====================================//

		/// <inheritdoc/>
		protected override void Update()
		{
			sampler?.Update();
		}

		#region Sampler Event
		private void Sampler_RequestAdaptToInterpreter(KinetixFrame _Frame)
		{
			_Frame.AdaptToInterpreter(poseInterpretor[0]);
		}

		private void Sampler_OnPlayedFrame(KinetixFrame _Frame)
		{
			currentFrame = _Frame;

			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					_Frame.Sample(poseInterpretor[i]);
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

			networkSampler.OnPlayStart(_Clip);
			Call_OnAnimationStart(_Clip.animationIds);
		}

		private void Sampler_OnAnimationStop(KinetixClipWrapper _Clip)
		{
			currentFrame = null;
		
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.AnimationEnd(_Clip);
				}
			}

			networkSampler.OnPlayEnd();
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

			OnQueueStart?.Invoke();
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

			OnQueueStop?.Invoke();
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

		private void NetworkSampler_OnSerializeData(KinetixNetworkDataRaw _Data)
		{
			currentNetworkData = _Data;
			OnNetworkingData?.Invoke(_Data);
		}
		#endregion

		public override void Dispose()
		{
			base.Dispose();
			OnNetworkingData = null;

			networkSampler.OnSerializeData -= NetworkSampler_OnSerializeData;

			sampler.OnQueueStart              -= Sampler_OnQueueStart             ;
			sampler.OnQueueStop               -= Sampler_OnQueueStop              ;
			sampler.OnAnimationStart          -= Sampler_OnAnimationStart         ;
			sampler.OnAnimationStop           -= Sampler_OnAnimationStop          ;
			sampler.OnPlayedFrame             -= Sampler_OnPlayedFrame            ;
			sampler.RequestAdaptToInterpreter -= Sampler_RequestAdaptToInterpreter;
			sampler.RequestAvatarPos          -= Sampler_RequestAvatarPos         ;
			sampler.RequestAvatar             -= Sampler_RequestAvatar            ;

			UnregisterAllIkEvents();

			sampler.Dispose();
		}
	}
}
