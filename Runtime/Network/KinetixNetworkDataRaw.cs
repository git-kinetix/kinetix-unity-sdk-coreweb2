// // ----------------------------------------------------------------------------
// // <copyright file="KinetixNetworkDataRaw.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.IO;

namespace Kinetix.Internal
{

    [Serializable]
	public class KinetixNetworkDataRaw
	{
		public long timestamp;
		public string animationId;
		public float animationTime;

        public KinetixNetworkDataRaw(byte[] bytes)
        {
			MemoryStream stream = new MemoryStream(bytes);
			BinaryReader reader = new BinaryReader(stream);

			timestamp = reader.ReadInt64();
			if (reader.ReadBoolean())
				animationId = reader.ReadString();
			animationTime = reader.ReadSingle();

			reader.Dispose();
			stream.Dispose();
		}

        public KinetixNetworkDataRaw(long timestamp, string animationId, float animationTime)
        {
            this.timestamp = timestamp;
            this.animationId = animationId;
            this.animationTime = animationTime;
        }

        public byte[] ToBytes()
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write(timestamp);
			writer.Write(animationId != null);
			if (animationId != null) writer.Write(animationId);
			writer.Write(animationTime);


			byte[] array = stream.ToArray();

			writer.Dispose();
			stream.Dispose();

			return array;
		}
	}
}
