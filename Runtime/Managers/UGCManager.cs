using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kinetix.Utils;
using Newtonsoft.Json;

namespace Kinetix.Internal
{
    internal static class UGCManager
    {
        const int FETCH_ANIM_CONTACT_ATTEMPTS = 30;
        const int FETCH_ANIM_REQUEST_TIMEOUT = 900;

        const int TOKEN_CONTACT_ATTEMPTS = 25;
        const int TOKEN_REQUEST_TIMEOUT = 375;

        public static Action OnUGCTokenExpired;

        private static bool EnableUGC = true;
        private static string VirtualWorldId;

        //Cache
        private static DateTime lastFetchDate      = System.DateTime.MinValue;
        private static string UgcUrl               = string.Empty;             
        private static string TokenUUID               = string.Empty;             

        struct ObjectUGC
        {
            public string uuid;
            public string url;
        }




        public static void Initialize(string _VirtualWorldId, bool _EnableUGC)
        {
            EnableUGC = _EnableUGC;
            VirtualWorldId = _VirtualWorldId;
        }


        public static bool IsUGCAvailable()
        {
            return KinetixConstants.C_ShouldUGCBeAvailable
            && EnableUGC
            && AccountManager.LoggedAccount != null;
        }

        public static async Task<string> GetUgcUrl()
        {
            int fetchTimeOut = 5;

            //if Now is earlier than lastFetchDate+5 minutes
            if(UgcUrl == string.Empty || System.DateTime.Compare(System.DateTime.Now, lastFetchDate.AddMinutes(fetchTimeOut)) > 0 )
            {
                if (AccountManager.LoggedAccount == null)
                {
                    throw new Exception("Unable to find a logged account. Did you use the KineticCore.Account.ConnectAccount method?");
                }
                
                string uri = KinetixConstants.c_SDK_API_URL + "/v1/process/token?userId=" + AccountManager.LoggedAccount.AccountId;

                KeyValuePair<string, string>[] headers = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("x-api-key", VirtualWorldId)
                };

                string jsonUgcUrl = await WebRequestHandler.Instance.GetAsyncRaw(uri, headers, null);
                lastFetchDate = System.DateTime.Now;                

                ObjectUGC oUgc = JsonConvert.DeserializeObject<ObjectUGC>(jsonUgcUrl);
                UgcUrl = oUgc.url;
                TokenUUID = oUgc.uuid;

                return UgcUrl;
            } 
            else
            {                
                return UgcUrl;
            }            
        }

        public static async void StartPollingForUGC()
        {
            if (AccountManager.LoggedAccount == null)
            {
                throw new Exception("Unable to find a logged account. Did you use the KineticCore.Account.ConnectAccount method?");
            }

            // Try to get the emotes
            KinetixEmote[] currentMetadata = AccountManager.LoggedAccount.Emotes.ToArray();

            string url = KinetixConstants.c_SDK_API_URL + "/v1/users/" + AccountManager.LoggedAccount.AccountId + "/emotes";

            KeyValuePair<string, string>[] headers = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("x-api-key", VirtualWorldId)
            };

            await WebRequestHandler.Instance.PollUrl(url, headers, (jsonDone) => {

                if (jsonDone == string.Empty || jsonDone == WebRequestHandler.ERROR_RECEIVED)
                    return false;

                // Then try getting the emotes again
                SdkApiUserAsset[] collection = JsonConvert.DeserializeObject<SdkApiUserAsset[]>(jsonDone);

                if (collection.Length > currentMetadata.Length) {
                    AnimationMetadata newEmoteMetadata = collection[collection.Length - 1].ToAnimationMetadata();
                    AccountManager.LoggedAccount.AddEmoteFromMetadata(newEmoteMetadata);

                    AccountManager.OnUpdatedAccount?.Invoke();
                }

                

                return collection.Length > currentMetadata.Length;

            }, FETCH_ANIM_CONTACT_ATTEMPTS, FETCH_ANIM_REQUEST_TIMEOUT);
        }

        public static async void StartPollingForNewUGCToken()
        {
            if (String.IsNullOrEmpty(TokenUUID))
            {
                KinetixDebug.LogError("Tried to StartPollingForNewUGCToken without having first called or waited GetUgcUrl");
                return;
            }

            string url = KinetixConstants.c_SDK_API_URL + "/v1/process/token/" + TokenUUID;

            KeyValuePair<string, string>[] headers = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("x-api-key", VirtualWorldId)
            };

            await WebRequestHandler.Instance.PollUrl(url, headers, (jsonResult) => {
                if (jsonResult == string.Empty || jsonResult == WebRequestHandler.ERROR_RECEIVED)
                {
                    TokenUUID = string.Empty;
                    UgcUrl = string.Empty;
                    OnUGCTokenExpired?.Invoke();
                    return true;
                }

                SdkTokenValidityResult tokenValidity = JsonConvert.DeserializeObject<SdkTokenValidityResult>(jsonResult);

                if (tokenValidity == null)
                {
                    KinetixDebug.LogError("Unable to parse response from API for the token validity");
                    return false;
                }

                if (tokenValidity.expireIn < 1)
                {
                    TokenUUID = string.Empty;
                    UgcUrl = string.Empty;
                    OnUGCTokenExpired?.Invoke();
                    return true;
                }

                return false;

            }, TOKEN_CONTACT_ATTEMPTS, TOKEN_REQUEST_TIMEOUT);
        }
    }
}
