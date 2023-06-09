using System.Collections.Generic;

namespace Kinetix.Internal
{
    public class KinetixContext
    {
        public bool PlayContext(string _ContextName)
        {
            return KinetixContextBehaviour.PlayContext(_ContextName);
        }

        public void RegisterEmoteForContext(string _ContextName, string _EmoteUuid)
        {
            KinetixContextBehaviour.RegisterEmoteForContext(_ContextName, _EmoteUuid);
        }

        public void UnregisterEmoteForContext(string _ContextName)
        {            
            KinetixContextBehaviour.UnregisterEmoteForContext(_ContextName);
        }

        public ContextualEmote GetContextEmote(string _ContextName)
        {
            return KinetixContextBehaviour.GetContextEmote(_ContextName);
        }

        public Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return KinetixContextBehaviour.GetContextEmotes();
        }

        public bool IsContextEmoteAvailable(string _ContextName)
        {
            return KinetixContextBehaviour.IsContextEmoteAvailable(_ContextName);
        }
    }
}
