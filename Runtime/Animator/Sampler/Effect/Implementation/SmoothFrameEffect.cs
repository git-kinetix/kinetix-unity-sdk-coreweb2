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
	///	Interpolate between 2 _Frames (_Current and next) of each track
	/// </summary>
	public class SmoothFrameEffect : IFrameEffectModify, ISamplerAuthority
	{
		private const double TIME_EPSILON = 1e-5;
		private const double MAX_TIME_EPSILON = 1 - TIME_EPSILON;
		private Dictionary<int, int> previousFramesSet = new Dictionary<int, int>(); // Dico<hash, frameId>
		private Dictionary<int, KinetixFrame> nextFramesSet = new Dictionary<int, KinetixFrame>(); // Dico<hash, nextFrame>

		private sbyte oldPlayRateSign = 1;

		/// <inheritdoc/>
		public SamplerAuthorityBridge Authority { get; set; }

		public int Priority => 200;

		public SmoothFrameEffect() {}

		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
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

					//Blend 'current' with the next frame to be played
					Blend(ref current, nextFrame, time);
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

		private void Blend(ref KinetixFrame _Current, KinetixFrame _PreviousFrame, float _Time)
		{
			//Blendshapes
			int count = _Current.blendshapes.Count;
			for (int i = 0; i < count; i++)
			{
				_Current.blendshapes[(ARKitBlendshapes)i] = Blend(
					_Current.blendshapes[(ARKitBlendshapes)i], 
					_PreviousFrame.blendshapes[(ARKitBlendshapes)i], 
					_Time
				);
			}
			
			//Bones
			count = _Current.humanTransforms.Length;
			for (int i = 0; i < count; i++)
			{
				Blend(ref _Current.humanTransforms[i], _PreviousFrame.humanTransforms[i], _Time);
			}

			//Armature
			Blend(ref _Current.armature, _PreviousFrame.armature, _Time);
		}

		private void Blend(ref TransformData? _Current, TransformData? _PreviousFrame, float _Time)
		{
			if (!_PreviousFrame.HasValue)
				return;

			if (!_Current.HasValue)
			{
				if (_PreviousFrame.HasValue)
					_Current = _PreviousFrame;
				return;
			}

			TransformData data = _Current.Value;
			Blend(ref data, _PreviousFrame.Value, _Time);
			_Current = data;
		}

		private void Blend(ref TransformData _Current, TransformData _PreviousFrame, float _Time)
		{
			if (_Current.position.HasValue && _PreviousFrame.position.HasValue)
				_Current.position = Vector3.Lerp(_Current.position.Value, _PreviousFrame.position.Value, _Time);
	
			if (_Current.rotation.HasValue && _PreviousFrame.rotation.HasValue)
				_Current.rotation = Quaternion.Slerp(_Current.rotation.Value, _PreviousFrame.rotation.Value, _Time);
			
			if (_Current.scale.HasValue && _PreviousFrame.scale.HasValue)
				_Current.scale = Vector3.Lerp(_Current.scale.Value, _PreviousFrame.scale.Value, _Time);
		}

		private float Blend(float _CurrentBlend, float _PreviousFrameBlend, float _Time) => Mathf.Lerp(_CurrentBlend, _PreviousFrameBlend, _Time);

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
