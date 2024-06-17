using System;

namespace Kinetix.Internal
{
    internal static class KinetixUGCBehaviour
    {
        public static void StartPollingForNewUGCToken()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().StartPollingForNewUGCToken();
        }


        public static async void GetUgcUrl(Action<string> _UrlFetchedCallback)
        {
            string url = await KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().GetUgcUrl();
            
            _UrlFetchedCallback?.Invoke(url);
        }

        public static async void GetRetakeUgcUrl(string _RetakeToken, Action<string> _UrlFetchedCallback)
        {
            string url = await KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().GetRetakeUgcUrl(_RetakeToken);
            
            _UrlFetchedCallback?.Invoke(url);
        }

        public static bool IsUGCAvailable()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().IsUGCAvailable();
        }
    }
}
