// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCharacterComponentRemote.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;
using Kinetix.Internal;
using Kinetix.Internal.Utils;
using Kinetix.Internal.Retargeting;

namespace Kinetix
{
	/// <summary>
	/// A remote character. It recieves poses from the network and apply them to the character.
	/// </summary>
	public class KinetixCharacterComponentRemote : KinetixCharacterComponent
	{
		const float OUTER_BLEND_DURATION = 0.35f;

		private SimulationSampler sampler;

		///<inheritdoc/>
		public override void Init(ServiceLocator _ServiceLocator, KinetixAvatar _KinetixAvatar, RootMotionConfig _RootMotionConfig)
		{
			base.Init(_ServiceLocator, _KinetixAvatar, _RootMotionConfig);

			networkSampler.OnDeserializeData += NetworkSampler_OnDeserializeData;
		
			sampler = new SimulationSampler();
			sampler.OnQueueStart += Sampler_OnQueueStart;
			sampler.OnQueueStop += Sampler_OnQueueStop;
			sampler.OnAnimationStart += Sampler_OnAnimationStart;
			sampler.OnAnimationStop += Sampler_OnAnimationStop;
			sampler.OnPlayedFrame += Sampler_OnPlayedFrame;
			sampler.RequestAdaptToInterpreter += Sampler_RequestAdaptToInterpreter;
			sampler.RequestAvatarPos += Sampler_RequestAvatarPos;
			sampler.RequestAvatar += Sampler_RequestAvatar;

			sampler.Effect.RegisterEffect(new ClipToClipBlendEffect());
			sampler.Effect.RegisterEffect(new OuterBlendEffect(OUTER_BLEND_DURATION));
			sampler.Effect.RegisterEffect(new BlendCancelBetweenClipEffect());
			sampler.Effect.RegisterEffect(new RootMotionEffect(_RootMotionConfig));
		}

		private void NetworkSampler_OnDeserializeData(KinetixNetworkData obj)
		{
			switch (obj.action)
			{
				case KinetixNetworkData.Action.Start:
					PlayAnimation(obj.id);
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

		private void Sampler_OnAnimationStart(KinetixClipWrapper obj)
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.AnimationStart(obj.clip);
				}
			}

			Call_OnAnimationStart(obj.animationIds);
		}

		private void Sampler_OnAnimationStop(KinetixClipWrapper obj)
		{
			if (AutoPlay)
			{
				int count = poseInterpretor.Count;
				for (int i = 0; i < count; i++)
				{
					if (poseInterpretor[i] is IPoseInterpreterStartEnd startEnd) startEnd.AnimationEnd(obj.clip);
				}
			}

			Call_OnAnimationEnd(obj.animationIds);
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
				return new KinetixPose(new TransformData[0], new HumanBodyBones[0], null, default, default);
			}

			return poseInterpretor[0].GetPose();
		}
		#endregion

		#region Sampler play / stop
		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(string _AnimationIds) => PlayAnimation(new AnimationIds(_AnimationIds));

		/// <summary>
		/// Play an animation on the current player
		/// </summary>
		/// <param name="_AnimationIds">IDs of the animation</param>
		public void PlayAnimation(AnimationIds _AnimationIds)
		{
			if (_AnimationIds?.UUID == null)
			{
				KinetixDebug.LogWarning("Animation ID cannot be null when Play Animation");
				return;
			}

			KinetixEmote emote = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetEmote(_AnimationIds);

			KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().IsAnimationOwnedByUser(_AnimationIds, async (_IsOwned) => {
				if (!_IsOwned)
					await KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AddEmoteFromIds(_AnimationIds);

				KinetixClip clip = await KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().GetRetargetedClipByAvatar<KinetixClip, KinetixClipExporter>(emote, kinetixAvatar, SequencerPriority.VeryHigh, true);

				if (clip == null)
				{
					KinetixDebug.LogWarning("Can't get the animation " + _AnimationIds.UUID);
					return;
				}

				sampler.Play(new KinetixClipWrapper(clip, _AnimationIds));
			});
			
		}

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

		public override void Dispose()
		{
			base.Dispose();

			networkSampler.OnDeserializeData -= NetworkSampler_OnDeserializeData;

			sampler.OnQueueStart -= Sampler_OnQueueStart;
			sampler.OnQueueStop -= Sampler_OnQueueStop;
			sampler.OnAnimationStart -= Sampler_OnAnimationStart;
			sampler.OnAnimationStop -= Sampler_OnAnimationStop;
			sampler.OnPlayedFrame -= Sampler_OnPlayedFrame;
			sampler.RequestAvatarPos -= Sampler_RequestAvatarPos;
			sampler.RequestAvatar -= Sampler_RequestAvatar;

			sampler.Dispose();
		}

    }
}
