// // ----------------------------------------------------------------------------
// // <copyright file="AnimatorBlendEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine;

namespace Kinetix.Internal
{
	/// <summary>
	/// Effect that can blend between an animator and kinetix clip system
	/// </summary>
	public class AnimatorBlendEffect : AInterpolationEffect, IFrameEffectModify, ISamplerAuthority
	{
		public int Priority => 90;
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Lerp duration in seconds
		/// </summary>
		public float blendDuration;
		private bool isFirstLoop = true;
		private bool disableBegginingBlend = true;
		private sbyte previousSampleRateSign = 0;
		private float softStop = -1;

		/// <param name="_BlendDuration">
		/// Lerp duration in seconds
		/// </param>
		public AnimatorBlendEffect(float _BlendDuration = 0.35f)
		{
			this.blendDuration = _BlendDuration;
		}

		/// <inheritdoc/>
		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
			if(!IsEnabled)
				return;

			sbyte sampleRateSign = 0;
			float sampleRate = Authority.GetPlayRate();

			//Manual sign getting (0 is also a valid value)
			if (sampleRate > 0)
				sampleRateSign = 1;
			else if (sampleRate < 0)
				sampleRateSign = -1;

			previousSampleRateSign = sampleRateSign;


			if (!isFirstLoop && Authority.GetQueueDirection() == AnimationLoopMode.Loop)
				return;

			KinetixPose avatarPose = Authority.GetAvatarPos();

			float blendDuration = this.blendDuration;
			float maxTime = Authority.GetQueueDuration();
			float currentTime = Authority.GetQueueElapsedTime();
			float animationWeight;
			if (softStop > 0)
			{
				isFirstLoop = false;
				blendDuration = softStop;
				animationWeight = Mathf.Abs(currentTime / softStop);
				animationWeight = 1 - animationWeight;
			}
			else
			{
				if (maxTime < blendDuration * 2f)
					blendDuration = maxTime / 2f;

				animationWeight = Mathf.Min(
					disableBegginingBlend && sampleRate > 0 ? 100 : Mathf.Abs(currentTime / blendDuration),
					disableBegginingBlend && sampleRate < 0 ? 100 : Mathf.Abs((maxTime - currentTime) / blendDuration)
				);
			}

			if (currentTime > blendDuration)
			{
				disableBegginingBlend = true;
				isFirstLoop = false;
			}

			if (animationWeight > 1 || animationWeight < 0)
				return;

			Average(ref _FinalFrame, new KinetixPose[2] { new KinetixFrame(_FinalFrame), avatarPose }, new float[] {animationWeight, 1-animationWeight});
		}


		/// <inheritdoc/>
		public void OnQueueEnd()
		{
		}

		/// <inheritdoc/>
		public void OnQueueStart()
		{
			disableBegginingBlend = false;
			isFirstLoop = true;
			this.softStop = -1;
		}

		/// <inheritdoc/>
		public void Update() {}

		public void OnSoftStop(float _SoftDuration)
		{
			this.softStop = _SoftDuration;
		}
	}
}
