// // ----------------------------------------------------------------------------
// // <copyright file="AnimationMetadata.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix
{
    [Serializable]
    public class AnimationMetadata
    {
		[SerializeField]
        public EOwnership Ownership;
        
		[SerializeField]
        public AnimationIds Ids;

		public string Name;
		public string Description;

        #region URL

		public string AnimationURL;
        public Dictionary<string, string> UrlByFormat = new Dictionary<string, string>();
        public string IconeURL;
		public DateTime CreatedAt;

		#endregion

        public List<AvatarAnimationMetadata> Avatars = new List<AvatarAnimationMetadata>();
    }

    [Serializable]
    public class AvatarAnimationMetadata
    {
        public string AvatarUUID;
        public string AnimationURL;
    }   
}
