using System.Collections.Generic;

namespace Kinetix.Internal
{
    internal static class KinetixContextBehaviour
    {
        public static bool PlayContext(string contextName)
        {
            return ContextManager.PlayContext(contextName);
        }

        public static void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            ContextManager.RegisterEmoteForContext(contextName, emoteUuid);
        }

        
        public static void UnregisterEmoteForContext(string contextName)
        {
            ContextManager.UnregisterEmoteForContext(contextName);
        }

        public static ContextualEmote GetContextEmote(string contextName)
        {
            return ContextManager.GetContextEmote(contextName);
        }

        public static Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return ContextManager.GetContextEmotes();
        }

        public static bool IsContextEmoteAvailable(string contextName)
        {
            return ContextManager.IsContextEmoteAvailable(contextName);
        }
    }
}
