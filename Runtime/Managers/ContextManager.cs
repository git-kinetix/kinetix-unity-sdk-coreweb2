using System.Collections.Generic;
using Kinetix.Internal.Cache;
using UnityEngine;


namespace Kinetix.Internal
{
    internal static class ContextManager
    {
        private static ContextualEmoteSO emoteContextsSO;
        private static Dictionary<string, ContextualEmote> contexts;

        public static void Initialize(ContextualEmoteSO emoteContexts = null)
        {
            emoteContextsSO = emoteContexts;
            contexts = new Dictionary<string, ContextualEmote>();
            InitEmotes();

            KinetixCore.Account.OnConnectedAccount += RegisterEmotes;
        }

        public static void InitEmotes()
        {
            if (emoteContextsSO == null)
                return;
            
            foreach (ContextualEmote emote in emoteContextsSO.contexts)
            {
                contexts[emote.ContextName] = emote.Clone();
            }
        }


        public static void RegisterEmotes()
        {
            if (emoteContextsSO == null)
                return;
            
            foreach (ContextualEmote emote in emoteContextsSO.contexts)
            {
                RegisterEmoteForContext(emote.ContextName, emote);
            }
        }


        public static bool PlayContext(string contextName)
        {
            if (!contexts.ContainsKey(contextName))
                return false;
            
            if (contextName == string.Empty)
                return false;
            
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(new AnimationIds(contexts[contextName].EmoteUuid));

            return true;
        }

        public static void RegisterEmoteForContext(string contextName, ContextualEmote emote)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();
            AnimationIds emoteIds = new AnimationIds(emote.EmoteUuid);

            KinetixCore.Animation.LoadLocalPlayerAnimations(new AnimationIds[] { emoteIds });
            
            contexts[contextName] = emote.Clone();
        }

        public static void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(contextName))
                contexts[contextName] = new ContextualEmote();

            ContextualEmote emote = contexts[contextName];
            emote.EmoteUuid = emoteUuid;

            RegisterEmoteForContext(contextName, emote);
        }

        public static ContextualEmote GetContextEmote(string contextName)
        {
            if (!contexts.ContainsKey(contextName))
                return null;

            return contexts[contextName];
        }

        public static Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            contexts ??= new Dictionary<string, ContextualEmote>();
            return contexts;
        }
    }
}

