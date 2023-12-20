// // ----------------------------------------------------------------------------
// // <copyright file="KinetixNetworkData.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix.Internal
{
    public class KinetixNetworkData
	{
		public enum Action
		{
			Start,
			Stop,
			MoveToTime
		}

		public Action action;

		public float time;
		public AnimationIds id;
	}
}
