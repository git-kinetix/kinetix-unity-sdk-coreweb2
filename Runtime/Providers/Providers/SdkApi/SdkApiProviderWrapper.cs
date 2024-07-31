// // ----------------------------------------------------------------------------
// // <copyright file="SdkApiProviderWrapper.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kinetix.Internal;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Utils
{
	public class SdkApiProviderWrapper : IProviderWrapper
	{
		private string GameAPIKey = string.Empty;

		public SdkApiProviderWrapper(string _GameAPIKey)
		{
			GameAPIKey = _GameAPIKey;
		}

		/// <summary>
		/// Make a Web Request to get all the NFTs Metadata of the User's Wallet
		/// </summary>
		public async Task<AnimationMetadata[]> GetAnimationsMetadataOfOwner(string _AccountId)
		{
			TaskCompletionSource<AnimationMetadata[]> tcs = new TaskCompletionSource<AnimationMetadata[]>();

			try
			{
				GetAnimationsMetadataOfOwnerInternal(_AccountId, null, null, (metadatas) =>
				{
					tcs.SetResult(metadatas);   
				});
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
			
			return await tcs.Task;
		}

		private async void GetAnimationsMetadataOfOwnerInternal(string _AccountId, List<AnimationMetadata> _AnimationMetadatas = null, string _PageKey = null, Action<AnimationMetadata[]> _OnSuccess = null)
		{
			string uri = KinetixConstants.c_SDK_API_URL + "/v1/users/" + _AccountId + "/emotes";

			_AnimationMetadatas ??= new List<AnimationMetadata>();

			MetadataDownloaderConfig   metadataDownloaderConfig = new MetadataDownloaderConfig(uri, GameAPIKey);
			MetadataDownloader         metadataDownloader       = new MetadataDownloader(metadataDownloaderConfig);
			MetadataDownloaderResponse response = await OperationManagerShortcut.Get().RequestExecution(metadataDownloader);

		   
			string result = response.json;

			SdkApiUserAsset[] collection = JsonConvert.DeserializeObject<SdkApiUserAsset[]>(result);

			if (collection == null)
			{
				KinetixDebug.LogWarning("API provided no results when fetching owner's emotes");
				return;
			}

			try
			{
				for (int i = 0; i < collection.Length; i++)
				{
					AnimationMetadata metadata = collection[i].ToAnimationMetadata();

					if (metadata != null)
						_AnimationMetadatas.Add(metadata);
				}
				
				_OnSuccess?.Invoke(_AnimationMetadatas.ToArray());
				
			}
			catch (Exception e)
			{
				throw e;
			}
		}


		/// <summary>
		/// Make a Web Request to get metadata of specific Emote
		/// </summary>
		public async Task<AnimationMetadata> GetAnimationMetadataOfEmote(AnimationIds _AnimationIds)
		{
			TaskCompletionSource<AnimationMetadata> tcs = new TaskCompletionSource<AnimationMetadata>();

			string uri = KinetixConstants.c_SDK_API_URL + "/v1/emotes/" + _AnimationIds.UUID;

			GetRawAPIResultConfig   apiResultOpConfig = new GetRawAPIResultConfig(uri, GameAPIKey);
			GetRawAPIResult         apiResultOp       = new GetRawAPIResult(apiResultOpConfig);
			GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp);

			string result = response.json;

			try
			{
				SdkApiAsset collection = JsonConvert.DeserializeObject<SdkApiAsset>(result);
				tcs.SetResult(collection.ToAnimationMetadata());
			}
			catch (ArgumentNullException e)
			{
				tcs.SetException(e);
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}

			return await tcs.Task;
		}

        public async Task<AnimationMetadata> GetAnimationMetadataOfAvatar(AnimationIds _AnimationIds, string _AvatarId, AnimationMetadata _Metadata = null)
        {
			TaskCompletionSource<AnimationMetadata> tcs = new TaskCompletionSource<AnimationMetadata>();

			string uri = KinetixConstants.c_SDK_API_URL + "/v1/emotes/" + _AnimationIds.UUID + "/avatar/" + _AvatarId;

			GetRawAPIResultConfig apiResultOpConfig = new GetRawAPIResultConfig(uri, GameAPIKey);
			GetRawAPIResult apiResultOp = new GetRawAPIResult(apiResultOpConfig);
			GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp);
			
			string result = response.json;
			if (response.raw.ResponseCode == 404)
			{
				KinetixDebug.LogError($"\"{nameof(GetAnimationMetadataOfAvatar)}\" returned (404)... Retrying using \"{nameof(GetAnimationMetadataOfEmote)}\"...");
				return await GetAnimationMetadataOfEmote(_AnimationIds);
			}
			
			try
			{
				SdkAvatarApiAsset collection = JsonConvert.DeserializeObject<SdkAvatarApiAsset>(result);
				collection.EditAnimationMetadata(ref _Metadata, _AvatarId);
				tcs.SetResult(_Metadata);
			}
			catch (ArgumentNullException e)
			{
				tcs.SetException(e);
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}

			return await tcs.Task;
		}

		public async Task<SdkApiProcess[]> GetAnimationProcessesOfOwner(string _AccountId)
		{
			TaskCompletionSource<SdkApiProcess[]> tcs = new TaskCompletionSource<SdkApiProcess[]>();

			string uri = KinetixConstants.c_SDK_API_URL + "/v2/users/" + _AccountId + "/processes";

			GetRawAPIResultConfig apiResultOpConfig = new GetRawAPIResultConfig(uri, GameAPIKey);
			GetRawAPIResult apiResultOp = new GetRawAPIResult(apiResultOpConfig);
			GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp);
			
			string result = response.json;

			if (response.raw.ResponseCode == 404)
			{
				KinetixDebug.LogError($"\"{nameof(GetAnimationProcessesOfOwner)}\" returned (404)... Retrying using \"{nameof(GetAnimationProcessesOfOwner)}\"...");
				return await GetAnimationProcessesOfOwner(_AccountId);
			}
			
			try
			{
				SdkApiProcess[] collection = JsonConvert.DeserializeObject<SdkApiProcess[]>(result);
				tcs.SetResult(collection);
			}
			catch (ArgumentNullException e)
			{
				tcs.SetException(e);
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}

			return await tcs.Task;
		}

		public async Task<SdkApiProcess> ValidateEmote(string _ProcessId)
		{
			TaskCompletionSource<SdkApiProcess> tcs = new TaskCompletionSource<SdkApiProcess>();

			string url = KinetixConstants.c_SDK_API_URL + "/v1/process/" + _ProcessId + "/validate";
            
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json" },
                { "Accept", "application/json" },
                { "x-api-key", GameAPIKey },
				//{ "sdk-version", KinetixConstants.SDK_USER_AGENT}
            };
            
            try
            {
                WebRequestDispatcher webRequest = new WebRequestDispatcher();

                RawResponse response = await webRequest.SendRequest<RawResponse>(url, WebRequestDispatcher.HttpMethod.POST, headers);

                if (!response.IsSuccess)
                    throw new Exception(response.Error);

				tcs.SetResult(JsonConvert.DeserializeObject<SdkApiProcess>(response.Content));
            }
            catch (Exception e)
            {
                throw e;
            }

			return await tcs.Task;
		}

		public async Task<SdkApiProcess> RetakeEmote(string _ProcessId)
		{
			TaskCompletionSource<SdkApiProcess> tcs = new TaskCompletionSource<SdkApiProcess>();

			string url = KinetixConstants.c_SDK_API_URL + "/v1/process/" + _ProcessId + "/retake";
            
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json" },
                { "Accept", "application/json" },
                { "x-api-key", GameAPIKey },
				//{ "sdk-version", KinetixConstants.SDK_USER_AGENT}
            };
            
            try
            {
                WebRequestDispatcher webRequest = new WebRequestDispatcher();

                RawResponse response = await webRequest.SendRequest<RawResponse>(url, WebRequestDispatcher.HttpMethod.POST, headers);

                if (!response.IsSuccess)
                    throw new Exception(response.Error);

				tcs.SetResult(JsonConvert.DeserializeObject<SdkApiProcess>(response.Content));
            }
            catch (Exception e)
            {
                throw e;
            }

			return await tcs.Task;
		}
    }
}
