using System.Collections.Generic;

namespace Kinetix.Internal
{
    internal static class KinetixContextBehaviour
    {
        public static bool PlayContext(string contextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().PlayContext(contextName);
        }

        public static void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().RegisterEmoteForContext(contextName, emoteUuid);
        }

        
        public static void UnregisterEmoteForContext(string contextName)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().UnregisterEmoteForContext(contextName);
        }

        public static ContextualEmote GetContextEmote(string contextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().GetContextEmote(contextName);
        }

        public static Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().GetContextEmotes();
        }

        public static bool IsContextEmoteAvailable(string contextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().IsContextEmoteAvailable(contextName);
        }
    }
}
