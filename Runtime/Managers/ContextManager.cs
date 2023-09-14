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

        public void Initialize(ContextualEmoteSO _EmoteContexts = null)
        {
            emoteContextsSO = _EmoteContexts;
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


        public bool PlayContext(string _ContextName)
        {
            if (!contexts.ContainsKey(_ContextName))
                return false;
            
            if (_ContextName == string.Empty)
                return false;

            if (contexts[_ContextName].EmoteID == string.Empty)
                return false;
            
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(new AnimationIds(contexts[_ContextName].EmoteID));

            return true;
        }

        public bool PlayContext(string _PlayerUUID, string _ContextName)
        {
            if (!contexts.ContainsKey(_ContextName))
                return false;
            
            if (_ContextName == string.Empty)
                return false;

            if (contexts[_ContextName].EmoteID == string.Empty)
                return false;
            
            KinetixCore.Animation.PlayAnimationOnAvatar(_PlayerUUID, new AnimationIds(contexts[_ContextName].EmoteID));

            return true;
        }

        public void RegisterEmoteForContext(string _ContextName, ContextualEmote _Emote)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();
            
            contexts[_ContextName] = _Emote.Clone();
        }

        public void RegisterEmoteForContext(string _ContextName, string _EmoteID)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(_ContextName))
                contexts[_ContextName] = new ContextualEmote();

            ContextualEmote emote = contexts[_ContextName];
            emote.EmoteID = _EmoteID;

            LoadContextEmote(new AnimationIds(_EmoteID), _ContextName);

            RegisterEmoteForContext(_ContextName, emote);
        }

        public void UnregisterEmoteForContext(string _ContextName)
        {
            contexts ??= new Dictionary<string, ContextualEmote>();

            if (!contexts.ContainsKey(_ContextName))
                return;
            
            ContextualEmote emote = contexts[_ContextName];

            UnloadContextEmote(new AnimationIds(emote.EmoteID), _ContextName);

            emote.EmoteID = string.Empty;
        }

        public void LoadContextEmote(AnimationIds _EmoteIds, string _ContextName)
        {     
            KinetixCore.Animation.LoadLocalPlayerAnimation(_EmoteIds, LOCK_CONTEXT_ID + "_" + _ContextName);
        }

        public void UnloadContextEmote(AnimationIds _EmoteIds, string _ContextName)
        {
            KinetixCore.Animation.UnloadLocalPlayerAnimation(_EmoteIds, LOCK_CONTEXT_ID + "_" + _ContextName);
        }

        public ContextualEmote GetContextEmote(string _ContextName)
        {
            if (!contexts.ContainsKey(_ContextName))
                return null;

            return contexts[_ContextName];
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

        public bool IsContextEmoteAvailable(string _ContextName)
        {
            return contexts.ContainsKey(_ContextName)
            && contexts[_ContextName].EmoteID != string.Empty
            && KinetixCore.Animation.IsAnimationAvailableOnLocalPlayer(new AnimationIds(contexts[_ContextName].EmoteID));
        }
    }
}

