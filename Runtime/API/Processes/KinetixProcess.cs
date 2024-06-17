using System;

namespace Kinetix.Internal
{
    public class KinetixProcess
    {
        public void GetConnectedAccountProcesses(Action<SdkApiProcess[]> _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixProcessBehaviour.GetConnectedAccountProcesses(_OnSuccess, _OnFailure);
        }

        public void ValidateEmoteProcess(string _ProcessId, Action<SdkApiProcess> _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixProcessBehaviour.ValidateEmoteProcess(_ProcessId, _OnSuccess, _OnFailure);
        }

        public void RetakeEmoteProcess(string _ProcessId, Action<string> _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixProcessBehaviour.RetakeEmoteProcess(_ProcessId, _OnSuccess, _OnFailure);
        }
    }
}
