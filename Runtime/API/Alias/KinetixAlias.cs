using System;

namespace Kinetix.Internal
{
    public class KinetixAlias
    {
        public void GetEmoteIDByAlias(string _Alias, Action<string> _OnSuccess, Action _OnFailure = null)
        {
            KinetixAliasBehaviour.GetEmoteIDByAlias(_Alias, _OnSuccess, _OnFailure);
        }
    }
}

