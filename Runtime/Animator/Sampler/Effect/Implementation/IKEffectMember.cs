// // ----------------------------------------------------------------------------
// // <copyright file="IKEffectMember.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------
using Kinetix.Internal.Retargeting.IK;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
	/// <summary>
	/// Use by the <see cref="IKEffect"/> to create 1 IK (and its settings) by member (left hand / right hand / left foot / right foot)
	/// </summary>
	public class IKEffectMember
	{
		public float targetRotationWeight = 1;
		public float targetPositionWeight = 1;
		public bool clampHips = false;

		internal Vector3? position;
		internal Quaternion? endBoneRotation;
		internal AvatarBoneTransform root;
		internal IK3BonesOptimisedResolver resolver;

		internal IKEffectMember(AvatarBoneTransform root, IK3BonesOptimisedResolver resolver)
		{
			this.root = root;
			this.resolver = resolver;
			SetPoleVector(Vector3.zero); //Set default value
		}

		public Vector3 CalculateHipsPos(Vector3 hipsPos, ref float hipsSum)
		{
			if (clampHips == false || !position.HasValue || targetPositionWeight == 0)
				return Vector3.zero;

			GenericBoneTransform endEffector   = resolver.transforms[0];     //End  effector
			GenericBoneTransform startEffector = resolver.transforms.Last(); //Start effector
			Vector3 startToEnd  = endEffector.position - startEffector.position;
			Vector3 endToTarget = (position.Value - endEffector.position); //Weight lerp is approximated

			++hipsSum;
			Vector3 startToTarget = startToEnd + endToTarget;
			if (startToTarget.sqrMagnitude < resolver.Size * resolver.Size)
				return hipsPos;
			
			startToTarget = Vector3.ClampMagnitude(startToTarget, resolver.Size);

			Vector3 startToHips = hipsPos - startEffector.position;
			return Vector3.Lerp(hipsPos, position.Value - startToTarget + startToHips, targetPositionWeight);
		}

		public void Update()
		{
			if ((!position.HasValue && !endBoneRotation.HasValue) || (targetRotationWeight == 0 && targetPositionWeight == 0))
				return;				  

			//Get animation rotation of the endbone
			Quaternion[] endBoneQ = resolver.transforms.Select(t => t.localRotation).ToArray();

			Vector3 target = position ?? resolver.transforms[0].position;

			resolver.endBoneRotation = targetRotationWeight == 0 ? null : endBoneRotation;
			resolver.Resolve(target);

			Quaternion endBoneGlobRot = default;
			//End bone lerp
			if (targetRotationWeight != 0 && endBoneRotation != null)
			{
				resolver.transforms[0].localRotation = Quaternion.Slerp(endBoneQ[0], resolver.transforms[0].localRotation, targetRotationWeight);
				endBoneGlobRot = resolver.transforms[0].rotation;
			}

			//End position lerp (= bones 1 and 2)
			for (int i = 1; i < endBoneQ.Length; i++)
			{
				resolver.transforms[i].localRotation = Quaternion.Slerp(endBoneQ[i], resolver.transforms[i].localRotation, targetPositionWeight);
			}

			//Fix end bone rotation
			if (targetRotationWeight != 0 && endBoneRotation != null)
			{
				resolver.transforms[0].rotation = endBoneGlobRot;
			}
		}

		public void UnsetEndBoneRotation()
		{
			endBoneRotation = null;
		}

		public void UnsetIK()
		{
			position = null;
		}

		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public void SetEndBoneRotation(GameObject _RootGameObject, Quaternion _Quaternion)
		{
			endBoneRotation = Quaternion.Inverse(_RootGameObject.transform.rotation) * _Quaternion;
		}

		public void SetEndBoneRotation(Quaternion _Quaternion)
		{
			endBoneRotation = _Quaternion;
		}

		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public void SetPoleVector(GameObject _RootGameObject, Vector3 _PoleVector)
		{
			SetPoleVector(_RootGameObject.transform.InverseTransformPoint(_PoleVector));
		}
		
		public void SetPoleVector(Vector3 _PoleVector)
		{
			resolver.PoleVector = _PoleVector;
		}

		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public void SetIKPosition(GameObject _RootGameObject, Vector3 _GlobalPosition)
		{
			position = _RootGameObject.transform.InverseTransformPoint(_GlobalPosition);
		}

		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public Quaternion GetEndBoneRotation(GameObject _RootGameObject)
		{
			if (!endBoneRotation.HasValue)
				return default;
			return _RootGameObject.transform.rotation * endBoneRotation.Value;
		}

		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public Vector3 GetPoleVector(GameObject _RootGameObject)
		{
			return _RootGameObject.transform.TransformPoint(resolver.PoleVector.Value);
		}
		
		/// <remarks>
		/// Doesn't apply rootmotion's start position
		/// </remarks>
		public Vector3 GetIKPosition(GameObject _RootGameObject)
		{
			if (!position.HasValue)
				return default;
			return _RootGameObject.transform.TransformPoint(position.Value);
		}

		/// <summary>
		/// Set IK position in the global context
		/// </summary>
		public void SetIKPosition(Vector3 _GlobalPosition)
		{
			position = _GlobalPosition;
		}
	}
}
