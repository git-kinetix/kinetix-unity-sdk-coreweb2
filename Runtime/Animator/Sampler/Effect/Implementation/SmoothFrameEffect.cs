// // ----------------------------------------------------------------------------
// // <copyright file="SmoothFrameEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Kinetix.Internal
{
	/// <summary>
	///	Interpolate between 2 _Frames (_Current and _Next) of each track
	/// </summary>
	public class SmoothFrameEffect : AInterpolationEffect, IFrameEffectModify, ISamplerAuthority
	{
		private const double TIME_EPSILON = 1e-5;
		private const double MAX_TIME_EPSILON = 1 - TIME_EPSILON;
		private Dictionary<int, int> previousFramesSet = new Dictionary<int, int>(); // Dico<hash, frameId>
		private Dictionary<int, KinetixFrame> nextFramesSet = new Dictionary<int, KinetixFrame>(); // Dico<hash, nextFrame>

		private sbyte oldPlayRateSign = 1;

		public int Priority => 200;
		public bool IsEnabled { get; set; } = true;

		public SmoothFrameEffect() {}

		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
			if (!IsEnabled || _Tracks == null || _Tracks.Length == 0 || _Frames.Length == 0)
				return;

			KinetixClipTrack track;
			KinetixFrame current, nextFrame;
			bool containsFrame;
			int hash;
			float time;
			float localElapsedTime;
			float elapsedTime = Authority.GetQueueElapsedTime();
			float playRate = Authority.GetPlayRate();

			if (Mathf.Sign(playRate) != oldPlayRateSign)
			{
				oldPlayRateSign = (sbyte)((playRate >= 0) ? 1 : (-1)); //Sign
				previousFramesSet.Clear();
				nextFramesSet.Clear();
			}

			List<int> keys = new List<int>(previousFramesSet.Keys);
			for (int i = 0; i < _Frames.Length; i++)
			{
				current = _Frames[i];
				track = _Tracks[i];
				hash = track.GetHashCode();
				keys.Remove(hash);
				if ((containsFrame = previousFramesSet.TryGetValue(hash, out int frameId)) && frameId == current.frame)
				{
					localElapsedTime = track.GlobalToLocalTime(elapsedTime);
					nextFrame = nextFramesSet[hash];
					if (playRate < 0)
						time = Mathf.Abs((current.CurrentTime - localElapsedTime) / (nextFrame.CurrentTime - current.CurrentTime));
					else
						time = Mathf.Abs((nextFrame.CurrentTime - localElapsedTime) / (nextFrame.CurrentTime - current.CurrentTime));

					//Clone
					current = new KinetixFrame(current);

					if (time < TIME_EPSILON || time > MAX_TIME_EPSILON)
						continue;
					
					time = 1 - time;

					//Lerp 'current' with the next frame to be played
					Lerp(ref current, nextFrame, time);
					current.CurrentTime = localElapsedTime;
					_Frames[i] = current;
				}
				else
				{
					//Get next frame to be played (use the playRate's sign)
					int frame = playRate < 0 ? current.frame - 1 : current.frame + 1;
					if (frame < 0 || frame >= current.clip.KeyCount)
						continue;

					nextFrame = track.SampleFrameHandler(track.Clip, frame);
					if (containsFrame)
					{
						previousFramesSet[hash] = _Frames[i].frame; //Add a frame in the set
						nextFramesSet    [hash] = nextFrame;
					}
					else
					{
						previousFramesSet.Add(hash, _Frames[i].frame); //Add a frame in the set
						nextFramesSet    .Add(hash, nextFrame);
					}

				}
			}

			for (int i = 0; i < keys.Count; i++)
			{
				previousFramesSet.Remove(keys[i]); //Remove old frames
				nextFramesSet.Remove(keys[i]); //Remove old frames
			}
		}

		public void OnQueueEnd()
		{
			previousFramesSet.Clear();
			nextFramesSet.Clear();
		}

		public void OnQueueStart()
		{
			oldPlayRateSign = (sbyte)((Authority.GetPlayRate() >= 0) ? 1 : (-1)); //Sign
		}

		public void Update(){}

		public void OnSoftStop(float blendTime){}
	}
}
