using System.Collections.Generic;
using Kinetix.Internal.Cache;
using UnityEngine;


namespace Kinetix.Internal
{
    internal static class ContextManager
    {
        private static ContextualEmoteSO emoteContextsSO;
        private static Dictionary<string, ContextualEmote> contexts;

        const string LOCK_CONTEXT_ID = "ContextManager";

        public static void Initialize(ContextualEmoteSO emoteContexts = null)
        {
            emoteContextsSO = emoteContexts;
            contexts = new Dictionary<string, ContextualEmote>();

            InitEmotes();
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

            if (contexts[contextName].EmoteUuid == string.Empty)
                return false;
            
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(new AnimationIds(contexts[contextName].EmoteUuid));

            return true;
        }

        public static void RegisterEmoteForContext(string contextName, ContextualEmote emote)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();
            
            contexts[contextName] = emote.Clone();
        }

        public static void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(contextName))
                contexts[contextName] = new ContextualEmote();

            ContextualEmote emote = contexts[contextName];
            emote.EmoteUuid = emoteUuid;

            LoadContextEmote(new AnimationIds(emoteUuid), contextName);

            RegisterEmoteForContext(contextName, emote);
        }

        public static void UnregisterEmoteForContext(string contextName)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(contextName))
                return;
            
            ContextualEmote emote = contexts[contextName];

            UnloadContextEmote(new AnimationIds(emote.EmoteUuid), contextName);

            emote.EmoteUuid = string.Empty;
        }

        public static void LoadContextEmote(AnimationIds emoteIds, string contextName)
        {     
            KinetixCore.Animation.LoadLocalPlayerAnimation(emoteIds, LOCK_CONTEXT_ID + "_" + contextName);
        }

        public static void UnloadContextEmote(AnimationIds emoteIds, string contextName)
        {
            KinetixCore.Animation.UnloadLocalPlayerAnimation(emoteIds, LOCK_CONTEXT_ID + "_" + contextName);
        }

        public static ContextualEmote GetContextEmote(string contextName)
        {
            if (!contexts.ContainsKey(contextName))
                return null;

            return contexts[contextName];
        }

        public static Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            Dictionary<string, ContextualEmote> contextsCopy = new Dictionary<string, ContextualEmote>();

            foreach (KeyValuePair<string, ContextualEmote> context in contexts)
            {
                contextsCopy.Add(context.Key, context.Value.Clone());
            }

            return contextsCopy;
        }

        public static bool IsContextEmoteAvailable(string contextName)
        {
            return contexts.ContainsKey(contextName)
            && contexts[contextName].EmoteUuid != string.Empty
            && KinetixCore.Animation.IsAnimationAvailableOnLocalPlayer(new AnimationIds(contexts[contextName].EmoteUuid));
        }
    }
}

