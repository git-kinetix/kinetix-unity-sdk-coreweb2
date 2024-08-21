// // ----------------------------------------------------------------------------
// // <copyright file="IKEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------
using Kinetix.Internal.Retargeting.IK;
using Kinetix.Internal.Retargeting.Utils;
using Kinetix.Internal.Utils;
using Kinetix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix
{
	public struct IKInfo
	{
		public IKTransformInfo root;
		public IKTransformInfo hips;

		public IKTransformInfo leftHand;
		public IKTransformInfo leftLowerArm;
		public IKTransformInfo leftUpperArm;

		public IKTransformInfo rightHand;
		public IKTransformInfo rightLowerArm;
		public IKTransformInfo rightUpperArm;

		public IKTransformInfo leftFoot;
		public IKTransformInfo leftLowerLeg;
		public IKTransformInfo leftUpperLeg;

		public IKTransformInfo rightFoot;
		public IKTransformInfo rightLowerLeg;
		public IKTransformInfo rightUpperLeg;
	}

	public struct IKTransformInfo
	{
		public HumanBodyBones bone;

		internal Vector3 localPosition;
		internal Quaternion localRotation;
		internal Vector3 localScale;

		public Vector3 globalPosition;
		public Quaternion globalRotation;
		public Vector3 globalScale;
	}
}

namespace Kinetix.Internal
{
	public class IKEffect : IFrameEffect, IFrameEffectModify, ISamplerAuthority
	{
		public int Priority => -200;

		public SamplerAuthorityBridge Authority { get; set; }

		private SkeletonPool.PoolItem skeleton;
		private SkeletonPool.PoolItem skeletonTPose;
		private AvatarBoneTransform root;
		private AvatarBoneTransform hips;
		
		public IKEffectMember leftFoot  ;
		public IKEffectMember rightFoot ;
		public IKEffectMember leftHand  ;
		public IKEffectMember rightHand ;

		public delegate void DelegateBeforeIkEffect(IKInfo currentPose);
		public event DelegateBeforeIkEffect OnBeforeIkEffect;

		public bool IsEnabled { get; set; } = true;
		public void UnregisterAllEvents()
		{
			OnBeforeIkEffect = null;
		}

		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
			if (root == null || skeleton == null || IsEnabled == false || OnBeforeIkEffect == null)
				return;

			SetFrameToSkeleton(skeleton, _FinalFrame);

			KinetixPose pose = Authority.GetAvatarPos();
			TransformData infoBeforeSettingTheRoot = new TransformData() { position = root.position, rotation = root.rotation, scale = root.scale };
			if (pose.rootGlobal.HasValue)
			{
				if (pose.rootGlobal.Value.position.HasValue) root.position = pose.rootGlobal.Value.position.Value;
				if (pose.rootGlobal.Value.rotation.HasValue) root.rotation = pose.rootGlobal.Value.rotation.Value;
				if (pose.rootGlobal.Value.scale.HasValue)    root.scale    = pose.rootGlobal.Value.scale.Value;
			}
			
			IKTransformInfo rootTransformInfo = InfoFromBone(root);
			if (pose.root.HasValue)
			{
				if (pose.root.Value.position.HasValue) rootTransformInfo.localPosition = pose.root.Value.position.Value;
				if (pose.root.Value.rotation.HasValue) rootTransformInfo.localRotation = pose.root.Value.rotation.Value;
				if (pose.root.Value.scale.HasValue)    rootTransformInfo.localScale    = pose.root.Value.scale.Value;
			}

			// Call Callback //
			OnBeforeIkEffect?.Invoke(
				new IKInfo()
				{
					root = rootTransformInfo,
					hips = InfoFromBone(hips),

					leftHand     = InfoFromBone(leftHand.resolver.transforms[0]),
					leftLowerArm = InfoFromBone(leftHand.resolver.transforms[1]),
					leftUpperArm = InfoFromBone(leftHand.resolver.transforms[2]),

					rightHand     = InfoFromBone(rightHand.resolver.transforms[0]),
					rightLowerArm = InfoFromBone(rightHand.resolver.transforms[1]),
					rightUpperArm = InfoFromBone(rightHand.resolver.transforms[2]),

					leftFoot     = InfoFromBone(leftFoot.resolver.transforms[0]),
					leftLowerLeg = InfoFromBone(leftFoot.resolver.transforms[1]),
					leftUpperLeg = InfoFromBone(leftFoot.resolver.transforms[2]),

					rightFoot     = InfoFromBone(rightFoot.resolver.transforms[0]),
					rightLowerLeg = InfoFromBone(rightFoot.resolver.transforms[1]),
					rightUpperLeg = InfoFromBone(rightFoot.resolver.transforms[2]),
				}
			);

			root.position = infoBeforeSettingTheRoot.position.Value;
			root.rotation = infoBeforeSettingTheRoot.rotation.Value;
			root.scale    = infoBeforeSettingTheRoot.scale.Value;

#if DEV_KINETIX
			Debug.DrawLine( hips.localPosition, leftHand .position.GetValueOrDefault() , Color.cyan    );
			Debug.DrawLine( hips.localPosition, rightHand.position.GetValueOrDefault() , Color.magenta );
			Debug.DrawLine( hips.localPosition, leftFoot .position.GetValueOrDefault() , Color.blue    );
			Debug.DrawLine( hips.localPosition, rightFoot.position.GetValueOrDefault() , Color.red     );
#endif

			leftFoot.Update();
			rightFoot.Update();
			leftHand .Update();
			rightHand.Update();

			//Calculate hips position
			Vector3 hipsPos = hips.position;
			
			float hipsCount = 0;
			hipsPos = leftFoot.CalculateHipsPos(hipsPos, ref hipsCount)
					+ rightFoot.CalculateHipsPos(hipsPos, ref hipsCount)
					+ leftHand.CalculateHipsPos(hipsPos, ref hipsCount)
					+ rightHand.CalculateHipsPos(hipsPos, ref hipsCount);

			if (hipsCount > 0)
			{
				hipsPos /= hipsCount;

				hips.position = hipsPos;
			}

			DebugSkeleton();

			//Apply avatar positions to final frame
			AvatarBoneTransform forLoopTr;
			for (int i = _FinalFrame.humanTransforms.Count - 1; i >= 0; i--)
			{

				string humanName = UnityHumanUtils.HumanToString(_FinalFrame.bones[i]);
				string humanNameLower = humanName.ToLower();
				if (humanName == UnityHumanUtils.AVATAR_HIPS)
				{
					//Apply hips
					TransformData hipsPose = _FinalFrame.humanTransforms[i];
					if (hipsPose.rotation.HasValue) hipsPose.position = skeleton[humanName].localPosition;

					_FinalFrame.humanTransforms[i] = hipsPose;
					continue;
				}

				if (!humanNameLower.Contains("left") && !humanNameLower.Contains("right"))
					continue;

				forLoopTr = skeleton[humanName];

				TransformData poseValue = _FinalFrame.humanTransforms[i];
				if (poseValue.position.HasValue) poseValue.position = forLoopTr.localPosition;
				if (poseValue.rotation.HasValue) poseValue.rotation = forLoopTr.localRotation;
				if (poseValue.scale.HasValue)    poseValue.scale    = forLoopTr.localScale   ;

				_FinalFrame.humanTransforms[i] = poseValue;
			}
		}

		public void OnQueueStart()
		{
			skeleton?.Dispose();
			skeletonTPose?.Dispose();
			skeleton = Authority.GetAvatar();
			skeletonTPose = Authority.GetAvatar();

			hips  = skeleton[UnityHumanUtils.AVATAR_HIPS];
			AvatarBoneTransform hipsT = skeletonTPose[UnityHumanUtils.AVATAR_HIPS];

			root = (AvatarBoneTransform)hips.IterateParent().Last();
			
			leftFoot  = new IKEffectMember(root, new IK3BonesOptimisedResolver(hips, hipsT, skeleton[UnityHumanUtils.AVATAR_LEFT_FOOT ], skeleton[UnityHumanUtils.AVATAR_LEFT_UPPER_LEG ], skeletonTPose[UnityHumanUtils.AVATAR_LEFT_FOOT ], Vector3.forward));
			rightFoot = new IKEffectMember(root, new IK3BonesOptimisedResolver(hips, hipsT, skeleton[UnityHumanUtils.AVATAR_RIGHT_FOOT], skeleton[UnityHumanUtils.AVATAR_RIGHT_UPPER_LEG], skeletonTPose[UnityHumanUtils.AVATAR_RIGHT_FOOT], Vector3.forward));
			leftHand  = new IKEffectMember(root, new IK3BonesOptimisedResolver(hips, hipsT, skeleton[UnityHumanUtils.AVATAR_LEFT_HAND ], skeleton[UnityHumanUtils.AVATAR_LEFT_UPPER_ARM ], skeletonTPose[UnityHumanUtils.AVATAR_LEFT_HAND ], Vector3.back));
			rightHand = new IKEffectMember(root, new IK3BonesOptimisedResolver(hips, hipsT, skeleton[UnityHumanUtils.AVATAR_RIGHT_HAND], skeleton[UnityHumanUtils.AVATAR_RIGHT_UPPER_ARM], skeletonTPose[UnityHumanUtils.AVATAR_RIGHT_HAND], Vector3.back));
		}

		/// <inheritdoc/>
		public void OnQueueEnd()
		{
			skeleton?.Dispose();
			skeleton = null;
			skeletonTPose?.Dispose();
			skeletonTPose = null;
			root = null;
			leftFoot  = null;
			rightFoot = null;
			leftHand  = null;
			rightHand = null;
		}

		public void OnSoftStop(float _SoftDuration){}

		public void Update(){}

		//-------------------------------------------//
		//                   UTILS                   //
		//-------------------------------------------//
		private void SetFrameToSkeleton(SkeletonPool.PoolItem _SkeletonPool, KinetixFrame _FinalFrame)
		{
			AvatarBoneTransform forLoopTr;
			for (int i = _FinalFrame.humanTransforms.Count - 1; i >= 0; i--)
			{
				forLoopTr = _SkeletonPool[UnityHumanUtils.HumanToString(_FinalFrame.bones[i])];

				TransformData poseValue = _FinalFrame.humanTransforms[i];
				if (poseValue.position.HasValue) forLoopTr.localPosition = poseValue.position.Value;
				if (poseValue.rotation.HasValue) forLoopTr.localRotation = poseValue.rotation.Value;
				if (poseValue.scale.HasValue) forLoopTr.localScale = poseValue.scale.Value;
			}
		}


		private void DebugSkeleton()
		{
#if DEV_KINETIX
			float v = Authority.GetQueueElapsedTime() / Authority.GetQueueDuration();
			float skeletonMovement = 5 * Authority.GetQueueElapsedTime();

			AvatarBoneTransform leftFootTr = leftFoot.resolver.transforms.Last() as AvatarBoneTransform;
			AvatarBoneTransform leftFootDebug = AvatarBoneTransform.Clone(leftFootTr, out _);
			Vector3 leftFootDebugPos = leftFootTr.position;
			Quaternion leftFootDebugRot = leftFootTr.rotation;
			leftFootDebug.parent = null;
			leftFootDebug.localPosition = leftFootDebugPos;
			leftFootDebug.localRotation = leftFootDebugRot;

			AvatarBoneTransform rightFootTr = rightFoot.resolver.transforms.Last() as AvatarBoneTransform;
			AvatarBoneTransform rightFootDebug = AvatarBoneTransform.Clone(rightFootTr, out _);
			Vector3 rightFootDebugPos = rightFootTr.position;
			Quaternion rightFootDebugRot = rightFootTr.rotation;
			rightFootDebug.parent = null;
			rightFootDebug.localPosition = rightFootDebugPos;
			rightFootDebug.localRotation = rightFootDebugRot;

			AvatarBoneTransform leftHandTr = leftHand.resolver.transforms.Last() as AvatarBoneTransform;
			AvatarBoneTransform leftHandDebug = AvatarBoneTransform.Clone(leftHandTr, out _);
			Vector3 leftHandDebugPos = leftHandTr.position;
			Quaternion leftHandDebugRot = leftHandTr.rotation;
			leftHandDebug.parent = null;
			leftHandDebug.localPosition = leftHandDebugPos;
			leftHandDebug.localRotation = leftHandDebugRot;

			AvatarBoneTransform rightHandTr = rightHand.resolver.transforms.Last() as AvatarBoneTransform;
			AvatarBoneTransform rightHandDebug = AvatarBoneTransform.Clone(rightHandTr, out _);
			Vector3 rightHandDebugPos = rightHandTr.position;
			Quaternion rightHandDebugRot = rightHandTr.rotation;
			rightHandDebug.parent = null;
			rightHandDebug.localPosition = rightHandDebugPos;
			rightHandDebug.localRotation = rightHandDebugRot;

			SkeletonDebugDraw.DrawSkeleton(leftFootDebug,  skeletonMovement * Vector3.back, Color.Lerp(new Color(0, 0, 1, 1), new Color(1, 0, 0, 1), v), duration: 10, scaleFactor: 1);
			SkeletonDebugDraw.DrawSkeleton(rightFootDebug, skeletonMovement * Vector3.back, Color.Lerp(new Color(0, 1, 1, 1), new Color(1, 1, 0, 1), v), duration: 10, scaleFactor: 1);
			SkeletonDebugDraw.DrawSkeleton(leftHandDebug,  skeletonMovement * Vector3.back, Color.Lerp(new Color(1, 0, 1, 1), new Color(0, 1, 0, 1), v), duration: 10, scaleFactor: 1);
			SkeletonDebugDraw.DrawSkeleton(rightHandDebug, skeletonMovement * Vector3.back, Color.Lerp(new Color(0, 0, 0, 1), new Color(1, 1, 1, 1), v), duration: 10, scaleFactor: 1);
#endif
		}

		private IKTransformInfo InfoFromBone(GenericBoneTransform _Bone)
		{
			HumanBodyBones human = (HumanBodyBones)(-1);
			if(_Bone is AvatarBoneTransform av)
			{
				human = av.m_humanBone ?? UnityHumanUtils.StringToHuman(av.humanBone);
			}

			return new IKTransformInfo()
			{
				bone = human,

				localPosition = _Bone.localPosition,
				localRotation = _Bone.localRotation,
				localScale    = _Bone.localScale,

				globalPosition = _Bone.position,
				globalRotation = _Bone.rotation,
				globalScale    = _Bone.scale,
			};
		}

	}
}
