using System.Collections.Generic;

namespace Kinetix.Internal
{
    public class KinetixContext
    {
        public bool PlayContext(string contextName)
        {
            return KinetixContextBehaviour.PlayContext(contextName);
        }

        public void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            KinetixContextBehaviour.RegisterEmoteForContext(contextName, emoteUuid);
        }

        public ContextualEmote GetContextEmote(string contextName)
        {
            return KinetixContextBehaviour.GetContextEmote(contextName);
        }

        public Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return KinetixContextBehaviour.GetContextEmotes();
        }
    }
}
