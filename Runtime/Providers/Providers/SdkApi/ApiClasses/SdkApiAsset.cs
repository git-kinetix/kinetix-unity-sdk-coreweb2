using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Kinetix.Internal
{
	[System.Serializable]
	internal class SdkAvatarApiAsset
	{
		public int id;
		public string uuid;
		public string name;
		public string source;
		public string defaultThumbnailGifUrl;
		public string defaultThumbnailPngUrl;
		public string thumbnailGifUrl;
		public string thumbnailPngUrl;
		public string animationFbxUrl;
		public string animationGlbUrl;
		public string animationKinanimUrl;

		public string animationURL => animationKinanimUrl ?? animationGlbUrl;

		public System.DateTime createdAt;

		public void EditAnimationMetadata(ref AnimationMetadata _Metadata, string _AvatarId)
		{
			_Metadata ??= new AnimationMetadata()
			{
				Ids = new AnimationIds(uuid),
				Avatars = new List<AvatarAnimationMetadata>(),
				CreatedAt = createdAt,
				Name = name,
				UrlByFormat = new Dictionary<string, string>(),
			};

			if (string.IsNullOrEmpty(_AvatarId))
			{
				throw new ArgumentException($"\" {nameof(_AvatarId)} \" can't be null or empty.", nameof(_AvatarId));
			}

			AvatarAnimationMetadata avatar = _Metadata.Avatars.FirstOrDefault(FirstOrDefault);
			if (avatar == null)
			{
				avatar = new AvatarAnimationMetadata();
				avatar.AvatarUUID = _AvatarId;
				_Metadata.Avatars.Add(avatar);
			}

			avatar.AnimationURL = animationURL;
			if (string.IsNullOrEmpty(animationFbxUrl))
			{
				avatar.AnimationURL = animationGlbUrl;
			}

			avatar.IconUrl = thumbnailPngUrl;

			bool FirstOrDefault(AvatarAnimationMetadata metadata) => metadata.AvatarUUID == _AvatarId;
		}

	}

	[System.Serializable]
	internal class SdkApiAsset
	{
		const         string Thumbnail_Filename        = "thumbnail";
		const         string Animation_Filename        = "animation-v2";
		private const string Animation_Filename_Legacy = "animation";

		public int                                    id;
		public string                                 uuid;
		public string                                 name;
		public string                                 type;
		public SignedFile[]                           files;
		public Dictionary<string, AvatarSignedFile[]> avatars;

		public System.DateTime createdAt;

		public AnimationMetadata ToAnimationMetadata()
		{
			string currentUrlExtension = string.Empty;
			string animUrl       = string.Empty;
			string animUrlLegacy = string.Empty;
			string thumbnailUrl  = string.Empty;
			Dictionary<string, string> urlByFormat = new Dictionary<string, string>();

			foreach (SignedFile file in files)
			{
				if (file.extension == "fbx")
						continue;
		
				switch (file.name)
				{
					case Thumbnail_Filename:
					{
						if (file.extension == "png")
							thumbnailUrl = file.url;
					}
						break;

					case Animation_Filename:
						urlByFormat["." + file.extension.ToLower()] = file.url;

						if (currentUrlExtension != "kinanim") 
						{
							animUrl = file.url;
							currentUrlExtension = file.extension;
						}
						break;

					case Animation_Filename_Legacy:
						animUrlLegacy = file.url;
						break;
				}
			}

			currentUrlExtension = string.Empty;

			List<AvatarAnimationMetadata> avatarsMetadata = new List<AvatarAnimationMetadata>();
			foreach (KeyValuePair<string, AvatarSignedFile[]> filesTmp in avatars)
			{
				AvatarAnimationMetadata avatarAnimationMetadata = new AvatarAnimationMetadata
				{
					UrlByFormat = new Dictionary<string, string>(),
					AvatarUUID = filesTmp.Key
				};

				foreach (AvatarSignedFile file in filesTmp.Value)
				{
					if (file.extension == "fbx")
						continue;

					switch (file.name)
					{
						case Thumbnail_Filename:
							if (file.extension == "png")
								avatarAnimationMetadata.IconUrl = file.url;
							break;

						case Animation_Filename:
							avatarAnimationMetadata.UrlByFormat["." + file.extension.ToLower()] = file.url;

							if (currentUrlExtension != "kinanim")
							{
								avatarAnimationMetadata.AnimationURL = file.url;
								currentUrlExtension = file.extension;
							}
							break;

						case Animation_Filename_Legacy:
							avatarAnimationMetadata.UrlByFormat["." + file.extension.ToLower()] = file.url;
							avatarAnimationMetadata.AnimationURL = file.url;
							break;
					}
				}
				
				avatarsMetadata.Add(avatarAnimationMetadata);
			}

			string animationURl = string.IsNullOrEmpty(animUrl) ? animUrlLegacy : animUrl;

			AnimationMetadata animationMetadata = new AnimationMetadata
			{
				Ids          = new AnimationIds(uuid),
				Name         = name,
				Description  = string.Empty,
				AnimationURL = animationURl,
				IconeURL     = thumbnailUrl,
				Ownership    = EOwnership.OWNER,
				CreatedAt    = createdAt,
				Avatars      = avatarsMetadata,
				UrlByFormat = urlByFormat
			};

			return animationMetadata;
		}
	}
}
