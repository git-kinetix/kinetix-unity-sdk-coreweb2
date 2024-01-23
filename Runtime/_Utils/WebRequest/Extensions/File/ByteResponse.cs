// // ----------------------------------------------------------------------------
// // <copyright file="FileResponse.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEngine.Networking;

namespace Kinetix.Utils
{
    internal class ByteResponse: IResponse
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public long ResponseCode { get; set; }
        
        public byte[] Data { get; private set; }

        public void Parse(UnityWebRequest request)
        {
            if (request.downloadHandler is DownloadHandlerBuffer downloadHandler)
            {
                Data = downloadHandler.data;
            }
		}
    }
}
