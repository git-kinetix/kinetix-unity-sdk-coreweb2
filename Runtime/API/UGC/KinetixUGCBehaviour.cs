using System;

namespace Kinetix.Internal
{
    internal static class KinetixUGCBehaviour
    {
        public static void StartPollingForUGC()
        {
            UGCManager.StartPollingForUGC();
        }

        public static void StartPollingForNewUGCToken()
        {
            UGCManager.StartPollingForNewUGCToken();
        }


        public static async void GetUgcUrl(Action<string> urlFetchedCallback)
        {
            string url = await UGCManager.GetUgcUrl();
            
            urlFetchedCallback(url);
        }

        public static bool IsUGCAvailable()
        {
            return UGCManager.IsUGCAvailable();
        }
    }
}
