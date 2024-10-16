// // ----------------------------------------------------------------------------
// // <copyright file="KinetixClipTrack.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
	public class KinetixClipTrack : IDisposable, IEquatable<KinetixClipTrack>
	{
		/// <summary>
		/// Unique ID to differentiate with _Other tracks
		/// </summary>
		internal uint hash;

		public delegate KinetixFrame SampleFrame(KinetixClip clip, int frame);
		public SampleFrame SampleFrameHandler;

		public KinetixFrame PreviousFrame => previousFrame;
		private KinetixFrame previousFrame;
		private int previousFrameIndex = -1; //Used to reload frame or not

		private bool disposed;
		public bool Disposed => disposed;


		/// <summary>
		/// Clip to be played by the track
		/// </summary>
		public KinetixClipWrapper Clip => clip;
		private readonly KinetixClipWrapper clip;

		/// <summary>
		/// Time in sampler context (global time) at which the track will play
		/// </summary>
		public float timelineStartTime = 0;
		/// <summary>
		/// Range in animation context (local time) to be played
		/// </summary>
		public AnimationTimeRange TimeRange {
			get => _timeRange;
			set {
				_timeRange = value;
				FixTimeRange();
			}
		}
		private AnimationTimeRange _timeRange = AnimationTimeRange.Default;


		/// <summary>
		/// Time in sampler context (global time) at which the track will end
		/// </summary>
		public float GlobalEndTime => Duration + timelineStartTime;
		/// <summary>
		/// Duration of the track
		/// </summary>
		public float Duration => TimeRange.maxTime - TimeRange.minTime;


		/// <param name="_Clip"><see cref="Clip"/></param>
		public KinetixClipTrack(KinetixClipWrapper _Clip) : this(_Clip, AnimationTimeRange.Default) { }
		/// <param name="_Clip"><see cref="Clip"/></param>
		/// <param name="_TimeRange"><see cref="TimeRange"/></param>
		public KinetixClipTrack(KinetixClipWrapper _Clip, AnimationTimeRange _TimeRange) : this(_Clip, _TimeRange, 0) { }
		/// <param name="_Clip"><see cref="Clip"/></param>
		public KinetixClipTrack(KinetixClipWrapper _Clip, float _TimelineStartTime) : this(_Clip, AnimationTimeRange.Default, _TimelineStartTime) { }
		/// <param name="_Clip"><see cref="Clip"/></param>
		/// <param name="_TimeRange"><see cref="TimeRange"/></param>
		/// <param name="_TimelineStartTime"><see cref="timelineStartTime"/></param>
		public KinetixClipTrack(KinetixClipWrapper _Clip, AnimationTimeRange _TimeRange, float _TimelineStartTime)
		{
			this.clip = _Clip;
			this.TimeRange = _TimeRange;
			this.timelineStartTime = _TimelineStartTime;
			previousFrameIndex = -1;

			FixTimeRange();
		}

		private void FixTimeRange()
		{
			float duration = clip.clip.Duration;
			if (_timeRange.minTime < 0 || _timeRange.minTime > duration)
				_timeRange.minTime = 0;

			if (_timeRange.maxTime < 0 || _timeRange.maxTime > duration)
				_timeRange.maxTime = duration;

			if (_timeRange.minTime > _timeRange.maxTime)
			{
				//Tuple exchange: Min become Max. Max become Min
				(_timeRange.minTime, _timeRange.maxTime) = (_timeRange.maxTime, _timeRange.minTime);
			}
		}

		/// <summary>
		/// Create a track and put it after <paramref name="_PreviousTrack"/>
		/// </summary>
		/// <param name="_PreviousTrack">Track before this track</param>
		/// <param name="_BlendingTime">Blending time between the 2 tracks</param>
		/// <param name="_NewTrackClip">Clip for the new track</param>
		/// <returns></returns>
		public static KinetixClipTrack CreateTrackAfter(KinetixClipTrack _PreviousTrack, float _BlendingTime, KinetixClipWrapper _NewTrackClip)
			=> CreateTrackAfter(_PreviousTrack, _BlendingTime, _NewTrackClip, AnimationTimeRange.Default);
		/// <summary>
		/// Create a track and put it after <paramref name="_PreviousTrack"/>
		/// </summary>
		/// <param name="_PreviousTrack">Track before this track</param>
		/// <param name="_BlendingTime">Blending time between the 2 tracks</param>
		/// <param name="_NewTrackClip">Clip for the new track</param>
		/// <param name="_TimeRange">Play range of the animation (relative to the animation itself)</param>
		/// <returns></returns>
		public static KinetixClipTrack CreateTrackAfter(KinetixClipTrack _PreviousTrack, float _BlendingTime, KinetixClipWrapper _NewTrackClip, AnimationTimeRange _TimeRange)
		{
			return new KinetixClipTrack(_NewTrackClip, _TimeRange, _PreviousTrack.GlobalEndTime - _BlendingTime);
		}

		public KinetixFrame Update(float _GlobalElapsedTime)
		{
			FixTimeRange();

			float elapsedTime = GlobalToLocalTime(_GlobalElapsedTime);

			KinetixFrame toReturn = null;
			int frame = Mathf.FloorToInt(elapsedTime * clip.clip.FrameRate); //s * f/s = f
			int minFrame = Mathf.RoundToInt(TimeRange.minTime * clip.clip.FrameRate);
			int maxFrame = Mathf.RoundToInt(TimeRange.maxTime * clip.clip.FrameRate);

			if (frame >= minFrame && frame < maxFrame)
			{
				if (frame != previousFrameIndex || previousFrame == null)
				{
					previousFrameIndex = frame;
					previousFrame
						= toReturn
						= SampleFrameHandler(clip, previousFrameIndex);
				}
				else
				{
					toReturn = previousFrame;
				}

				return toReturn;
			}
			else
			{
				previousFrameIndex = -1;
				return previousFrame = null;
			}

		}

		public float GlobalToLocalTime(float _GlobalElapsedTime)
		{
			return _GlobalElapsedTime - timelineStartTime + TimeRange.minTime;
		}

		/// <summary>
		/// Truncate the _TimeRange so that the track ends at <paramref name="_GlobalEndTime"/> 
		/// </summary>
		/// <remarks>
		/// If <paramref name="_GlobalEndTime"/> is goes behond the duration of the _Clip. <see cref="TimeRange"/>.maxTime will be set to the _Clip duration
		/// </remarks>
		/// <param name="_GlobalEndTime">Time in sampler context (global time) at which the track will end</param>
		public void TruncateEnd(float _GlobalEndTime)
		{
			float maxTime = _GlobalEndTime - timelineStartTime;
			_timeRange.maxTime = Mathf.Clamp(maxTime, 0, Clip.clip.Duration);
		}

		/// <summary>
		/// Truncate the _TimeRange so that the track ends at <paramref name="_GlobalStartTime"/> 
		/// </summary>
		/// <remarks>
		/// If <paramref name="_GlobalStartTime"/> is goes before the duration of the _Clip. <see cref="TimeRange"/>.min will be set to 0
		/// </remarks>
		/// <param name="_GlobalStartTime">Time in sampler context (global time) at which the track will end</param>
		public void TruncateStart(float _GlobalStartTime)
		{
			float minTime = _GlobalStartTime - timelineStartTime;
			_timeRange.minTime = Mathf.Max(0, minTime);
		}

		/// <summary>
		/// Check if global time (sampler) is contained within the track<br/>
		/// <code>
		///                         <paramref name="globalTime"/><br/>
		///                             |<br/>
		///                         |---------------------------------|<br/>
		/// <see cref="timelineStartTime"/>        <see cref="GlobalEndTime"/>
		/// </code>
		/// </summary>
		/// <param name="globalTime"></param>
		/// <returns></returns>
		public bool ContainsGlobalTime(float globalTime)
		{
			return timelineStartTime <= globalTime && globalTime <= GlobalEndTime;
		}

		/// <summary>
		/// The overlap range between this track and <paramref name="_OtherTrack"/>
		/// <code>
		///                         |RETURNS|
		/// track 1 |---------------........|<br/>
		///                 track 2 |........-------------------------|<br/>
		/// </code>
		/// </summary>
		/// <param name="_OtherTrack">The _Other track to test</param>
		/// <returns>Returns null if there is no overlap. Returns a range in global (sampler) context otherwise</returns>
		public AnimationTimeRange? GetOverlap(KinetixClipTrack _OtherTrack)
		{
			AnimationTimeRange toReturn = new AnimationTimeRange(0, 0);

			(bool isThis, float time)[] overlapCheckArray = new (bool, float)[4]
			{
				(true, timelineStartTime),
				(true, GlobalEndTime),
				(false, _OtherTrack.timelineStartTime),
				(false, _OtherTrack.GlobalEndTime)
			};

			var enumerator = overlapCheckArray.OrderBy(o => o.time).GetEnumerator();
			enumerator.MoveNext();

			bool aBool = enumerator.Current.isThis;
			enumerator.MoveNext();

			bool bBool = enumerator.Current.isThis;
			toReturn.minTime = enumerator.Current.time;
			enumerator.MoveNext();

			toReturn.maxTime = enumerator.Current.time;

			enumerator.Dispose();

			if (aBool == bBool && toReturn.minTime != toReturn.maxTime) //if (true,true,false,false) or (false,false,true,true) it means tracks aren't overlaping
				return null;

			return toReturn;
		}

		public void Dispose()
		{
			disposed = true;
		}

		public override bool Equals(object _Obj)
		{
			return Equals(_Obj as KinetixClipTrack);
		}

		public bool Equals(KinetixClipTrack _Other)
		{
			return !(_Other is null) &&
				   hash == _Other.hash;
		}

		public static bool operator ==(KinetixClipTrack _Left, KinetixClipTrack _Right)
		{
			if (_Left is null && _Right is null) 
				return true;

			if (_Left is null)
				return _Right.Equals(_Left);

			return _Left.Equals(_Right);
		}

		public static bool operator !=(KinetixClipTrack _Left, KinetixClipTrack _Right)
		{
			return !(_Left == _Right);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (int)hash;
			}
		}
	}
}
