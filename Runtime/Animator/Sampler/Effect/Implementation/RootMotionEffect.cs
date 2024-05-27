// // ----------------------------------------------------------------------------
// // <copyright file="RootMotionEffect.cs" company="Kinetix SAS">
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
	/// Effect that transfers the hips position to the root
	/// </summary>
	public class RootMotionEffect : IFrameEffectModify, ISamplerAuthority, IFrameEffectAdd
	{
		public RootMotionConfig config = new RootMotionConfig()
		{
			ApplyHipsYPos     = false,
			ApplyHipsXAndZPos = false,
			BackToInitialPose = false,
			BakeIntoPoseXZ    = false,
			BakeIntoPoseY     = false
		};

		public int Priority => -100;

		private int countAnime = 0;
		private bool isEnabled;
		
		private int hipsIndexStartPos;
		
		private Vector3 hipsOriginalPosition;
		private Vector3 rootOriginalPosition;

		private Vector3 calculatedRootPosition = Vector3.zero;
		private Vector3 lastHipsPosition = Vector3.zero;

		private AvatarBoneTransform hips;
		private AvatarBoneTransform root;
		
		private SkeletonPool.PoolItem skeleton;
		private KinetixPose startPos;

		public event Action<KinetixFrame> OnAddFrame;

		public RootMotionEffect(RootMotionConfig _Config)
		{
			if (_Config != null)
				this.config = _Config;
		}

		public SamplerAuthorityBridge Authority { get; set; }

		/// <inheritdoc/>
		public void OnAnimationStart(KinetixClip _)
		{
			if (++countAnime > 1 && isEnabled) RevertToOffsets();
			
			startPos = Authority.GetAvatarPos();
			if (startPos == null)
			{
				isEnabled = false;
				return;
			}

			hipsIndexStartPos = Array.IndexOf(startPos.bones, HumanBodyBones.Hips);
			hips = skeleton[UnityHumanUtils.AVATAR_HIPS];
			root = (AvatarBoneTransform)hips.IterateParent().Last();
			isEnabled = true;

			//Set hips to pos
			TransformData transform = startPos.humanTransforms[hipsIndexStartPos];
			TrDataToTrAvatar(transform, hips);

			//Set root to pos
			if (startPos.root.HasValue) TrDataToTrAvatar(startPos.root.Value, root);

			//Init root motion
			SaveOffsets();
		}

		/// <inheritdoc/>
		public void OnAnimationEnd()
		{
			if (--countAnime == 0 && isEnabled)
				RevertToOffsets();
		}


		/// <inheritdoc/>
		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
        {
			if (!isEnabled) return;
			int hipsIndexCurrent = Array.IndexOf(_FinalFrame.bones, HumanBodyBones.Hips);
			TransformData transform = _FinalFrame.humanTransforms[hipsIndexCurrent];
			
			//Set hips from curve
			TrDataToTrAvatar(transform, hips);
			
			ProcessRootMotionAfterAnimSampling();
			
			//Set curve from hips
			transform = TrAvatarToTrData(transform, hips);
			
			//Apply & set curve from root
			_FinalFrame.humanTransforms[hipsIndexCurrent] = transform;
			_FinalFrame.root = TrAvatarToTrData(new TransformData() { position = Vector3.zero}, root);
		}

		private TransformData TrAvatarToTrData(TransformData _Original, AvatarBoneTransform _Transform)
		{
			if (_Original.position.HasValue) _Original.position = _Transform.localPosition;
			if (_Original.rotation.HasValue) _Original.rotation = _Transform.localRotation;
			if (_Original.scale.HasValue   ) _Original.scale    = _Transform.localScale;
			return _Original;
		}

		private void TrDataToTrAvatar(TransformData _Data, AvatarBoneTransform _Transform)
		{
			if (_Data.position.HasValue) _Transform.localPosition = _Data.position.Value;
			if (_Data.rotation.HasValue) _Transform.localRotation = _Data.rotation.Value;
			if (_Data.scale.HasValue   ) _Transform.localScale    = _Data.scale.Value;
		}

		/// <inheritdoc/>
		public void OnQueueStart()
		{
			skeleton?.Dispose();
			skeleton = Authority.GetAvatar();
		}


		/// <inheritdoc/>
		public void OnQueueEnd() 
		{
			isEnabled = false;
			skeleton?.Dispose();
			skeleton = null;
		}

		/// <inheritdoc/>
		public void Update() {}

		private void SaveOffsets()
		{
			hipsOriginalPosition = hips.parent.localRotation * hips.localPosition;
			lastHipsPosition = hipsOriginalPosition;

			rootOriginalPosition = root.position;
		}

		private void ProcessRootMotionAfterAnimSampling()
		{
			Quaternion armatureRotation = hips.parent.localRotation;

			// Handling root post-processing
			Vector3 newRootPosition = Vector3.zero;
			calculatedRootPosition = root.localRotation * ((armatureRotation * hips.localPosition) - lastHipsPosition);

			if (!config.BakeIntoPoseXZ && config.ApplyHipsXAndZPos)
			{
				newRootPosition.x = calculatedRootPosition.x;
				newRootPosition.z = calculatedRootPosition.z;
			}

			if (!config.BakeIntoPoseY && config.ApplyHipsYPos)
			{
				newRootPosition.y = calculatedRootPosition.y;
			}

			root.localPosition += newRootPosition;

			lastHipsPosition = armatureRotation * hips.localPosition;


			// Handling hips post-processing
			Vector3 newHipsPos = armatureRotation * hips.localPosition;


			if (config.ApplyHipsXAndZPos)
			{
				newHipsPos.x = hipsOriginalPosition.x;

				newHipsPos.z = hipsOriginalPosition.z;
			}

			if (config.ApplyHipsYPos)
			{
				newHipsPos.y = hipsOriginalPosition.y;
			}


			hips.localPosition = Quaternion.Inverse(armatureRotation) * newHipsPos;
		}

		private void RevertToOffsets()
		{
			// Save current hips position before moving the root
			Vector3 hipsPos = hips.position;


			// Revert root position
			Vector3 newRootPos = root.position;

			if (config.BackToInitialPose && config.ApplyHipsXAndZPos)
			{
				newRootPos.x = rootOriginalPosition.x;
				newRootPos.z = rootOriginalPosition.z;
			}

			if (config.ApplyHipsYPos)
			{
				newRootPos.y = rootOriginalPosition.y;
			}

			root.position = newRootPos;

			// Revert hips position
			Vector3 calcHipsPos = hipsPos;
			Vector3 newHipsPos = hips.position;

			if (config.BackToInitialPose && config.ApplyHipsXAndZPos)
			{
				newHipsPos.x = calcHipsPos.x;
				newHipsPos.z = calcHipsPos.z;
			}

			if (config.ApplyHipsYPos)
			{
				newHipsPos.y = calcHipsPos.y;
			}

			hips.position = hipsPos;
			lastHipsPosition = hips.localPosition;

			//Bridge back to avatar
			OnAddFrame?.Invoke(
				new KinetixFrame(
					new TransformData[1] { TrAvatarToTrData(startPos.humanTransforms[hipsIndexStartPos], hips) }, 
					new HumanBodyBones[1] { HumanBodyBones.Hips },
					null
				)
				{
					root = TrAvatarToTrData(startPos.root ?? TransformData.Default, root)
				}
			);
		}

        public void OnSoftStop(float _)
        {
        }
    }
}
