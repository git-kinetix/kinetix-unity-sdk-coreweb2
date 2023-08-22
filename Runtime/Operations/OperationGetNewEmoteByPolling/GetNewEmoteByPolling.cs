using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kinetix.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Internal
{
    public class GetNewEmoteByPolling : Operation<GetNewEmoteByPollingConfig, GetNewEmoteByPollingResponse>
    {
        public GetNewEmoteByPolling(GetNewEmoteByPollingConfig forNewUgcEmoteConfig) : base(forNewUgcEmoteConfig)
        {
        }

        public override async Task Execute()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json" },
                { "Accept", "application/json" },
                { "x-api-key", Config.ApiKey }
            };

            WebRequestDispatcher webRequest = new WebRequestDispatcher();
            
            while (!CurrentTaskCompletionSource.Task.IsCompleted)
            {
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    CurrentTaskCompletionSource.TrySetCanceled();
                    return;
                }
                
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
                    // Then try getting the emotes again
                    List<SdkApiUserAsset> fetchedCollection = JsonConvert.DeserializeObject<SdkApiUserAsset[]>(json).ToList();
                    fetchedCollection.RemoveAll(asset => asset.data == null);
                    
                    KinetixEmote[] userEmotes = KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.Emotes.ToArray();
                    if (fetchedCollection.Count > userEmotes.Length)
                    {
                        List<AnimationMetadata> userAnimationMetadatas    = userEmotes.Select(emote => emote.Metadata).ToList();
                        List<AnimationMetadata> fetchedAnimationMetadatas = new List<AnimationMetadata>();
                        fetchedCollection.ToList().ForEach(userAsset => fetchedAnimationMetadatas.Add(userAsset.ToAnimationMetadata()));

                        HashSet<string> userUUIDs = new HashSet<string>(userAnimationMetadatas.Select(userMetadata => userMetadata.Ids.UUID));
                        fetchedAnimationMetadatas.RemoveAll(fetchedMetadata => userUUIDs.Contains(fetchedMetadata.Ids.UUID));

                        AnimationMetadata[] newEmotesMetadata = fetchedAnimationMetadatas.ToArray();
                        GetNewEmoteByPollingResponse result = new GetNewEmoteByPollingResponse()
                        {
                            newAnimationsMetadata = newEmotesMetadata
                        };

                        CurrentTaskCompletionSource.TrySetResult(result);
                        return;
                    }
                }

                await TaskUtils.Delay(Config.IntervalTimeInSeconds);
            }

            CurrentTaskCompletionSource.TrySetException(new TimeoutException());
        }

        public override bool Compare(GetNewEmoteByPollingConfig forNewUgcEmoteByPollingConfig)
        {
            return Config.Url == forNewUgcEmoteByPollingConfig.Url;
        }

        public override IOperation<GetNewEmoteByPollingConfig, GetNewEmoteByPollingResponse> Clone()
        {
            return new GetNewEmoteByPolling(Config);
        }
    }
}
