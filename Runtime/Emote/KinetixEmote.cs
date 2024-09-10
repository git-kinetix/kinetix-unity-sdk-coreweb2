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
		public bool HasValidPath(string _AvatarId = null)
		{
			string filePath = _AvatarId == null ? this.FilePath : RT3KPath.ValueOrDefault(_AvatarId);
			
			if (string.IsNullOrEmpty(filePath))
				return false;
			return File.Exists(filePath);
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

		public AvatarAnimationMetadata GetAvatarMetadata(string _AvatarId)
		{
			return Metadata.Avatars.FirstOrDefault(First);
			bool First(AvatarAnimationMetadata avatar) => avatar.AvatarUUID == _AvatarId;
		}

		public string GetAnimationURLOrNull(string _AvatarId = null)
		{
			if (_AvatarId == null)
				return Metadata.AnimationURL;

			return GetAvatarMetadata(_AvatarId)?.AnimationURL;
		}
		
		public string GetAnimationURLOrNull(string _Extension, string _AvatarId)
		{
			if (_AvatarId == null)
				return Metadata.UrlByFormat.ValueOrDefault(_Extension);

			return GetAvatarMetadata(_AvatarId)?.UrlByFormat.ValueOrDefault(_Extension);
		}
	}
}
