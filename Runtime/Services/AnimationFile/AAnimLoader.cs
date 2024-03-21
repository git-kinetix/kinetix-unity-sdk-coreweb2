using Kinetix.Internal.Retargeting.AnimationData;
using Kinetix.Utils;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kinetix.Internal
{
	public abstract class AAnimLoader
	{
		public abstract string Extension { get; }
		protected readonly ServiceLocator serviceLocator;

		public AAnimLoader(ServiceLocator _ServiceLocator)
		{
			serviceLocator = _ServiceLocator;
		}

		/// <summary>
		/// Load the animation from local path
		/// </summary>
		public abstract Task<RuntimeRetargetFrameIndexer> Load(KinetixEmote emote, string filepath, CancellationTokenSource cancellationToken, string avatarId = null);
		/// <summary>
		/// Download and load the animation
		/// </summary>
		public abstract Task<RuntimeRetargetFrameIndexer> Download(KinetixEmote emote, string url, CancellationTokenSource cancellationToken, string avatarId = null);
		
		public string GetFilePath(string emoteId, string avatarId = null)
		{
			string filename;
			if (avatarId == null)
				filename = emoteId + Extension;
			else
				filename = avatarId + "/" + emoteId + Extension;
			return Path.Combine(PathConstants.CacheAnimationsPath, filename);
		}
		public bool ExistInLocal(string emoteId, string avatarId = null) => File.Exists(GetFilePath(emoteId, avatarId));

		public string EnsureDirectoryExist(string path)
		{
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			return path;
		}
	}
}
