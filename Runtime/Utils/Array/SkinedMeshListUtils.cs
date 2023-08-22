using Kinetix.Internal.Utils;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal.Utils
{
	public static class SkinedMeshListUtils
	{
		private static readonly string[] TO_TEST = new string[]
		{
			ARKitBlendshapes.mouthClose  .ToString(),
			ARKitBlendshapes.mouthOpen   .ToString(),
			ARKitBlendshapes.eyeBlinkLeft.ToString(),
		};

		public static SkinnedMeshRenderer[] GetARKitRenderers(this SkinnedMeshRenderer[] renderers)
		{
			return renderers.Where(Where).ToArray();

			bool Where(SkinnedMeshRenderer s)
			{
				if (s.sharedMesh == null)
					return false;

				return s.sharedMesh.blendShapeCount != 0 && s.sharedMesh.HasLossyBlendshape(TO_TEST);
			}
		}
	}
}
