using System;

namespace Kinetix.Internal
{
    public class KinetixUGC
    {
        /// <summary>
        /// Event called upon token expiration
        /// </summary>
        public event Action OnUGCTokenExpired;

        public bool IsUGCAvailable()
        {
            return KinetixUGCBehaviour.IsUGCAvailable();
        }

        public void StartPollingForUGC()
        {
            KinetixUGCBehaviour.StartPollingForUGC();
        }

        public void StartPollingForNewUGCToken()
        {
            KinetixUGCBehaviour.StartPollingForNewUGCToken();
        }

        public void GetUgcUrl(Action<string> urlFetchedCallback)
        {
            KinetixUGCBehaviour.GetUgcUrl(urlFetchedCallback);
        }

        public KinetixUGC()
        {
            UGCManager.OnUGCTokenExpired += UGCTokenExpired;
        }

        private void UGCTokenExpired()
        {
            OnUGCTokenExpired?.Invoke();
        }
    }
}
