using Kinetix.Internal.Kinanim;
using Kinetix.Internal.Kinanim.Utils;
using System;
using UnityEngine;
using Frame = Kinetix.Internal.Retargeting.AnimationData.RuntimeRetargetAnimationData.Frame;

namespace Kinetix.Internal.Retargeting.AnimationData
{
	public class KinanimDataIndexer : RuntimeRetargetFrameIndexer<KinanimImporter>
	{
		public KinanimDataIndexer(KinanimImporter dataSource) : base(dataSource) {}

		///<inheritdoc/>
		protected override void InternalInit()
		{
			dataTarget.FrameRate  = dataSource.Result.header.frameRate;
			dataTarget.FrameCount = dataSource.Result.header.FrameCount;
			dataTarget.hasBlendshape = dataSource.Result.header.hasBlendshapes;

			for (int j = 0; j < KinanimData.BLENDSHAPE_COUNT; j++)
			{
				dataTarget.blendshapesName[j] = ((ARKitBlendshapes)j).ToString();
			}

			SetDefaultFingerprint();
		}

		///<inheritdoc/>
		public override void UpdateIndexCount()
		{
			int frameCount = dataTarget.FrameCount;

			int maxUncompressedFrame = dataSource.compression?.MaxUncompressedFrame ?? dataSource.HighestImportedFrame;
			if (maxUncompressedFrame >= frameCount)
				maxUncompressedFrame = frameCount - 1;

			int maxLoadedFrame = dataTarget.MaxLoadedFrameId;
			int toLoad = maxUncompressedFrame - maxLoadedFrame;

			if (toLoad <= 0)
				return;

			dataTarget.DeclareTransformLoaded(maxUncompressedFrame);
			dataTarget.DeclareBlendshapeLoaded(maxUncompressedFrame);
			dataTarget.ManuallyCallOnLoadedFrames(toLoad);

		}

		///<inheritdoc/>
		protected override void IndexFrame(int frameIndex)
		{
			Frame frame = dataTarget.MakeFrameAt(frameIndex);
			KinanimData.FrameData frameData = dataSource.Result.content.frames[frameIndex];
			for (int j = 0; j < KinanimData.TRANSFORM_COUNT; j++)
			{
				frame.SetTransform((HumanBodyBones)Enum.Parse(typeof(HumanBodyBones), ((KinanimTransform)j).ToString()), frameData.transforms[j].ToSdkData());
			}

			if (frameData.blendshapes != null)
			{
				for (int j = 0; j < KinanimData.BLENDSHAPE_COUNT; j++)
				{
					frame.SetBlendshape((ARKitBlendshapes)Enum.Parse(typeof(ARKitBlendshapes), ((KinanimBlendshape)j).ToString()), frameData.blendshapes[j]);
				}
			}

			dataTarget.AddFrames(frame);
		}
	}
}
