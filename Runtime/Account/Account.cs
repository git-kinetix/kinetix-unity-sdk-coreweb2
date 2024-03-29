using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kinetix.Internal
{
    public class Account
    {
        public string AccountId { get { return accountId; } }
        private string accountId;

        public List<KinetixEmote> Emotes { get { return emotes; } }
        private List<KinetixEmote> emotes;


        public Account(string _AccountId)
        {
            accountId = _AccountId;
        }

        public async Task<KinetixEmote[]> FetchMetadatas()
        {   
            if (emotes != null)
                return emotes.ToArray();
            
            try
            {
                AnimationMetadata[] animationMetadatas = await KinetixCoreBehaviour.ServiceLocator.Get<ProviderService>().GetAnimationMetadataOfOwner(this);
                emotes = new List<KinetixEmote>();
                
                for (int i = 0; i < animationMetadatas.Length; i++)
                {
                    if (animationMetadatas[i] != null)
                        AddEmoteFromMetadata(animationMetadatas[i]);
                }
                
                return emotes.ToArray();
            }
            catch (Exception)
            {
                return new KinetixEmote[] { };
            }
        }

        public void AddEmotesFromMetadata(AnimationMetadata[] animationsMetadata)
        {
            for (int i = 0; i < animationsMetadata.Length; i++)
            {
                AddEmoteFromMetadata(animationsMetadata[i]);
            }
        }
        
        public void AddEmoteFromMetadata(AnimationMetadata animationMetadata)
        {
            KinetixEmote emote = KinetixCoreBehaviour.ServiceLocator.Get<EmotesService>().GetEmote(animationMetadata.Ids);
            
            emote.SetMetadata(animationMetadata);
            emotes.Add(emote);
        }

        public async Task<AnimationMetadata> AddEmoteFromIds(AnimationIds animationMetadataIds)
        {
            try
            {
                AnimationMetadata animMetadata = await KinetixCoreBehaviour.ServiceLocator.Get<ProviderService>().GetAnimationMetadataOfEmote(animationMetadataIds);
                AddEmoteFromMetadata(animMetadata);
                return animMetadata;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool HasEmote(AnimationIds emoteIds)
        {
            if (emotes == null)
                return false;

            foreach (KinetixEmote emote in emotes)
            {
                if (emoteIds.Equals(emote.Metadata.Ids))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
