using System.Collections.Generic;

namespace Kinetix.Internal
{
    internal static class KinetixContextBehaviour
    {
        public static bool PlayContext(string _ContextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().PlayContext(_ContextName);
        }

        public static bool PlayContext(string _PlayerUUID, string _ContextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().PlayContext(_PlayerUUID, _ContextName);
        }

        public static void RegisterEmoteForContext(string _ContextName, string emoteUuid)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().RegisterEmoteForContext(_ContextName, emoteUuid);
        }

        
        public static void UnregisterEmoteForContext(string _ContextName)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().UnregisterEmoteForContext(_ContextName);
        }

        public static ContextualEmote GetContextEmote(string _ContextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().GetContextEmote(_ContextName);
        }

        public static Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().GetContextEmotes();
        }

        public static bool IsContextEmoteAvailable(string _ContextName)
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<ContextManager>().IsContextEmoteAvailable(_ContextName);
        }
    }
}
