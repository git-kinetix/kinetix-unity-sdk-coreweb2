// // ----------------------------------------------------------------------------
// // <copyright file="SamplerAuthorityBridge.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal.Utils;
using System.Collections.Generic;

namespace Kinetix.Internal
{
	/// <summary>
	/// Authority on the tracks.<br/>
	/// You can call some methods like "StartNextClip" or "GetAvatarPos".
	/// </summary>     
	public class SamplerAuthorityBridge
	{
		/// <returns>Returns the direction the queue</returns>
		public   delegate AnimationLoopMode GetQueueDirectionDelegate();
		/// <returns>Returns the play rate of the sampler</returns>
		public   delegate float GetPlayRateDelegate();
		/// <returns>Returns the elapsed time of the queue</returns>
		public   delegate float GetQueueElapsedTimeDelegate();
		/// <returns>Returns the duration of the queue</returns>
		public   delegate float GetQueueDurationDelegate();
		/// <returns>Returns the current pose of the avatar without the kinetix animation</returns>
		public   delegate KinetixPose GetAvatarPosDelegate();
		/// <param name="_Index">Sampler _Index</param>
		/// <returns>Returns the clip currently playing on the sampler n° <paramref name="_Index"/></returns>
		public   delegate KinetixClip GetClipDelegate(int _Index);
		/// <returns>Returns a pool item with custom transforms of the avatar in TPose</returns>
		internal delegate SkeletonPool.PoolItem GetAvatarDelegate();

		/// <summary>
		/// Get the direction the queue
		/// </summary>
		public GetQueueDirectionDelegate   GetQueueDirection;
		/// <summary>
		/// Get the play rate of the sampler
		/// </summary>
		public GetPlayRateDelegate GetPlayRate;
		/// <summary>
		/// Get the elapsed time of the queue
		/// </summary>
		public GetQueueElapsedTimeDelegate GetQueueElapsedTime;
		/// <summary>
		/// Get the duration of the queue
		/// </summary>
		public GetQueueDurationDelegate    GetQueueDuration;

		/// <summary>
		/// Get the current pose of the avatar without the kinetix animation
		/// </summary>
		public GetAvatarPosDelegate  GetAvatarPos;
		/// <summary>
		/// Get the TPose transform hierarchy of the avatar
		/// </summary>
		/// <remarks>
		/// Note: The pool item must be disposed after use
		/// </remarks>
		internal GetAvatarDelegate     GetAvatar;
	}
}
