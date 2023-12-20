using System;
using System.Threading.Tasks;
using Kinetix.Utils;

namespace Kinetix.Internal
{
    public class ByteRangeDownloader : Operation<ByteRangeDownloaderConfig, ByteRangeDownloaderResponse>
    {
        public ByteRangeDownloader(ByteRangeDownloaderConfig _Config): base(_Config) {}

        public override async Task Execute()
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                CurrentTaskCompletionSource.SetCanceled();
                return;
            }
            
            WebRequestDispatcher request = new WebRequestDispatcher();

            ByteResponse response = await request.GetBytes(Config.Url, Config.ByteMin, Config.ByteMax);

            if (CancellationTokenSource.IsCancellationRequested)
            {
                CurrentTaskCompletionSource.SetCanceled();
                return;
            }
            
            if (!response.IsSuccess)
            {
                CurrentTaskCompletionSource.SetException(new Exception("File not downloaded : " + Config.Url));
                return;
            }

            ByteRangeDownloaderResponse fileDownloaderResponse = new ByteRangeDownloaderResponse
            {
                bytes = response.Data
			};

            CurrentTaskCompletionSource.SetResult(fileDownloaderResponse);
        }

        public override bool Compare(ByteRangeDownloaderConfig _Config)
        {
            return Config.Url.Equals(_Config.Url);
        }

        public override IOperation<ByteRangeDownloaderConfig, ByteRangeDownloaderResponse> Clone()
        {
            return new ByteRangeDownloader(Config);
        }
    }
}
