namespace Kinetix.Internal
{
    [System.Serializable]
    internal class SdkApiAsset
    {
        const         string Thumbnail_Filename        = "thumbnail";
        const         string Animation_Filename        = "animation-v2";
        private const string Animation_Filename_Legacy = "animation";

        public int             id;
        public string          uuid;
        public string          name;
        public string          type;
        public SignedFile[]    files;
        public System.DateTime createdAt;

        public AnimationMetadata ToAnimationMetadata()
        {
            string animUrl       = string.Empty;
            string animUrlLegacy = string.Empty;
            string thumbnailUrl  = string.Empty;

            foreach (SignedFile file in files)
            {
                switch (file.name)
                {
                    case Thumbnail_Filename:
                    {
                        if (file.extension == "png")
                            thumbnailUrl = file.url;
                    }
                        break;
                    
                    case Animation_Filename:
                        animUrl = file.url;
                        break;

                    case Animation_Filename_Legacy:
                        animUrlLegacy = file.url;
                        break;
                }
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
            };

            return animationMetadata;
        }
    }
}
