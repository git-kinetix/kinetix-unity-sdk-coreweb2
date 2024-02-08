using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Kinetix.Internal
{
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
                        urlByFormat["." + file.extension] = file.url;
                        animUrl = file.url;
                        break;

                    case Animation_Filename_Legacy:
                        animUrlLegacy = file.url;
                        break;
                }
            }

            List<AvatarAnimationMetadata> avatarsMetadata = new List<AvatarAnimationMetadata>();
            foreach (KeyValuePair<string, AvatarSignedFile[]> filesTmp in avatars)
            {
                AvatarAnimationMetadata avatarAnimationMetadata = new AvatarAnimationMetadata();
                avatarAnimationMetadata.AvatarUUID = filesTmp.Key;

                foreach (AvatarSignedFile file in filesTmp.Value)
                {
                    if (file.extension == "fbx")
                        continue;

                    switch (file.name)
                    {
                        case Animation_Filename:
                            avatarAnimationMetadata.AnimationURL = file.url;
                            break;

                        case Animation_Filename_Legacy:
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
