using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("Kinetix.Tests.Unit.Editor")]
namespace Kinetix.Internal
{
    [Serializable]
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
