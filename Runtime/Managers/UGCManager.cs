using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Internal
{
    internal class UGCManager : AKinetixManager
    {
        const int FETCH_ANIM_CONTACT_ATTEMPTS = 30;
        const int FETCH_ANIM_REQUEST_TIMEOUT  = 900;

        const int TOKEN_CONTACT_ATTEMPTS = 25;
        const int TOKEN_REQUEST_TIMEOUT  = 375;

        public Action OnUGCTokenExpired;

        private bool   EnableUGC = true;
        private string GameAPIKey;
        private string APIBaseUrl;

        //Cache
        private DateTime lastFetchDate = System.DateTime.MinValue;
        private string   UgcUrl        = string.Empty;
        private string   TokenUUID     = string.Empty;

        //CancellationTokens
        private CancellationTokenSource tokenCancellationGetUgcUrl;
        private CancellationTokenSource tokenCancellationPollingForUgc;
        private CancellationTokenSource tokenCancellationPollingForToken;

        [Serializable]
        public class ObjectUGC
        {
            public string uuid;
            public string url;
        }

        public UGCManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config) {}
        
        protected override void Initialize(KinetixCoreConfiguration _Config)
        {
            Initialize(_Config.GameAPIKey, _Config.APIBaseURL, _Config.EnableUGC);
        }


        protected void Initialize(string _GameAPIKey, string _APIBaseUrl, bool _EnableUGC)
        {
            EnableUGC      = _EnableUGC;
            GameAPIKey = _GameAPIKey;
            APIBaseUrl = _APIBaseUrl;
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
            if (UgcUrl != string.Empty && DateTime.Compare(DateTime.Now, lastFetchDate.AddMinutes(fetchTimeOut)) <= 0) 
                return UgcUrl;
            
            if (KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount == null)
            {
                KinetixDebug.LogWarning(
                    "Unable to find a logged account. Did you use the KineticCore.Account.ConnectAccount method?");
                return string.Empty;
            }
            
            string uri = APIBaseUrl + "/v1/process/token?userId=" +
                         KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AccountId;


            GetRawAPIResultConfig   apiResultOpConfig = new GetRawAPIResultConfig(uri, GameAPIKey);
            GetRawAPIResult         apiResultOp = new GetRawAPIResult(apiResultOpConfig);

            string jsonUgcUrl;
            
            try
            {
                tokenCancellationGetUgcUrl = new CancellationTokenSource();
                GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp, tokenCancellationGetUgcUrl);
                
                jsonUgcUrl = response.json;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            
            lastFetchDate = System.DateTime.Now;

            ObjectUGC oUgc = JsonConvert.DeserializeObject<ObjectUGC>(jsonUgcUrl);
            UgcUrl    = oUgc.url;
            TokenUUID = oUgc.uuid;

            return UgcUrl;
        }

        public async Task<string> GetRetakeUgcUrl(string _Token)
        {
            string rawUrl = await GetUgcUrl();

            string modifiedUrl = rawUrl.Substring(0, rawUrl.IndexOf("?token=") + 7);
            modifiedUrl += _Token;

            return modifiedUrl;
        }

        public async void StartPollingForNewUGCToken()
        {
            if (string.IsNullOrEmpty(TokenUUID))
            {
                KinetixDebug.LogWarning(
                    "Tried to StartPollingForNewUGCToken without having first called or waited GetUgcUrl");
                return;
            }
            
            if (tokenCancellationPollingForToken != null && !tokenCancellationPollingForToken.IsCancellationRequested)
            {
                tokenCancellationPollingForToken?.Cancel();

                await KinetixYield.Yield();
            }

            string url = APIBaseUrl + "/v1/process/token/" + TokenUUID;

            float interval = (float)TOKEN_REQUEST_TIMEOUT / (float)TOKEN_CONTACT_ATTEMPTS;
            GetNewUgcTokenByPollingConfig getNewUgcTokenConfig =
                new GetNewUgcTokenByPollingConfig(url, GameAPIKey, TOKEN_REQUEST_TIMEOUT, interval);
            GetNewUgcTokenByPolling         operation = new GetNewUgcTokenByPolling(getNewUgcTokenConfig);

            try
            {
                tokenCancellationPollingForToken = new CancellationTokenSource();
                await OperationManagerShortcut.Get().RequestExecution(operation, tokenCancellationPollingForToken);
            }
            catch (TimeoutException)
            {
                TokenUUID = string.Empty;
                UgcUrl    = string.Empty;
                OnUGCTokenExpired?.Invoke();
                return;
            }
            catch (Exception)
            {
                return;
            }
            
            TokenUUID = string.Empty;
            UgcUrl    = string.Empty;
            OnUGCTokenExpired?.Invoke();
        }

        public void ClearPolling(bool _ForceCancelUrlFetching = false)
        {
            TokenUUID     = string.Empty;
            UgcUrl        = string.Empty;
            lastFetchDate = System.DateTime.MinValue;

            if (_ForceCancelUrlFetching == false)
                return;
            
            if (tokenCancellationGetUgcUrl != null && !tokenCancellationGetUgcUrl.IsCancellationRequested)
                tokenCancellationGetUgcUrl?.Cancel();
           
            if (tokenCancellationPollingForToken != null && !tokenCancellationPollingForToken.IsCancellationRequested)
                tokenCancellationPollingForToken?.Cancel();
            
            if (tokenCancellationPollingForUgc != null && !tokenCancellationPollingForUgc.IsCancellationRequested)
                tokenCancellationPollingForUgc?.Cancel();
        }

    }
}
