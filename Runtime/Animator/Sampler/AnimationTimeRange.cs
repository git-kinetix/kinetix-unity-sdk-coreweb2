// // ----------------------------------------------------------------------------
// // <copyright file="KinetixClipTrack.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal;

namespace Kinetix
{
    public struct AnimationTimeRange
	{
		public static readonly AnimationTimeRange Default = new AnimationTimeRange(-1, -1);

		public float minTime;
		public float maxTime;
		public readonly float Range => maxTime - minTime;

		public AnimationTimeRange(float _MinTime = -1, float _MaxTime = -1)
		{
			this.minTime = _MinTime;
			this.maxTime = _MaxTime;
		}

	}
}
