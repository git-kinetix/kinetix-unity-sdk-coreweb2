using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kinetix.Internal.Utils;

namespace Kinetix.Internal
{
	public class KinetixEmote
	{
		public AnimationIds      Ids      { get; }
		public AnimationMetadata Metadata { get; private set; }

		public string FilePath;
		public Dictionary<string, string> RT3KPath = new Dictionary<string, string>();

		public KinetixEmote(AnimationIds _Ids)
		{
			Ids                       = _Ids;
		}

		/// <summary>
		/// Check if emote contains metadata
		/// </summary>
		/// <returns>True if contains metadata</returns>
		public bool HasMetadata()
		{
			return Metadata != null;
		}

		/// <summary>
		/// Set the medata for this emote
		/// </summary>
		/// <param name="_AnimationMetadata">Metadata of the emote</param>
		public void SetMetadata(AnimationMetadata _AnimationMetadata)
		{
			Metadata = _AnimationMetadata;
		}
		
		/// <summary>
		/// Check if emote has a valid GLB Path in storage in order to import it
		/// </summary>
		/// <returns>True if path exists</returns>
		public bool HasValidPath(string avatarId = null)
		{
			string FilePath = avatarId == null ? this.FilePath : RT3KPath.ValueOrDefault(avatarId);
			
			if (string.IsNullOrEmpty(FilePath))
				return false;
			return File.Exists(FilePath);
		}

		public void ClearAvatar(KinetixAvatar _Avatar)
		{
			KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().ClearAvatar(this, _Avatar);
		}

		public void ClearAllAvatars(KinetixAvatar[] _AvoidAvatars = null)
		{
			KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().ClearAllAvatars(_AvoidAvatars);
		}

		public bool HasAnimationRetargeted(KinetixAvatar _Avatar)
		{
			return KinetixCoreBehaviour.ServiceLocator.Get<RetargetingService>().HasAnimationRetargeted(this, _Avatar);
		}


		public string GetAnimationURLOrFallback(string avatarId)
		{
			return GetAnimationURLOrNull(avatarId) ?? Metadata.AnimationURL;
		}

		public string GetAnimationURLOrNull(string avatarId = null)
		{
			if (avatarId == null)
				return Metadata.AnimationURL;

			return Metadata.Avatars.FirstOrDefault(First)?.AnimationURL;
			bool First(AvatarAnimationMetadata avatar) => avatar.AvatarUUID == avatarId;
		}
	}
}
