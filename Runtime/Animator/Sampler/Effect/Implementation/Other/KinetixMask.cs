// // ----------------------------------------------------------------------------
// // <copyright file="KinetixMask.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace Kinetix
{
	/// <summary>
	/// A mask to disable some bones of an Avatar from playing during a kinetix emote.
	/// </summary>
	//[CreateAssetMenu(menuName = "Kinetix/AvatarMask", fileName = "KinetixMask")]
	public class KinetixMask : ScriptableObject
	{
		[SerializeField]
		private bool[] disabledBones = new bool[(int)HumanBodyBones.LastBone];

        /// <summary>
        /// Check if a bone is enabled to play or not.
        /// </summary>
        /// <param name="_Bone">Human bone to check.</param>
        /// <returns>True if the bone is enabled to play.</returns>
        public bool IsEnabled(HumanBodyBones _Bone)
        {
            FixArraySize();
			return !disabledBones[(int)_Bone];
        }

        /// <summary>
        /// Set a bone's enabled state. If disabled, it won't play during a kinetix emote.
        /// </summary>
        /// <param name="_Bone">Human bone to set.</param>
        /// <param name="_Value">Set to false to disable a bone from playing.<br/> Set to true to enable a bone to play.<br/></param>
        public void SetEnabled(HumanBodyBones _Bone, bool _Value)
		{
			FixArraySize();
			disabledBones[(int)_Bone] = !_Value;
        }

        private void OnValidate()
        {
            FixArraySize();
        }

        internal void FixArraySize()
        {
            if (disabledBones.Length != (int)HumanBodyBones.LastBone)
            {
                Array.Resize(ref disabledBones, (int)HumanBodyBones.LastBone);
            }
        }
    }
}
