using Kinetix.Internal.Retargeting.AnimationData;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kinetix.Internal
{
	public class LoadAnimService : IKinetixService
	{
		private const string NO_METADATA_FOUND_FOR_EMOTE = "No metadata found for emote : ";
		
		private readonly AAnimLoader[] loaders;
		private readonly ServiceLocator serviceLocator;

		public LoadAnimService(ServiceLocator _ServiceLocator)
		{
			serviceLocator = _ServiceLocator;
			
			loaders = new AAnimLoader[]
			{
				new KinanimLoader(serviceLocator), //The order is important, it defines the search priority
				new GLBLoader(serviceLocator),
			};
		}

		/// <summary>
		/// Get a frame indexer for the emote. (= load animation)<br/>
		/// Download the file if needed
		/// </summary>
		/// <param name="emote">Emote to load</param>
		/// <param name="cancellationToken">Cancel token</param>
		/// <param name="avatarId">RT3K avatar id</param>
		/// <returns>Indexer of the loaded animation</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="Exception"></exception>
		public async Task<(RuntimeRetargetFrameIndexer, bool isRT3K)> GetFrameIndexer(KinetixEmote emote, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			if (cancellationToken == null)
				throw new ArgumentNullException(nameof(cancellationToken));

			if (emote == null)
				throw new ArgumentNullException(nameof(emote));

			if (!emote.HasMetadata())
				throw new Exception(NO_METADATA_FOUND_FOR_EMOTE + emote.Ids.UUID);

			//Check if has already been loaded once
			if (emote.HasValidPath(avatarId))
				return (await LoadAnimation(emote, avatarId == null ? emote.FilePath : emote.RT3KPath[avatarId], cancellationToken, avatarId), avatarId != null);

			//Check if exists in storage but has never been loaded once
			AAnimLoader loader;
			int loaderCount = loaders.Length;
			for (int i = 0; i < loaderCount; i++)
			{
				loader = loaders[i];
				if (loader.ExistInLocal(emote.Ids.UUID, avatarId))
				{
					return (await LoadAnimation(emote, loader.GetFilePath(emote.Ids.UUID, avatarId), cancellationToken, avatarId, loader), avatarId != null);
				}
			}

			//Try to download rt3k
			(bool success, RuntimeRetargetFrameIndexer result) = await TryDownload(emote, cancellationToken, avatarId);
			if (success)
				return (result, avatarId != null);

			//There is no RT3K for this animation, let's check for non-retargeted animation
			if (avatarId != null)
			{
				//Check if has already been loaded once
				if (emote.HasValidPath(null))
					return (await LoadAnimation(emote, emote.FilePath, null), false);

				for (int i = 0; i < loaderCount; i++)
				{
					//Check if exists in storage but has never been loaded once
					loader = loaders[i];
					if (loader.ExistInLocal(emote.Ids.UUID, null))
					{
						return (await LoadAnimation(emote, loader.GetFilePath(emote.Ids.UUID, null), cancellationToken, null, loader), false);
					}
				}

				//Try to download
				(success, result) = await TryDownload(emote, null);
				if (success)
					return (result, false);
			}

			return (null, false);
		}

		private async Task<RuntimeRetargetFrameIndexer> LoadAnimation(KinetixEmote emote, string filepath, CancellationTokenSource cancellationToken, string avatarId = null, AAnimLoader loader = null)
		{
			if (filepath == null)
				throw new ArgumentNullException(nameof(filepath));

			loader ??= GetLoaderFromFilepath(filepath);

			serviceLocator.Get<MemoryService>().TagFileAsBeingInUse(emote.Ids.UUID);
			return await loader.Load(emote, filepath, cancellationToken, avatarId);
		}

		private async Task<(bool success, RuntimeRetargetFrameIndexer result)> TryDownload(KinetixEmote emote, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			if (emote == null)
				throw new ArgumentNullException(nameof(emote));
			
			string url;
			if (avatarId != null)
			{
				url = emote.GetAnimationURLOrNull(avatarId);
				if (url == null)
					return (false, null);
			}
			else
				url = emote.Metadata.AnimationURL;

			AAnimLoader loader = GetLoaderFromFilepath(url);

			RuntimeRetargetFrameIndexer downloaded = await loader.Download(emote, url, cancellationToken);
			serviceLocator.Get<MemoryService>().TagFileAsBeingInUse(emote.Ids.UUID);
			serviceLocator.Get<MemoryService>().AddStorageAllocation(emote.FilePath);

			return (true, downloaded);
		}

		private AAnimLoader GetLoaderFromFilepath(string filepath)
		{
			if (filepath == null)
				throw new ArgumentNullException(nameof(filepath));

			string extension = Path.GetExtension(filepath);
			if (extension.Contains("?"))
				extension = extension.Substring(0, extension.IndexOf("?"));

			extension = extension.ToLower();
			try
			{
				return loaders.First(First);
				bool First(AAnimLoader loader)
				{
					return loader.Extension == extension;
				}
			}
			catch (Exception e)
			{
				throw new Exception($"Couldn't find file extension '{extension}'", e);
			}
		}
	}
}
