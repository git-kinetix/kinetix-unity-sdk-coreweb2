using System;

namespace Kinetix.Internal
{
    public class KinetixUGC
    {
        /// <summary>
        /// Event called upon token expiration
        /// </summary>
        public event Action OnUGCTokenExpired;

        public KinetixUGC()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().OnUGCTokenExpired += UGCTokenExpired;
        }
        
        public bool IsUGCAvailable()
        {
            return KinetixUGCBehaviour.IsUGCAvailable();
        }

        public void StartPollingForNewUGCToken()
        {
            KinetixUGCBehaviour.StartPollingForNewUGCToken();

        }

        public void GetUgcUrl(Action<string> _UrlFetchedCallback)
        {
            KinetixUGCBehaviour.GetUgcUrl(_UrlFetchedCallback);
        }

        public void GetRetakeUgcUrl(string _RetakeToken, Action<string> _UrlFetchedCallback)
        {
            KinetixUGCBehaviour.GetRetakeUgcUrl(_RetakeToken, _UrlFetchedCallback);
        }

        public void ClearCachedUrl()
        {
            KinetixUGCBehaviour.ClearCachedUrl();
        } 

        private void UGCTokenExpired()
        {
            OnUGCTokenExpired?.Invoke();
        }
    }
}
