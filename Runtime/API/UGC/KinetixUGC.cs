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

        public void GetUgcUrl(Action<string> urlFetchedCallback)
        {
            KinetixUGCBehaviour.GetUgcUrl(urlFetchedCallback);
        }

        private void UGCTokenExpired()
        {
            OnUGCTokenExpired?.Invoke();
        }
    }
}
