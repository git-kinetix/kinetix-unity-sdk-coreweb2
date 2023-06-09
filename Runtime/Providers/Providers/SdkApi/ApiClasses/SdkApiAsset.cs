// FILE_WEB2
namespace Kinetix.Internal 
{
    [System.Serializable]
    internal class SdkApiAsset
    {

        const string Thumbnail_Filename = "thumbnail";
        const string Animation_Filename = "animation";

        public int id;
        public string uuid;
        public string name;
        public string type;
        public SignedFile[] files;
        public System.DateTime createdAt;

        public AnimationMetadata ToAnimationMetadata()
        {
            ERarity enumRarity   = ERarity.NONE;
            string  animUrl      = string.Empty;
            string  thumbnailUrl = string.Empty;

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
                }
            }

            AnimationMetadata animationMetadata = new AnimationMetadata
            {
                Ids          = new AnimationIds(uuid),
                Name         = name,
                Description  = string.Empty,
                AnimationURL = animUrl,
                IconeURL     = thumbnailUrl,
                Ownership    = EOwnership.OWNER,
                CreatedAt    = createdAt,

                

                EmoteRarity = enumRarity,
                
                Exploration = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Ranking = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                ArtisticLevel = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                TechnicalLevel = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                VisualElement = new ElementAttribute()
                {
                    Value = EVisualElement.NONE
                },
                VisualEffect = new BoolAttribute()
                {
                    Value = false
                },
                ContactFeet = new BoolAttribute()
                {
                    Value = false
                },
                Aerial =  new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Amplitude = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Speed = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Spin =  new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Stamina = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
                Strength = new NumberAttribute()
                {
                    Value = 0, Max_Value = 100
                },
            };

            return animationMetadata;
        }
    }
}
