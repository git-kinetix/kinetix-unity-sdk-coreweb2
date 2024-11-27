using System;
using System.Collections.Generic;

namespace Kinetix.Internal
{
    public class SdkApiProcess
    {
        public Guid Uuid;
        public Guid Emote;
        public Guid Animation;
        public Guid Video;
        public int Vw;
        public int User;
        public bool PostMLError;
        public string Status;
        public int Progression;
        public DateTime CreatedAt;
        public string Name;
        public bool Validated;
        public bool Rejected;
        public SdkApiProcessHierarchy Hierarchy;

        public bool CanBeValidatedOrRejected => !Validated && !Rejected;
    }

    public class SdkApiProcessHierarchy
    {
        public int Children;
        public SdkApiProcessChild Child;
        public int Parents;
        public SdkApiProcessParent Parent;
    }

    public class SdkApiProcessChild
    {
        public string Uuid;
        public SdkApiProcessChild Child;
    }

    public class SdkApiProcessParent
    {
        public string Uuid;
    }
}
