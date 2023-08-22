using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal.Utils
{
	public static class BlendshapeMeshUtils
	{
		public static IEnumerable<KeyValuePair<string, int>> GetBlendshapes(this Mesh mesh)
		{
			int count = mesh.blendShapeCount;
			for (int i = 0; i < count; i++)
			{
				yield return new KeyValuePair<string, int>(mesh.GetBlendShapeName(i), i);
			}
		}

		/// <summary>
		/// As opposed to a strict comparition, this method search if part of the blenshape contains the name.<br/>
		/// Provide multiple arguments for optimisation purposes
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="toTest"></param>
		/// <returns>Returns true if one of the blenshapes contains one of the provided <paramref name="toTest"/></returns>
		public static bool HasLossyBlendshape(this Mesh mesh, string[] toTest)
		{
			return mesh.GetBlendshapes().Any(_BlendAny);

			bool _BlendAny(KeyValuePair<string, int> blend)
			{
				return toTest.Any(_ToTestAny);
				bool _ToTestAny(string str)
				{
					return blend.Key.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1;
				}
			}
		}

		/// <summary>
		/// As opposed to a strict comparition, this method search if part of the blenshape contains the name.
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="toTest"></param>
		/// <returns>Returns true if one of the blenshapes contains the provided <paramref name="toTest"/></returns>
		public static bool HasLossyBlendshape(this Mesh mesh, string toTest, out KeyValuePair<string,int> resultIdName)
		{
			resultIdName = mesh.GetBlendshapes().FirstOrDefault(_BlendAny);
			return resultIdName.Key != null;

			bool _BlendAny(KeyValuePair<string, int> blend)
			{
				return blend.Key.IndexOf(toTest, StringComparison.OrdinalIgnoreCase) != -1;
			}
		}

		/// <summary>
		/// As opposed to a strict comparition, this method search if part of the blenshape contains the name.
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="toTest"></param>
		/// <returns>Returns true if one of the blenshapes contains the provided <paramref name="toTest"/></returns>
		public static bool HasLossyBlendshape(this Mesh mesh, string toTest)
		{
			return mesh.GetBlendshapes().Any(_BlendAny);

			bool _BlendAny(KeyValuePair<string, int> blend)
			{
				return blend.Key.IndexOf(toTest, StringComparison.OrdinalIgnoreCase) != -1;
			}
		}
	}
}
