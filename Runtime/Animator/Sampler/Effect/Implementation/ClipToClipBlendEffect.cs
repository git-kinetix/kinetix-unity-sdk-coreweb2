// // ----------------------------------------------------------------------------
// // <copyright file="ClipToClipBlendEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
	/// <summary>
	/// An effect that blends <see cref="KinetixClip"/> between them
	/// </summary>
	public class ClipToClipBlendEffect : AInterpolationEffect, IFrameEffectModify, ISamplerAuthority
	{
		// How does it work :
		//
		//                blend        blend
		//          0    |......1     |......2
		// clip 1 : |-----------|    
		// clip 2 :      |-------------------|
		// clip 3 :                   |-----------|

		private bool isPlaying = false;
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// Lerp duration in seconds
		/// </summary>
		public float blendDuration;

		public int Priority => 100;

		/// <param name="_BlendDuration">Lerp duration in seconds</param>
		public ClipToClipBlendEffect(float _BlendDuration = 1f)
		{
			this.blendDuration = _BlendDuration;
		}

		/// <inheritdoc/>
		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
        {
			if (!IsEnabled) return;
			if (!isPlaying) return;

			float elapsedTime = Authority.GetQueueElapsedTime();

			int length = _Frames.Length;
			if (length > 1 && _Tracks != null)
			{
				KinetixClipTrack track;

				float[] weights = new float[length];
				float blendDuration, localElapsedTime;
				for (int i = 0; i < length; i++)
				{
					blendDuration = this.blendDuration;
					track = _Tracks[i];
					localElapsedTime = track.GlobalToLocalTime(elapsedTime);

					if (track.timeRange.Range < blendDuration * 2f)
					{
						blendDuration = track.timeRange.Range / 2f;
					}

					weights[i] = Mathf.Min(
						1,
						Mathf.Abs( (localElapsedTime - track.timeRange.minTime) / blendDuration), //start blend weight = 1 ; end blend weight = 0
						Mathf.Abs( (track.timeRange.maxTime - localElapsedTime) / blendDuration)  //start blend weight = 0 ; end blend weight = 1
					);
				}

				var tempTracks = _Tracks;
				Average(ref _FinalFrame, _Frames, weights);
			}
			else
			{
				_FinalFrame = new KinetixFrame(_Frames[0]);
			}
		}

		/// <inheritdoc/>
		public void OnQueueEnd()
		{
			isPlaying = false;
		}

		/// <inheritdoc/>
		public void OnQueueStart()
		{
			isPlaying = true;
		}

		/// <inheritdoc/>
		public void Update() {}

		public void OnSoftStop(float _)
		{
		}
	}
}
