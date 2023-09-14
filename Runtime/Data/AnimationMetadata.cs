// // ----------------------------------------------------------------------------
// // <copyright file="AnimationMetadata.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Kinetix.Internal;
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
        public string IconeURL;
		public DateTime CreatedAt;

		#endregion
    }
}
