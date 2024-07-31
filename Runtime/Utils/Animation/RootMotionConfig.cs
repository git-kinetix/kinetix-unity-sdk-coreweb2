// // ----------------------------------------------------------------------------
// // <copyright file="RootMotionConfig.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix
{
    public class RootMotionConfig
    {
        public bool ApplyHipsYPos = false;
        public bool ApplyHipsXAndZPos = false;
        public bool BackToInitialPose = false;
        public bool BakeIntoPoseXZ = false;
        public bool BakeIntoPoseY = false;

        // Use this is you scaled up of down your armature or a parent of the hips compared to your root. 
        //Ex: 100f if your armature is x100 your root.
        public float ArmatureRootScaleRatio = 1.0f; 

        public RootMotionConfig(bool ApplyHipsYPos = false, bool ApplyHipsXAndZPos = false, bool BackToInitialPose = false, bool BakeIntoPoseXZ = false, bool BakeIntoPoseY = false, float ArmatureRootScaleRatio = 1.0f)
        {
            this.ApplyHipsYPos = ApplyHipsYPos;
            this.ApplyHipsXAndZPos = ApplyHipsXAndZPos;
            this.BackToInitialPose = BackToInitialPose;
            this.BakeIntoPoseXZ = BakeIntoPoseXZ;
            this.BakeIntoPoseY = BakeIntoPoseY;
            this.ArmatureRootScaleRatio = ArmatureRootScaleRatio;
        }
    }
}
