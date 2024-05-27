// // ----------------------------------------------------------------------------
// // <copyright file="KinetixClipTrack.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix
{
    public enum AnimationLoopMode
	{
		/// <summary>
		/// The sampler will play the animation from t=A to t=B and destroy itself
		/// </summary>
		Default,
		/// <summary>
		/// The sampler will play the animation from t=A to t=B and loop back to t=A
		/// </summary>
		Loop
	}
}
