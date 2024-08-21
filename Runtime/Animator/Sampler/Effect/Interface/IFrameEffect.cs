// // ----------------------------------------------------------------------------
// // <copyright file="IFrameEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Kinetix.Internal
{
    public interface IFrameEffect
	{
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Priority of the effect. Highest priority will be updated first (constant)
		/// </summary>
		public int Priority { get; }

		/// <summary>
		/// Event sent when we're going from 0 clip to 1 clip playing
		/// </summary>
		public void OnQueueStart    ();
		/// <summary>
		/// Event sent when we're going from 1 clip to 0 clip playing
		/// </summary>
		public void OnQueueEnd      ();
		/// <summary>
		/// Update method of unity use it for any needed purpose
		/// </summary>
		public void Update          ();

		public void OnSoftStop(float softDuration);
	}
}
