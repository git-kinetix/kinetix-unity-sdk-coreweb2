// FILE_WEB2
namespace Kinetix.Internal
{
    internal class SdkApiUserAsset
    {
        public int id;
        public string emoteUuid;
        public string userId;
        public SdkApiAsset data;
        public System.DateTime createdAt;

        public AnimationMetadata ToAnimationMetadata()
        {
            if (data == null)
            {
                KinetixDebug.LogError("The animation " + emoteUuid + " is invalid.");
                return null;
            }
                
            return data.ToAnimationMetadata();
        }
    }
}
