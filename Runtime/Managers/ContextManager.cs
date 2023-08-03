using System.Collections.Generic;
using Kinetix.Internal.Cache;
using UnityEngine;


namespace Kinetix.Internal
{
    internal class ContextManager: AKinetixManager
    {
        private ContextualEmoteSO emoteContextsSO;
        private Dictionary<string, ContextualEmote> contexts;

        const string LOCK_CONTEXT_ID = "ContextManager";

        public ContextManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config) {}

        protected override void Initialize(KinetixCoreConfiguration _Config)
        {
            Initialize(_Config.EmoteContexts);
        }

        public void Initialize(ContextualEmoteSO emoteContexts = null)
        {
            emoteContextsSO = emoteContexts;
            contexts = new Dictionary<string, ContextualEmote>();

            InitEmotes();
        }

        public void InitEmotes()
        {
            if (emoteContextsSO == null)
                return;
            
            foreach (ContextualEmote emote in emoteContextsSO.contexts)
            {
                contexts[emote.ContextName] = emote.Clone();
            }
        }


        public void RegisterEmotes()
        {
            if (emoteContextsSO == null)
                return;
            
            foreach (ContextualEmote emote in emoteContextsSO.contexts)
            {
                RegisterEmoteForContext(emote.ContextName, emote);
            }
        }


        public bool PlayContext(string contextName)
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

        public void RegisterEmoteForContext(string contextName, ContextualEmote emote)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();
            
            contexts[contextName] = emote.Clone();
        }

        public void RegisterEmoteForContext(string contextName, string emoteUuid)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(contextName))
                contexts[contextName] = new ContextualEmote();

            ContextualEmote emote = contexts[contextName];
            emote.EmoteUuid = emoteUuid;

            LoadContextEmote(new AnimationIds(emoteUuid), contextName);

            RegisterEmoteForContext(contextName, emote);
        }

        public void UnregisterEmoteForContext(string contextName)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(contextName))
                return;
            
            ContextualEmote emote = contexts[contextName];

            UnloadContextEmote(new AnimationIds(emote.EmoteUuid), contextName);

            emote.EmoteUuid = string.Empty;
        }

        public void LoadContextEmote(AnimationIds emoteIds, string contextName)
        {     
            KinetixCore.Animation.LoadLocalPlayerAnimation(emoteIds, LOCK_CONTEXT_ID + "_" + contextName);
        }

        public void UnloadContextEmote(AnimationIds emoteIds, string contextName)
        {
            KinetixCore.Animation.UnloadLocalPlayerAnimation(emoteIds, LOCK_CONTEXT_ID + "_" + contextName);
        }

        public ContextualEmote GetContextEmote(string contextName)
        {
            if (!contexts.ContainsKey(contextName))
                return null;

            return contexts[contextName];
        }

        public Dictionary<string, ContextualEmote> GetContextEmotes()
        {
            Dictionary<string, ContextualEmote> contextsCopy = new Dictionary<string, ContextualEmote>();

            foreach (KeyValuePair<string, ContextualEmote> context in contexts)
            {
                contextsCopy.Add(context.Key, context.Value.Clone());
            }

            return contextsCopy;
        }

        public bool IsContextEmoteAvailable(string contextName)
        {
            return contexts.ContainsKey(contextName)
            && contexts[contextName].EmoteUuid != string.Empty
            && KinetixCore.Animation.IsAnimationAvailableOnLocalPlayer(new AnimationIds(contexts[contextName].EmoteUuid));
        }
    }
}

