using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Kinetix.Utils
{
    public static class WebRequestFileDownloadExtension
    {
        internal static async Task<FileResponse> GetFile(this WebRequestDispatcher _WebRequest, string _Url, string _Path, CancellationToken _Ctx = new CancellationToken(), int _Timeout = WebRequestDispatcher.TIMEOUT)
        {
            DownloadHandlerFile downloadHandler = new DownloadHandlerFile(_Path)
            {
                removeFileOnAbort = true
            };

            return await _WebRequest.SendRequest<FileResponse>(_Url, WebRequestDispatcher.HttpMethod.GET, _DownloadHandler: downloadHandler, _Ctx: _Ctx);
        }
        internal static async Task<ByteResponse> GetBytes(this WebRequestDispatcher _WebRequest, string _Url, long _ByteMin, long _ByteMax, CancellationToken _Ctx = new CancellationToken(), int _Timeout = WebRequestDispatcher.TIMEOUT)
        {
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();

            Dictionary<string, string> _Headers = new Dictionary<string, string>()
            {
                { "Range",$"bytes={_ByteMin}-{_ByteMax}" }
            };
            return await _WebRequest.SendRequest<ByteResponse>(_Url, WebRequestDispatcher.HttpMethod.GET, _Headers, _DownloadHandler: downloadHandler, _Ctx: _Ctx);
        }
    }
}
