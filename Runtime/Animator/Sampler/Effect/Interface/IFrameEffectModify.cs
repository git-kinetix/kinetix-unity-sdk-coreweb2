// // ----------------------------------------------------------------------------
// // <copyright file="IFrameEffectModify.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix.Internal
{
	/// <summary>
	/// Inherit this interface to create an effect that can modify frame before sending
	/// </summary>
	public interface IFrameEffectModify : IFrameEffect
	{
		/// <summary>
		/// Event sent when a frame has been played
		/// </summary>
		/// <param name="_FinalFrame">The cloned frame. You can modify informations in it</param>
		/// <param name="_Frames">Original _Frames. A null frame in the array means that the frame is the same as the previously sent</param>
		/// <param name="baseFrameIndex">Index on which <paramref name="_FinalFrame"/> is based</param>
		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks);
	}
}
