using System;

namespace Kinetix.Internal
{
    public class KinetixAlias
    {
        [Obsolete("Alias feaure is no longer supported and will be removed in next version")]
        public void GetEmoteIDByAlias(string _Alias, Action<string> _OnSuccess, Action _OnFailure = null)
        {
            KinetixAliasBehaviour.GetEmoteIDByAlias(_Alias, _OnSuccess, _OnFailure);
        }
    }
}

