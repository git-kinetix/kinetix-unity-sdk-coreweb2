// // ----------------------------------------------------------------------------
// // <copyright file="TransformPoseInterpreter.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal.Retargeting;
using Kinetix.Internal.Utils;
using Kinetix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kinetix.Internal
{
	/// <summary>
	/// A pose interpreter for Avatars using unity's transform system
	/// </summary>
	public class TransformPoseInterpreter : IPoseInterpreter, IPoseInterpreterStartEnd
	{
		private readonly struct BlendshapeMapData
		{
			public readonly int rendererId;
			public readonly int blendshapeId;
			public readonly float min;
			public readonly float max;

			public BlendshapeMapData(int rendererId, int blendshapeId, float min, float max)
			{
				this.rendererId = rendererId;
				this.blendshapeId = blendshapeId;
				this.min = min;
				this.max = max;
			}
		}

		private readonly GameObject root;
		private readonly GameObject armature;
		private readonly Dictionary<HumanBodyBones, GameObject> boneMap = new Dictionary<HumanBodyBones, GameObject>();
		private readonly SkinnedMeshRenderer[] skinnedMeshRenderer;
		private readonly Dictionary<ARKitBlendshapes, List<BlendshapeMapData>> blendshapeMap = new Dictionary<ARKitBlendshapes, List<BlendshapeMapData>>();
		protected KinetixClip clip;


		public TransformPoseInterpreter(GameObject root, Avatar avatar, SkinnedMeshRenderer[] skinnedMeshRenderer = null)
		{
			this.root = root;

			if (skinnedMeshRenderer != null)
			{
				this.skinnedMeshRenderer = skinnedMeshRenderer.Length == 0 ? null : skinnedMeshRenderer;

				int length = skinnedMeshRenderer.Length;
				for (int i = length-1; i >= 0; i--)
				{
					for (ARKitBlendshapes blend = 0; blend < ARKitBlendshapes.Count; blend++)
					{
						Mesh sharedMesh = skinnedMeshRenderer[i].sharedMesh;
						if (sharedMesh.HasLossyBlendshape(blend.ToString(), out KeyValuePair<string, int> resultIdName))
						{
							if (!blendshapeMap.ContainsKey(blend))
								blendshapeMap[blend] = new List<BlendshapeMapData>();

							float min = 0f, 
								  max = 0f;

							int frameCount = sharedMesh.GetBlendShapeFrameCount(resultIdName.Value);
							for (int j = 0; j < frameCount; j++)
							{
								float frameWeight = sharedMesh.GetBlendShapeFrameWeight(resultIdName.Value, j);
								min = Mathf.Min(frameWeight, min);
								max = Mathf.Max(frameWeight, max);
							}

							blendshapeMap[blend].Add(new BlendshapeMapData(i, resultIdName.Value, min, max));
						}
						
					}
				}
			}

			AvatarRetargetTable table = RetargetTableCache.GetTableSync(new AvatarData(avatar, root.transform));
			int count = UnityHumanUtils.HUMANS.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					string name = UnityHumanUtils.HUMANS[i];

					Transform boneTr = root.transform.Find(table.m_boneMapping[name]);
					boneMap[ (HumanBodyBones)Enum.Parse(typeof(HumanBodyBones), name.Replace(" ","") ) ] = (boneTr == null ? null : boneTr.gameObject);
				}
				catch (Exception) {}
			}

			armature = boneMap[HumanBodyBones.Hips].transform.parent.gameObject;
			armature = armature == root ? null : armature;
		}

		///<inheritdoc/>
		public virtual void AnimationStart(KinetixClip clip)
		{
			this.clip = clip;
		}

		public virtual void AnimationEnd(KinetixClip clip)
		{
		}

		public virtual void QueueStart()
		{

		}

		public virtual void QueueEnd()
		{
			clip = null;
		}


		///<inheritdoc/>
		public virtual void ApplyResetPose(string bonePath, TransformData pose)
		{
			Transform tr = root.transform.Find(bonePath);
			if (tr)
			{
				ApplyDataToTransform(pose, tr);
			}
		}

		///<inheritdoc/>
		public virtual void ApplyBone(HumanBodyBones bone, TransformData pose)
		{
			if (boneMap.TryGetValue(bone, out GameObject go) && go)
			{
				Transform tr = go.transform;
				ApplyDataToTransform(pose, tr);
			}
		}

		public virtual void ApplyOther(HumanSpecialBones bone, TransformData pose)
		{
			switch (bone)
			{
				case HumanSpecialBones.Root:
					ApplyDataToTransform(pose, root.transform);
					break;
				case HumanSpecialBones.Armature:
					Transform armature = boneMap[HumanBodyBones.Hips].transform.parent;
					ApplyDataToTransform(pose, armature);
					break;
				default:
					break;
			}
		}

		public virtual void ApplyBlendshape(ARKitBlendshapes blendshape, float pose)
		{
			if (skinnedMeshRenderer != null)
			{
				List<BlendshapeMapData> list = blendshapeMap.ValueOrDefault(blendshape);

				if (list == null)
					return;

				SkinnedMeshRenderer skm;
				BlendshapeMapData data;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					data = list[i];
					skm = skinnedMeshRenderer[data.rendererId];
					if (skm == null)
						continue;

					skm.SetBlendShapeWeight(data.blendshapeId, Mathf.Lerp(data.min, data.max, pose));
				}
			}
		}

		public virtual string GetArmature()
		{
			if (armature == null)
				return null;

			string path = armature.name;
			Transform parent = armature.transform.parent;
			while (parent != root.transform && parent != null)
			{
				path = parent.name + "/" + path;
				parent = parent.parent;
			}

			return path;
		}

		///<inheritdoc/>
		public virtual KinetixPose GetPose()
		{
			HumanBodyBones[] bones = boneMap.Keys.ToArray();
			int length = bones.Length;

			TransformData[] trs = new TransformData[length];

			for (int i = 0; i < length; i++)
			{
				HumanBodyBones b = bones[i];
				if (boneMap.TryGetValue(b, out GameObject go) && go)
				{
					Transform transform = go.transform;
					trs[i] = TransformToData(transform);
				}
			}

			TransformData rootTrData = TransformToData(root.transform);

			TransformData? armatureTrData = armature == null ? default(TransformData?) : TransformToData(armature.transform);

			SkinnedMeshRenderer skinnedMeshRenderer = this.skinnedMeshRenderer?.FirstOrDefault();
			return new KinetixPose(trs, bones, skinnedMeshRenderer == null ? null : new float[(int)ARKitBlendshapes.Count].Select(SelectBlendshapes), rootTrData, armatureTrData);

			float SelectBlendshapes(float arg1, int arg2)
				=> skinnedMeshRenderer.GetBlendShapeWeight(arg2);
		}

		private static void ApplyDataToTransform(TransformData pose, Transform tr)
		{
			if (pose.position.HasValue) tr.localPosition = pose.position.Value;
			if (pose.rotation.HasValue) tr.localRotation = pose.rotation.Value;
			if (pose.scale.HasValue) tr.localScale = pose.scale.Value;
		}

		private static TransformData TransformToData(Transform transform) => new TransformData()
		{
			position = transform.localPosition,
			rotation = transform.localRotation,
			scale = transform.localScale
		};

	}
}
