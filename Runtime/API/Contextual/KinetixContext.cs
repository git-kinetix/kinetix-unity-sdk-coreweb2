using System;
using System.Collections.Generic;

namespace Kinetix.Internal
{
    public class KinetixContext
    {
        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public bool PlayContext(string _ContextName)
        {
            return KinetixContextBehaviour.PlayContext(_ContextName);
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public bool PlayContext(string _PlayerUUID, string _ContextName)
        {
            return KinetixContextBehaviour.PlayContext(_PlayerUUID, _ContextName);
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public void RegisterEmoteForContext(string _ContextName, string _EmoteID)
        {
            KinetixContextBehaviour.RegisterEmoteForContext(_ContextName, _EmoteID);
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public void UnregisterEmoteForContext(string _ContextName)
        {            
            KinetixContextBehaviour.UnregisterEmoteForContext(_ContextName);
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public ContextualEmote GetContextEmote(string _ContextName)
        {
            return KinetixContextBehaviour.GetContextEmote(_ContextName);
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return KinetixContextBehaviour.GetContextEmotes();
        }

        [Obsolete("Context feaure is no longer supported and will be removed in next version")]
        public bool IsContextEmoteAvailable(string _ContextName)
        {
            return KinetixContextBehaviour.IsContextEmoteAvailable(_ContextName);
        }
    }
}
