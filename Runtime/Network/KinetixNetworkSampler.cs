// // ----------------------------------------------------------------------------
// // <copyright file="KinetixNetworkSampler.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;

namespace Kinetix.Internal
{
	/// <summary>
	/// Serialize and Deserialize animation data from a server.
	/// </summary>
	public class KinetixNetworkSampler
	{
        public event Action<KinetixNetworkDataRaw> OnSerializeData;
		public event Action<KinetixNetworkData> OnDeserializeData;

		//Remote
		private string currentAnimationId = null; 
		private long currentTimestamp = 0;

		//Local
		private const float FRAME_SEND_RATE = 0.3f;
		private float lastSendTime = 0;

		public void OnPlayEnd()
		{
			OnSerializeData?.Invoke(new KinetixNetworkDataRaw(DateTime.Now.Ticks, null, -1));
		}

		public void OnPlayStart(AnimationIds id)
		{
			OnSerializeData?.Invoke(new KinetixNetworkDataRaw(DateTime.Now.Ticks, id.UUID, 0));
		}

		public void OnPlayedFrame(AnimationIds id, float time)
		{
			if (lastSendTime + 0.3f > time)
				return;

			lastSendTime = time;
			OnSerializeData?.Invoke(new KinetixNetworkDataRaw(DateTime.Now.Ticks, id.UUID, time));
		}

		public void RecieveData(KinetixNetworkDataRaw serverData)
		{
			if (serverData.timestamp < currentTimestamp)
				return;

			currentTimestamp = serverData.timestamp;

			// Lag calculation, idk how to use is so I leave it here
			/*
			DateTime now = DateTime.Now;
			TimeSpan timeSpan = new TimeSpan(serverData.timestamp - now.Ticks);
			*/

			KinetixNetworkData data = new KinetixNetworkData()
			{
				id = serverData.animationId == null ? null : new AnimationIds(serverData.animationId),
				time = serverData.animationTime
			};

			if (serverData.animationId == null)
				data.action = KinetixNetworkData.Action.Stop;
			else if (serverData.animationId == currentAnimationId)
				data.action = KinetixNetworkData.Action.MoveToTime;
			else
			{
				data.action = KinetixNetworkData.Action.Start;
				currentAnimationId = serverData.animationId;
			}

            OnDeserializeData?.Invoke(
                data
            );
		}
	}
}
