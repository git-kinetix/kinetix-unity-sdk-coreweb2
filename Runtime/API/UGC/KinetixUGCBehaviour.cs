using System;

namespace Kinetix.Internal
{
    internal static class KinetixUGCBehaviour
    {
        public static void StartPollingForUGC()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().StartPollingForUGC();
        }

        public static void StartPollingForNewUGCToken()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().StartPollingForNewUGCToken();
        }


        public static async void GetUgcUrl(Action<string> urlFetchedCallback)
        {
            string url = await KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().GetUgcUrl();
            
            urlFetchedCallback(url);
        }

        public static bool IsUGCAvailable()
        {
            return KinetixCoreBehaviour.ManagerLocator.Get<UGCManager>().IsUGCAvailable();
        }
    }
}
