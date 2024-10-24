// // ----------------------------------------------------------------------------
// // <copyright file="ProviderService.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kinetix.Utils;
using UnityEngine;

namespace Kinetix.Internal
{
    internal class ProviderService: IKinetixService
    {
        private Dictionary<EKinetixNodeProvider, IProviderWrapper> ProviderWrappers;


        public ProviderService(KinetixCoreConfiguration _Config)
        {
            ProviderWrappers ??= new Dictionary<EKinetixNodeProvider, IProviderWrapper>();
            
            CreateProvider(_Config.EmoteProvider, _Config.GameAPIKey, _Config.APIBaseURL);
        }

      
        /// <summary>
        /// Create an instance of the Wrapper based on the provider
        /// </summary>
        /// <param name="_Provider">Node URL Provider</param>
        private void CreateProvider(EKinetixNodeProvider _Provider, string _APIKey, string _APIBaseURL)
        {
            switch (_Provider)
            {
                case EKinetixNodeProvider.SDK_API:
                    ProviderWrappers[EKinetixNodeProvider.SDK_API] = new SdkApiProviderWrapper(_APIKey, _APIBaseURL);
                    break;
            }
        }


        /// <summary>
        /// Make a Web Request to get all the user's emotes
        /// </summary>
        /// <param name="_Account">The account type</param>
        public async Task<AnimationMetadata[]> GetAnimationMetadataOfOwner(Account _Account)
        {
            try
            {
                if (String.IsNullOrEmpty(_Account.AccountId))
                {
                    throw new Exception("Account id is empty");
                }
                
                AnimationMetadata[] animationMetadatas = await ProviderWrappers[GetTypeForAccountSpecialization(_Account)].GetAnimationsMetadataOfOwner(_Account.AccountId);
                
                return animationMetadatas;
            }
            catch (Exception e)
            {
                KinetixDebug.LogException(e);
                return new AnimationMetadata[] { };
            }
        }

        /// <summary>
        /// Make a Web Request to get metadata of a specific emote
        /// </summary>
        /// <param name="_AnimationIds">Id of the animation</param>
        public async Task<AnimationMetadata> GetAnimationMetadataOfEmote(AnimationIds _AnimationIds)
        {
            try
            {
                return await ProviderWrappers[_AnimationIds.GetExpectedProvider()].GetAnimationMetadataOfEmote(_AnimationIds);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<SdkApiProcess[]> GetUserProcesses(string _AccountId)
        {
            try
            {
                return await ProviderWrappers[EKinetixNodeProvider.SDK_API].GetAnimationProcessesOfOwner(_AccountId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<SdkApiProcess> ValidateEmote(string _EmoteId)
        {
            try
            {
                return await ProviderWrappers[EKinetixNodeProvider.SDK_API].ValidateEmote(_EmoteId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        public async Task<SdkTokenValidityResult> RetakeEmote(string _EmoteId)
        {
            try
            {
                return await ProviderWrappers[EKinetixNodeProvider.SDK_API].RetakeEmote(_EmoteId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

		/// <summary>
		/// Make a Web Request to get metadata of a specific emote
		/// </summary>
		/// <param name="_AnimationIds">Id of the animation</param>
		/// <param name="_AvatarUUID">Id of the avatar</param>
		public async Task<AnimationMetadata> GetAnimationMetadataOfEmote(AnimationIds _AnimationIds, string _AvatarUUID, AnimationMetadata _Metadata)
        {
            try
            {
                return await ProviderWrappers[_AnimationIds.GetExpectedProvider()].GetAnimationMetadataOfAvatar(_AnimationIds, _AvatarUUID, _Metadata);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private EKinetixNodeProvider GetTypeForAccountSpecialization(Account _Account)
        {
            if (_Account is UserAccount)
                return EKinetixNodeProvider.SDK_API;
            return EKinetixNodeProvider.NONE;
        }
    }
}

