// // ----------------------------------------------------------------------------
// // <copyright file="ABlending.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
    /// <summary>
    /// Abstract blending class for <see cref="IFrameEffect"/>
    /// </summary>
    public abstract class ABlending : ISamplerAuthority
	{
		/// <inheritdoc/>
		public SamplerAuthorityBridge Authority { get; set; }

		/// <summary>
		/// Overwrite the pose of <paramref name="_Overwrite"/> replacing it by the average of <paramref name="_ToBlend"/> by their respective <paramref name="_Weight"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="_Overwrite">Result pose</param>
		/// <param name="_ToBlend">List of animations to blend with <paramref name="_Overwrite"/></param>
		/// <param name="_Weight"></param>
		public void Average<T, T2>(ref T _Overwrite, in T2[] _ToBlend, float[] _Weight)
			where T : KinetixPose
			where T2 : KinetixPose
		{
			int blendCount = _ToBlend.Length;
			int aLenght = _Overwrite.bones.Length;

			if (_Weight.Sum() <= 0)
				return;

			for (int i = 0; i < (int)ARKitBlendshapes.Count; i++)
			{
				ARKitBlendshapes blendshape = (ARKitBlendshapes)i;
				float blend  = 0,
					  pCount = 0;
				for (int j = 0; j < blendCount; j++)
				{
					blend += _ToBlend[j].blendshapes[blendshape] * _Weight[j];
					pCount += _Weight[j];
				}

				_Overwrite.blendshapes[blendshape] = blend / pCount;
			}

			//Blend human
			//A refer to the original pose
			//B refer to the pose to blend
			for (int i = 0; i < aLenght; i++)
			{
				HumanBodyBones bone = _Overwrite.bones[i];
				TransformData trA = _Overwrite.humanTransforms[i];

				Vector3 pos    = Vector3.zero;
				Quaternion rot = new Quaternion(0,0,0,0);
				Vector3 scale  = Vector3.zero;

				float pCount = 0,
					rCount = 0,
					sCount = 0;

				//Get the average of each B
				for (int j = 0; j < blendCount; j++)
				{
					KinetixPose b = _ToBlend[j];
					if (b == null)
						continue;

					int bIndex = Array.IndexOf(b.bones, bone);
					if (bIndex == -1)
						continue;

					TransformData trB = b.humanTransforms[bIndex];

					BlendAddWithWeight(ref pos, ref rot, ref scale, ref pCount, ref rCount, ref sCount, trB, _Weight[j]);
				}

				//Do the average
				if (trA.position != null) trA.position = pCount == 0 ? trA.position : pos / pCount;
				if (trA.rotation != null) trA.rotation = rCount == 0 ? trA.rotation : QNormalizeSafe(rot);
				if (trA.scale    != null) trA.scale    = sCount == 0 ? trA.scale    : scale / sCount;

				_Overwrite.humanTransforms[i] = trA;
			}

			//Blend armature
			if (_Overwrite.armature.HasValue)
			{
				TransformData trA = _Overwrite.armature.Value;

				Vector3 pos = Vector3.zero;
				Quaternion rot = Quaternion.identity;
				Vector3 scale = Vector3.zero;

				float pCount = 0,
					rCount = 0,
					sCount = 0;

				for (int j = 0; j < blendCount; j++)
				{
					KinetixPose b = _ToBlend[j];
					if (b == null)
						continue;

					if (b.armature == null)
						continue;

					TransformData trB = b.armature.Value;

					BlendAddWithWeight(ref pos, ref rot, ref scale, ref pCount, ref rCount, ref sCount, trB, _Weight[j]);
				}

				//Do the average
				if (trA.position != null) trA.position = pCount == 0 ? trA.position : pos / pCount;
				if (trA.rotation != null) trA.rotation = rCount == 0 ? trA.rotation : QNormalizeSafe(rot);
				if (trA.scale != null) trA.scale = sCount == 0 ? trA.scale : scale / sCount;

				_Overwrite.armature = trA;
            }
        }

		/// <summary>
		/// Add <paramref name="trB"/> to sum calculation
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="rot"></param>
		/// <param name="scale"></param>
		/// <param name="pCount"></param>
		/// <param name="rCount"></param>
		/// <param name="sCount"></param>
		/// <param name="trB"></param>
		private static void BlendAddWithWeight(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale, ref float pCount, ref float rCount, ref float sCount, TransformData trB, float weight)
		{
			//NOTE: Parameters are named after the local variables in 'Average' method

			if (float.IsInfinity(weight) || weight == 0)
				return;

			if (trB.rotation.HasValue)
			{
				Quaternion rotB = trB.rotation.GetValueOrDefault(Quaternion.identity);
				if (rCount == 0)
					rot = QScale(rotB, weight);
				else
					rot = QBlend(rot, rotB, weight);
			}

			pos   += trB.position.GetValueOrDefault(Vector3.zero) * weight;
			scale += trB.scale.GetValueOrDefault(Vector3.zero) * weight;

			pCount += trB.position.HasValue ? weight : 0;
			rCount += trB.rotation.HasValue ? weight : 0;
			sCount += trB.scale.HasValue    ? weight : 0;
		}

		const float MIN_DOT_SELF = 1e-10f;

		protected static Quaternion QScale(Quaternion _Q, float _Scale)
			=> new Quaternion(
					_Q.x * _Scale, 
					_Q.y * _Scale, 
					_Q.z * _Scale, 
					_Q.w * _Scale
				);

		protected static Quaternion QNormalizeSafe(Quaternion _Q)
		{
			float dot = Quaternion.Dot(_Q, _Q);
			if (dot > MIN_DOT_SELF)
			{
				float invLenght = 1f / Mathf.Sqrt(dot);
				return new Quaternion(
					_Q.x * invLenght,
					_Q.y * invLenght,
					_Q.z * invLenght,
					_Q.w * invLenght
				);
			}

			return Quaternion.identity;
		}

		protected static Quaternion QBlend(Quaternion _A, Quaternion _B, float _Weight)
			=> QAdd(_A, QScale(_B,_Weight));
		
		protected static Quaternion QAdd(Quaternion _RefQuat, Quaternion _ToAdd)
		{
			float sign = Mathf.Sign(Quaternion.Dot(_RefQuat, _ToAdd));
			return new Quaternion(
				_RefQuat.x + sign * _ToAdd.x,
				_RefQuat.y + sign * _ToAdd.y,
				_RefQuat.z + sign * _ToAdd.z,
				_RefQuat.w + sign * _ToAdd.w
			);
		}
	}

}
