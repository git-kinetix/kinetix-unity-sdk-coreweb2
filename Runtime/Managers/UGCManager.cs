using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Internal
{
    internal class UGCManager: AKinetixManager
    {
        const int FETCH_ANIM_CONTACT_ATTEMPTS = 30;
        const int FETCH_ANIM_REQUEST_TIMEOUT  = 900;

        const int TOKEN_CONTACT_ATTEMPTS = 25;
        const int TOKEN_REQUEST_TIMEOUT  = 375;

        public Action OnUGCTokenExpired;

        private bool   EnableUGC = true;
        private string VirtualWorldId;

        //Cache
        private DateTime lastFetchDate = System.DateTime.MinValue;
        private string   UgcUrl        = string.Empty;
        private string   TokenUUID     = string.Empty;


        [Serializable]
        public class ObjectUGC
        {
            public string uuid;
            public string url;
        }


        public UGCManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config) {}


        protected override void Initialize(KinetixCoreConfiguration _Config)
        {
            Initialize(_Config.VirtualWorldKey, _Config.EnableUGC);
        }

        protected void Initialize(string _VirtualWorldId, bool _EnableUGC)
        {
            EnableUGC      = _EnableUGC;
            VirtualWorldId = _VirtualWorldId;
        }
        
        public bool IsUGCAvailable()
        {
            return KinetixConstants.C_ShouldUGCBeAvailable
                   && EnableUGC
                   && KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount != null;
        }

        public async Task<string> GetUgcUrl()
        {
            int fetchTimeOut = 5;

            //if Now is earlier than lastFetchDate+5 minutes
            if (UgcUrl == string.Empty ||
                DateTime.Compare(DateTime.Now, lastFetchDate.AddMinutes(fetchTimeOut)) > 0)
            {
                if (KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount == null)
                {
                    KinetixDebug.LogWarning(
                        "Unable to find a logged account. Did you use the KineticCore.Account.ConnectAccount method?");
                    return string.Empty;
                }

                
                
                string uri = KinetixConstants.c_SDK_API_URL + "/v1/process/token?userId=" +
                             KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AccountId;

                GetRawAPIResultConfig   apiResultOpConfig = new GetRawAPIResultConfig(uri, VirtualWorldId);
                GetRawAPIResult         apiResultOp = new GetRawAPIResult(apiResultOpConfig);
                GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp);

                string jsonUgcUrl = response.json;
                lastFetchDate = System.DateTime.Now;

                ObjectUGC oUgc = JsonConvert.DeserializeObject<ObjectUGC>(jsonUgcUrl);
                UgcUrl    = oUgc.url;
                TokenUUID = oUgc.uuid;

                return UgcUrl;
            }

            return UgcUrl;
        }

        public async void StartPollingForUGC()
        {
            if (KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount == null)
            {
                KinetixDebug.LogWarning(
                    "Unable to find a logged account. Did you use the KineticCore.Account.ConnectAccount method?");
                return;
            }

            string url = KinetixConstants.c_SDK_API_URL + "/v1/users/" + KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AccountId +
                         "/emotes";

            float interval = (float)FETCH_ANIM_REQUEST_TIMEOUT / (float)FETCH_ANIM_CONTACT_ATTEMPTS;
            GetNewUgcEmoteByPollingConfig getNewUgcEmoteConfig =
                new GetNewUgcEmoteByPollingConfig(url, VirtualWorldId, FETCH_ANIM_REQUEST_TIMEOUT, interval);
            GetNewUgcEmoteByPolling         operation = new GetNewUgcEmoteByPolling(getNewUgcEmoteConfig);
            GetNewUgcEmoteByPollingResponse getNewUgcEmoteResponse;

            try
            {
                getNewUgcEmoteResponse = await OperationManagerShortcut.Get().RequestExecution(operation);
            }
            catch (Exception)
            {
                return;
            }

            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AddEmoteFromMetadata(getNewUgcEmoteResponse.newAnimationMetadata);
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().OnUpdatedAccount?.Invoke();
        }

        public async void StartPollingForNewUGCToken()
        {
            if (string.IsNullOrEmpty(TokenUUID))
            {
                KinetixDebug.LogWarning(
                    "Tried to StartPollingForNewUGCToken without having first called or waited GetUgcUrl");
                return;
            }

            string url = KinetixConstants.c_SDK_API_URL + "/v1/process/token/" + TokenUUID;

            float interval = (float)TOKEN_REQUEST_TIMEOUT / (float)TOKEN_CONTACT_ATTEMPTS;
            GetNewUgcTokenByPollingConfig getNewUgcTokenConfig =
                new GetNewUgcTokenByPollingConfig(url, VirtualWorldId, TOKEN_REQUEST_TIMEOUT, interval);
            GetNewUgcTokenByPolling         operation = new GetNewUgcTokenByPolling(getNewUgcTokenConfig);
            GetNewUgcTokenByPollingResponse getNewUgcTokenResponse;

            try
            {
                getNewUgcTokenResponse = await OperationManagerShortcut.Get().RequestExecution(operation);
            }
            catch (Exception)
            {
                return;
            }

            if (!getNewUgcTokenResponse.IsTokenOutdated)
                return;

            TokenUUID = string.Empty;
            UgcUrl    = string.Empty;
            OnUGCTokenExpired?.Invoke();
        }
    }
}
