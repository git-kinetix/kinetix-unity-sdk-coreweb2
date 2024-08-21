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
	public class KinetixMaskEffect : IFrameEffect, IFrameEffectModify
	{
		public int Priority => -150;
		public KinetixMask mask;

		public bool IsEnabled { get; set; } = true;
		public void OnPlayedFrame(ref KinetixFrame _FinalFrame, KinetixFrame[] _Frames, in KinetixClipTrack[] _Tracks)
		{
			if (mask == null || !IsEnabled)
				return;

			List<TransformData> tr = _FinalFrame.humanTransforms;
			List<HumanBodyBones> bones = _FinalFrame.bones;
			for (int i = _FinalFrame.bones.Count - 1; i >= 0; i--)
			{
				if (!mask.IsEnabled(bones[i]))
				{
					bones.RemoveAt(i);
					tr.RemoveAt(i);
				}
			}
		}

		public void OnQueueEnd(){}

		public void OnQueueStart(){}

		public void OnSoftStop(float softDuration){}

		public void Update(){}
	}
}
