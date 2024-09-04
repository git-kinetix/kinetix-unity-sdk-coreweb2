// // ----------------------------------------------------------------------------
// // <copyright file="KinetixMaskEffect.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------
using Kinetix.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.Internal
{
	public class KinetixMaskEffect : IFrameEffect, IFrameEffectModify, ISamplerAuthority
	{
		public int Priority => -150;
		public KinetixMask mask;

		public bool IsEnabled { get; set; } = true;
        public SamplerAuthorityBridge Authority { get; set; }

        public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
			if (mask == null || !IsEnabled)
				return;

			KinetixPose pose = Authority.GetAvatarPos();

			List<TransformData>  poseTr    = pose.humanTransforms;
			List<HumanBodyBones> poseBones = pose.bones;

			List<TransformData> tr     = _FinalFrame.humanTransforms;
			List<HumanBodyBones> bones = _FinalFrame.bones;

			for (int i = _FinalFrame.bones.Count - 1; i >= 0; i--)
			{
                HumanBodyBones bone = bones[i];
                if (!mask.IsEnabled(bone))
				{
					int g = poseBones.IndexOf(bone);
					if (g < 0)
						continue;

					tr[i] = poseTr[g];
				}
			}
		}

		public void OnQueueEnd(){}

		public void OnQueueStart(){}

		public void OnSoftStop(float _SoftDuration){}

		public void Update(){}
	}
}
