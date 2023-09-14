using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kinetix.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Kinetix.Internal
{
    public class GetEmoteIDByAlias : Operation<GetEmoteIDByAliasConfig, GetEmoteIDByAliasResponse>
    {
        public GetEmoteIDByAlias(GetEmoteIDByAliasConfig config) : base(config)
        {
        }

        public override async Task Execute()
        {
            if (CancellationTokenSource.IsCancellationRequested)
            {
                CurrentTaskCompletionSource.TrySetCanceled();
                return;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json" },
                { "Accept", "application/json" },
                { "x-api-key", Config.ApiKey }
            };

            WebRequestDispatcher webRequest = new WebRequestDispatcher();
            RawResponse response = await webRequest.SendRequest<RawResponse>(Config.Url, WebRequestDispatcher.HttpMethod.GET,
                headers, null, default, CancellationTokenSource.Token);
            
            if (CancellationTokenSource.IsCancellationRequested)
            {
                CurrentTaskCompletionSource.TrySetCanceled();
                return;
            }
            
            string json = response.Content;
            if (response.IsSuccess && json != string.Empty)
            {
                AliasData aliasData = JsonConvert.DeserializeObject<AliasData>(json);
                string    emoteID   = aliasData.emoteUuid;

                GetEmoteIDByAliasResponse opResponse = new GetEmoteIDByAliasResponse(emoteID);
                CurrentTaskCompletionSource.SetResult(opResponse);
            }
            else
            {
                CurrentTaskCompletionSource.SetException(new Exception(response.Error));
            }
        }

        public override bool Compare(GetEmoteIDByAliasConfig _Config)
        {
            return Config.Url == _Config.Url;
        }

        public override IOperation<GetEmoteIDByAliasConfig, GetEmoteIDByAliasResponse> Clone()
        {
            return new GetEmoteIDByAlias(Config);
        }
        
        [Serializable]
        public class AliasData
        {
            [SerializeField] public string emoteUuid;
        }
    }
}
