// // ----------------------------------------------------------------------------
// // <copyright file="IProviderWrapper.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Kinetix;

public interface IProviderWrapper
{
    public Task<AnimationMetadata[]> GetAnimationsMetadataOfOwner(string _AccountId);
    
    public Task<AnimationMetadata> GetAnimationMetadataOfEmote(AnimationIds _AnimationIds);
    public Task<AnimationMetadata> GetAnimationMetadataOfAvatar(AnimationIds _AnimationIds, string _AvatarId, AnimationMetadata _Metadata = null);
}

