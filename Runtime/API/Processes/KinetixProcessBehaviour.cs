using System;

namespace Kinetix.Internal
{
    internal static class KinetixProcessBehaviour
    {
        public static void GetConnectedAccountProcesses(Action<SdkApiProcess[]> _OnSuccess, Action _OnFailure)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().GetAllUserProcesses(_OnSuccess, _OnFailure);
        }

        public static void ValidateEmoteProcess(string _ProcessId, Action<SdkApiProcess> _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().ValidateEmoteProcess(_ProcessId, _OnSuccess, _OnFailure);
        }

        public static void RetakeEmoteProcess(string _ProcessId, Action<string> _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().RetakeEmoteProcess(_ProcessId, _OnSuccess, _OnFailure);
        }

    }
}

