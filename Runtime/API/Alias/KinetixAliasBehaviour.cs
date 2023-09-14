using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinetix.Internal
{
    internal static class KinetixAliasBehaviour
    {
        public static async void GetEmoteIDByAlias(string _Alias, Action<string> _OnSuccess, Action _OnFailure)
        {
            try
            {
                string emoteID = await KinetixCoreBehaviour.ManagerLocator.Get<AliasManager>().GetEmoteIDByAlias(_Alias);
                _OnSuccess?.Invoke(emoteID);
            }
            catch (Exception e)
            {
                _OnFailure?.Invoke();
                KinetixDebug.LogWarning("Issue while fetching Alias : " + e.Message);
            }
        }
    }
}

