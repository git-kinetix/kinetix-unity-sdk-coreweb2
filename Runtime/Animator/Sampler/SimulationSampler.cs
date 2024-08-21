// // ----------------------------------------------------------------------------
// // <copyright file="SimulationSampler.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
	public class SimulationSampler : IDisposable, IEnumerable<KinetixClipTrack>
	{
		public const float DEFAULT_CLIP_BLEND = 1;
		public const float DEFAULT_QUEUE_END_BLEND = 0.35f;

		// ====================================================================== //
		//  EVENTS                                                                //
		// ====================================================================== //
		public event Action                                      OnQueueStart             ;
		public event Action                                      OnQueueStop              ;
		public event Action<KinetixClipWrapper>                  OnAnimationStart         ;
		public event Action<KinetixClipWrapper>                  OnAnimationStop          ;
		public event Action<KinetixFrame>                        OnPlayedFrame            ;
		public event Action<KinetixFrame>                        RequestAdaptToInterpreter;
		public event SamplerAuthorityBridge.GetAvatarPosDelegate RequestAvatarPos         ;
		internal event SamplerAuthorityBridge.GetAvatarDelegate  RequestAvatar            ;
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  FIELDS AND PROPERTIES                                                 //
		// ====================================================================== //
		#region FIELDS AND PROPERTIES

		//  Effect Sampler
		// ----------------- //
		private readonly EffectSampler effect;
		public EffectSampler Effect => effect;
		/// <summary>
		/// If you're using a different blend 
		/// </summary>
		public float defaultBlendOffset = DEFAULT_CLIP_BLEND;
		/// <summary>
		/// If true, do blendshape animations
		/// </summary>
		public bool blendshapeEnabled = true;

		/// <summary>
		/// Instance Singleton shared between every effects
		/// </summary>
		private readonly SamplerAuthorityBridge authorityBridge;

		private uint currentPlayingTrackHash = 0;

		//  Queue and play
		// ----------------- //
		private float removeOldClipsAfterTime = -1;
		private float softStopQueueDuration; //Time (after a soft stop) before the sampler call 'Stop()'
		private bool isSoftStop;
		private bool playEnabled = false;
		private bool isPlaying   = false;

		public float ElapsedTime { get => elapsedTime; set => elapsedTime = value; }
		private float elapsedTime;
		private float previousFrameTime;

		//  Cache
		// ----------------- //
		internal HumanBodyBones[] bones;

		//  Samplers
		// ----------------- //
		private float playRate = 1f;
		public AnimationLoopMode AnimationLoopMode { get; set; } = AnimationLoopMode.Default;

		private uint nextAssignTrackHash = 1;
		private List<KinetixClipTrack> tracks = new List<KinetixClipTrack>();
		private List<KinetixClipTrack> playAfterStopTracks = new List<KinetixClipTrack>();
		private bool doPlayAfterStop = false;
		private KinetixPose interpreterAvatarPos;
		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  CTOR AND DISPOSE                                                      //
		// ====================================================================== //
		#region CTOR AND DISPOSE
		public SimulationSampler()
		{
			authorityBridge = new SamplerAuthorityBridge()
			{
				GetAvatarPos = Authority_GetAvatarPos,
				GetAvatar = Authority_GetAvatar,
				GetQueueDirection = Authority_AnimationDirection,
				GetPlayRate = Authority_GetPlayRate,
				GetQueueDuration = Authority_GetQueueDuration,
				GetQueueElapsedTime = Authority_GetQueueElapsedTime,
			};

			effect = new EffectSampler(authorityBridge);
			effect.OnFrameAdded += SamplerEffect_OnFrameAdded;
		}

		public void Dispose()
		{
			//TODO: Dispose tracks
			tracks = null;
	
			effect.OnFrameAdded -= SamplerEffect_OnFrameAdded;

			OnQueueStart = null;
			OnQueueStop = null;
			OnAnimationStart = null;
			OnAnimationStop = null;
			OnPlayedFrame = null;

			playEnabled = false;
		}
		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  EFFECT AUTHORITY                                                      //
		// ====================================================================== //
		#region AUTHORITY
		private KinetixPose Authority_GetAvatarPos()
		{
			return interpreterAvatarPos ??= RequestAvatarPos?.Invoke();
		}

		private SkeletonPool.PoolItem     Authority_GetAvatar()                  => RequestAvatar?.Invoke();
		private AnimationLoopMode Authority_AnimationDirection() => AnimationLoopMode;
		private float Authority_GetPlayRate() => GetPlayRate();
		private float Authority_GetQueueDuration() => GetQueueDuration();
		private float Authority_GetQueueElapsedTime() => elapsedTime;
		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  SAMPLER                                                               //
		// ====================================================================== //
		#region SAMPLER
		protected KinetixClipTrack AddTrack(KinetixClipTrack _Track)
		{
			_Track.SampleFrameHandler = SampleFrameHandler;
			if (isSoftStop)
				playAfterStopTracks.Add(_Track);
			else
				tracks.Add(_Track);

			unchecked
			{
				_Track.hash = nextAssignTrackHash++;
				if (_Track.hash == 0)
				{
					nextAssignTrackHash++;
					_Track.hash++;
				}
			}
			
			return _Track;
		}

		protected void DequeueSampler(int _Index)
		{
			if (isSoftStop)
				return;

			tracks.RemoveAt(_Index);
		}

		protected KinetixFrame SampleFrameHandler(KinetixClip _KinetixClip, int _Frame)
		{
			KinetixFrame toReturn = bones == null ?
				new KinetixFrame(out bones, _KinetixClip, _Frame) :
				new KinetixFrame(bones, _KinetixClip, _Frame);

			if (!blendshapeEnabled)
				toReturn.hasBlendshapes = false;
			RequestAdaptToInterpreter?.Invoke(toReturn);

			return toReturn;
		}
		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  PLAYMODE CONTROLS                                                     //
		// ====================================================================== //
		#region PLAYMODE CONTROLS
		public float GetPlayRate() => playRate;
		public void SetPlayRate(float rate)
		{
			playRate = rate;
		}

		public float GetQueueDuration()
		{
			if (tracks.Count == 0) 
				return 0;
			
			return tracks.Max(Max);
			static float Max(KinetixClipTrack __Track)
			{
				return __Track.GlobalEndTime;
			}
		}

		/// <summary>
		/// Helper method for <see cref="Play(bool, float, KinetixClipWrapper)"/> and <see cref="Add(bool, float, KinetixClipWrapper)"/>
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <returns></returns>
		private float GetAddStartTime(bool _Immediate)
		{
			float startTime = 0;
			if (!isSoftStop)
			{
				startTime = _Immediate ?
					elapsedTime + defaultBlendOffset :
					GetQueueDuration() - defaultBlendOffset;
			}

			return startTime;
		}

		public void Pause()  => playEnabled = false;
		public void Resume() => playEnabled = true ;
		public bool GetIsPlaying() => isPlaying;
		public bool GetIsPaused() => !playEnabled;

		public void Play()
		{

			if (isSoftStop)
			{
				doPlayAfterStop = true;
				return;
			}

			if (isPlaying)
			{
				playEnabled = true;
				isPlaying = true;
				return;
			}

			if (tracks.Count == 0)
				return;

			InvokeSoftStop(-1);
			currentPlayingTrackHash = 0;
			elapsedTime = playRate < 0 ? GetQueueDuration() : 0;
			previousFrameTime = elapsedTime;
			playEnabled = true;
			isPlaying = true;
			doPlayAfterStop = false;
			InvokeQueueStart();
		}

		/// <summary>
		/// Add a clip and start playing the sampler
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_Clip">Clip to play</param>
		public KinetixClipTrack Play(bool _Immediate, KinetixClipWrapper _Clip)
		{
			float time = GetAddStartTime(_Immediate);
			return Play(_Immediate, Mathf.Max(0, time), _Clip);
		}

		/// <summary>
		/// Add a clip and start playing the sampler
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_StartTime">Time at which to start the _Track</param>
		/// <param name="_Clip">Clip to play</param>
		public KinetixClipTrack Play(bool _Immediate, float _StartTime, KinetixClipWrapper _Clip)
		{
			KinetixClipTrack toReturn = Add(_Immediate, _StartTime, _Clip);
			Play();
			return toReturn;
		}


		/// <summary>
		/// Add a clips and start playing the sampler
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_Clips">Clips to play</param>
		public KinetixClipTrack[] PlayClips(bool _Immediate, params KinetixClipWrapper[] _Clips)
		{
			float startTime = GetAddStartTime(_Immediate);
			return PlayClips(_Immediate, startTime, _Clips);
		}

		/// <summary>
		/// Add a clips and start playing the sampler
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_StartTime">Time at which to start the tracks</param>
		/// <param name="_Clips">Clips to play</param>
		public KinetixClipTrack[] PlayClips(bool _Immediate, float _StartTime, params KinetixClipWrapper[] _Clips)
		{
			KinetixClipTrack[] toReturn = AddClips(_Immediate, _StartTime, _Clips);
			Play();
			return toReturn;
		}

		/// <summary>
		/// Add a _Track to the sampler without triggeting <see cref="Play()"/>
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_Clip">Clips to play</param>
		public KinetixClipTrack Add(bool _Immediate, KinetixClipWrapper _Clip)
		{
			float time = GetAddStartTime(_Immediate);
			return Add(_Immediate, Mathf.Max(0, time), _Clip);
		}

		/// <summary>
		/// Add a _Track to the sampler without triggeting <see cref="Play()"/>
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_StartTime">Time at which to start the _Track</param>
		/// <param name="_Clip">Clip to play</param>
		public KinetixClipTrack Add(bool _Immediate, float _StartTime, KinetixClipWrapper _Clip)
		{
			return AddClips(_Immediate, _StartTime, _Clip)[0];
		}

		/// <summary>
		/// Add tracks to the sampler without triggeting <see cref="Play()"/>
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_Clips">Clips to play</param>
		public KinetixClipTrack[] AddClips(bool _Immediate, params KinetixClipWrapper[] _Clips)
		{
			float startTime = GetAddStartTime(_Immediate);
			return AddClips(_Immediate, startTime, _Clips);
		}

		/// <summary>
		/// Add tracks to the sampler without triggeting <see cref="Play()"/>
		/// </summary>
		/// <param name="_Immediate">If true, stop current _Track with a blend and play provided clips</param>
		/// <param name="_StartTime">Time at which to start the tracks</param>
		/// <param name="_Clips">Clips to play</param>
		public KinetixClipTrack[] AddClips(bool _Immediate, float _StartTime, params KinetixClipWrapper[] _Clips)
		{
			KinetixClipTrack[] toReturn = new KinetixClipTrack[_Clips.Length];
			if (!isSoftStop && _Immediate)
			{
				if (!isPlaying || !playEnabled || playRate < 0)
				{
					Stop();
				}
				else
				{
					removeOldClipsAfterTime = _StartTime - elapsedTime;
					ResetElapsedTimeAndClearOldTracks(removeOldClipsAfterTime, true);
				}
				_StartTime = 0;
			}

			float nextStartTime = _StartTime;
			int length = _Clips.Length;
			for (int i = 0; i < length; i++)
			{
				KinetixClipTrack item = new KinetixClipTrack(_Clips[i])
				{
					timelineStartTime = nextStartTime
				};
				nextStartTime = item.GlobalEndTime - defaultBlendOffset;
				AddTrack(item);

				toReturn[i] = item;
			}

			return toReturn;
		}

		public void RemoveAllTracks()
		{
			for (int i = tracks.Count - 1; i >= 0; i--)
			{
				tracks[i].Dispose();
				tracks.RemoveAt(i);
			}
		}

		/// <summary>
		/// Remove a _Track from the queue
		/// </summary>
		/// <param name="_Track">Track to remove</param>
		public void RemoveTrack(KinetixClipTrack _Track)
		{
			if (tracks == null)
			{
				_Track.Dispose();
				return;
			}

			if (_Track.hash == currentPlayingTrackHash)
			{
				InvokeAnimationStop(currentPlayingTrackHash);
				currentPlayingTrackHash = 0;
			}

			tracks.Remove(_Track);
			_Track.Dispose();
		}

		/// <summary>
		/// Method that align all _Track to 0 and reset the elapsed time
		/// </summary>
		/// <param name="_TruncateOffset">SetData the end time of tracks to {<see cref="elapsedTime"/>+<paramref name="_TruncateOffset"/>}</param>
		/// <param name="_KillFuturTracks">If true, remove all tracks that weren't played</param>
		public void ResetElapsedTimeAndClearOldTracks(float? _TruncateOffset = null, bool _KillFuturTracks = true)
		{
			KinetixClipTrack kinetixClipTrack;
			for (int i = tracks.Count - 1; i >= 0; i--)
			{
				kinetixClipTrack = tracks[i];

				if (!_KillFuturTracks && kinetixClipTrack.timelineStartTime > elapsedTime)
					continue;

				if (!kinetixClipTrack.ContainsGlobalTime(elapsedTime))
				{
					kinetixClipTrack.Dispose();
					tracks.RemoveAt(i);
				}
				else
				{
					kinetixClipTrack.timelineStartTime -= elapsedTime;
					if (_TruncateOffset.HasValue)
						kinetixClipTrack.TruncateEnd(_TruncateOffset.Value);	
				}
			}

			elapsedTime = 0;
			previousFrameTime = 0;
		}

		/// <summary>
		/// Stop with a blend to the Avatar (requires the effect <see cref="AnimatorBlendEffect"/> )
		/// </summary>
		public void SoftStop() => SoftStop(DEFAULT_QUEUE_END_BLEND);
		/// <summary>
		/// Stop with a blend to the Avatar (requires the effect <see cref="AnimatorBlendEffect"/> )
		/// </summary>
		/// <param name="_StopDelay">Delay before complete stop. Note doesn't influence the duration of the blending. See <see cref="AnimatorBlendEffect.blendDuration"/>.</param>
		public void SoftStop(float _StopDelay)
		{
			if (tracks.Count == 0)
			{
				Stop();
				return;
			}

			AnimationLoopMode = AnimationLoopMode.Default;
			float duration = GetQueueDuration();
			_StopDelay = Mathf.Min(_StopDelay, duration - elapsedTime);
			if (_StopDelay < 0)
			{
				Stop();
				return;
			}
			
			if (_StopDelay < Mathf.Epsilon)
				Stop();

			isSoftStop = true;
			ResetElapsedTimeAndClearOldTracks(_StopDelay);
			softStopQueueDuration = GetQueueDuration();
			InvokeSoftStop(_StopDelay);
			Resume();
		}
		
		/// <summary>
		/// Completely stop and reset the sampler without blending.
		/// </summary>
		public void Stop()
		{
			interpreterAvatarPos = null;
			removeOldClipsAfterTime = -1;
			isSoftStop = false; //REQUIRED FOR CLEARING SOFT STOP
			InvokeSoftStop(-1); //REQUIRED FOR CLEARING SOFT STOP

			elapsedTime = 0;
			previousFrameTime = 0;
			playEnabled = false;
			isPlaying = false;
			AnimationLoopMode = AnimationLoopMode.Default;

			if (tracks.Count > 0 && currentPlayingTrackHash != 0)
				InvokeAnimationStop(currentPlayingTrackHash);

			currentPlayingTrackHash = 0;

			if (tracks.Count > 0)
				InvokeQueueStop();

			RemoveAllTracks();

			tracks = playAfterStopTracks;
			playAfterStopTracks = new List<KinetixClipTrack>();
			if (doPlayAfterStop)
				Play();
		}

		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  UPDATE                                                                //
		// ====================================================================== //
		public void Update()
		{
			int samplersCount = tracks.Count;
			if (isPlaying && samplersCount == 0)
			{
				Stop();
				return;
			}

			if (!playEnabled && elapsedTime == previousFrameTime)
			{
				return;
			}

			if (samplersCount == 0)
			{
				Stop();
				return;
			}

			if (isSoftStop && softStopQueueDuration != GetQueueDuration())
			{
				Stop();
				return;
			}

			float currentElapsedTime = elapsedTime;
			elapsedTime += Time.deltaTime * playRate;
			previousFrameTime = elapsedTime;

			if (removeOldClipsAfterTime >= 0 && elapsedTime > removeOldClipsAfterTime)
			{
				removeOldClipsAfterTime = -1;
				KinetixClipTrack kinetixClipTrack;
				for (int i = tracks.Count - 1; i >= 0; i--)
				{
					kinetixClipTrack = tracks[i];

					if (kinetixClipTrack.timelineStartTime > elapsedTime)
						continue;

					if (!kinetixClipTrack.ContainsGlobalTime(elapsedTime))
					{
						kinetixClipTrack.Dispose();
						tracks.RemoveAt(i);
					}
				}

				samplersCount = tracks.Count;
			}

			//--------- Get frames from tracks ---------// 
			KinetixFrame frame;
			List<KinetixFrame> frames = new List<KinetixFrame>();
			List<KinetixClipTrack> playedTracks = new List<KinetixClipTrack>();

			if (currentElapsedTime > GetQueueDuration() || currentElapsedTime < 0)
			{
				switch (AnimationLoopMode)
				{
					case AnimationLoopMode.Default:
						Stop();
						break;
					case AnimationLoopMode.Loop:
						if (playRate < 0)
							elapsedTime = GetQueueDuration();
						else
							elapsedTime = 0;
						break;
				}
				return;
			}

			uint maxHash = 0;
			float maxTime = playRate > 0 ? float.MinValue : float.MaxValue;

			KinetixClipTrack forTrack;
			float globalEndTime;
			//Get frames from each tracks 
			for (int i = samplersCount - 1; i >= 0; i--)
			{
				forTrack = tracks[i];
				frame = forTrack.Update(currentElapsedTime);
				if (frame != null)
				{
					globalEndTime = playRate > 0 ? forTrack.GlobalEndTime : forTrack.timelineStartTime;
					if ((globalEndTime - maxTime) * playRate > 0) //If globalEndTime is of the sign of playRate
					{
						//(globalEndTime > maxTime && playRate > 0)
						//||
						//(globalEndTime < maxTime && playRate < 0)

						maxHash = forTrack.hash;
						maxTime = globalEndTime;
					}

					frames.Add(frame);
					playedTracks.Add(forTrack);
				}
				else if (forTrack.hash == currentPlayingTrackHash)
				{
					InvokeAnimationStop(currentPlayingTrackHash);
					currentPlayingTrackHash = 0;
				}
			}

			frames.Reverse();
			playedTracks.Reverse();
			if (frames.Count == 0)
			{
				if (currentPlayingTrackHash != 0)
					InvokeAnimationStop(currentPlayingTrackHash);

				currentPlayingTrackHash = 0;
				return;
			}

			if (maxHash != currentPlayingTrackHash)
			{
				if (currentPlayingTrackHash != 0)
					InvokeAnimationStop(currentPlayingTrackHash);

				currentPlayingTrackHash = maxHash;
				InvokeAnimationStart(currentPlayingTrackHash);
			}

			//Compute effect and send _Frame
			bool hasBlendshapes = frames.Any(f => f.hasBlendshapes);
			frame = effect.ModifyFrame(frames.ToArray(), playedTracks.ToArray());
			frame.hasBlendshapes = hasBlendshapes;
			
			OnPlayedFrame?.Invoke(frame);

			interpreterAvatarPos = null; //Clear avatar pos cache
		}
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  EVENT INVOKE                                                          //
		// ====================================================================== //
		#region EVENT INVOKE
		private void InvokeQueueStart()
		{
			effect.OnQueueStart();
			OnQueueStart?.Invoke();
		}
		private void InvokeQueueStop() 
		{
			effect.OnQueueStop();
			OnQueueStop?.Invoke();
		}
		private void InvokeAnimationStart(uint _TrackHash)
		{
			KinetixClipWrapper clip = tracks.First(t => t.hash == _TrackHash).Clip;
			OnAnimationStart?.Invoke(clip);
		}
		private void InvokeAnimationStop(uint _TrackHash) 
		{
			KinetixClipWrapper clip = tracks.First(t => t.hash == _TrackHash).Clip;
			OnAnimationStop?.Invoke(clip);
		}
		private void InvokeSoftStop(float _SoftDuration) 
		{
			effect.OnSoftStop(_SoftDuration);
		}
		#endregion
		// ---------------------------------------------------------------------- //

		// ====================================================================== //
		//  EVENT HANDLER                                                         //
		// ====================================================================== //
		#region EVENT HANDLER
		private void SamplerEffect_OnFrameAdded(KinetixFrame _Frame)
		{
			OnPlayedFrame?.Invoke(_Frame);
		}
		#endregion
		// ---------------------------------------------------------------------- //
		
		public IEnumerator<KinetixClipTrack> GetEnumerator()
		{
			return ((IEnumerable<KinetixClipTrack>)tracks).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)tracks).GetEnumerator();
		}

	}
}
