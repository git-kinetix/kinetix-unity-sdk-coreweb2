using Kinetix.Internal.Retargeting.AnimationData;
using System.Threading;
using System.Threading.Tasks;

namespace Kinetix.Internal
{
    internal class GLBLoader : AAnimLoader
	{
		private const string FILE_EXTENSION = ".glb";

		public GLBLoader(ServiceLocator _ServiceLocator) : base(_ServiceLocator)
		{
		}

		public override string Extension => FILE_EXTENSION;

		public override async Task<RuntimeRetargetFrameIndexer> Load(KinetixEmote emote, string filepath, CancellationTokenSource cancellationToken, string avatarId)
		{
			EnsureDirectoryExist(filepath);

			GLTFUtility.AnimationJson[] animations = await RetargetingManager.LoadGLB(filepath);
			RuntimeRetargetFrameIndexer indexer = new GLBDataIndexer( animations[0], !string.IsNullOrEmpty(avatarId));
			indexer.Init();
			indexer.UpdateIndexCount();

			return indexer;
		}

		public override async Task<RuntimeRetargetFrameIndexer> Download(KinetixEmote emote, string url, CancellationTokenSource cancellationToken, string avatarId = null)
		{
			string filepath = GetFilePath(emote.Ids.UUID, avatarId);
			EnsureDirectoryExist(filepath);

			FileDownloaderConfig fileDownloadOperationConfig = new FileDownloaderConfig(url, filepath);
			FileDownloader fileDownloadOperation = new FileDownloader(fileDownloadOperationConfig);

			FileDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(fileDownloadOperation, cancellationToken);

			emote.FilePath = response.path;

			return await Load(emote, response.path, cancellationToken, avatarId);
		}

	}
}
