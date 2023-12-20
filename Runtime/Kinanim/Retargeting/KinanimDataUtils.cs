using Kinetix.GLTFUtility;
using Kinetix.Internal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal.Kinanim.Utils
{
	internal static class KinanimDataUtils
	{
		public static TransformData ToSdkData(this KinanimData.TransformData transform)
		{
			TransformData clipData = new TransformData();

			if (transform.rotation.HasValue)
			{
				clipData.rotation = new Quaternion(
					transform.rotation.Value.x,
					-transform.rotation.Value.y,
					-transform.rotation.Value.z,
					transform.rotation.Value.w
				);
			}
			if (transform.position.HasValue)
			{
				clipData.position = new Vector3(
					-transform.position.Value.x,
					transform.position.Value.y,
					transform.position.Value.z
				);
			}
			if (transform.scale.HasValue)
			{
				clipData.scale = new Vector3(
					transform.scale.Value.x,
					transform.scale.Value.y,
					transform.scale.Value.z
				);
			}

			return clipData;
		}
	}
}
