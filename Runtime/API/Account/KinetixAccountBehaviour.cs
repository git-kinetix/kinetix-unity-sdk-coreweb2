using System;

namespace Kinetix.Internal
{
    internal static class KinetixAccountBehaviour
    {
        public static async void ConnectAccount(string _UserId, Action _OnSuccess = null, Action _OnFailure = null)
        {
            bool isSuccess = await KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().ConnectAccount(_UserId);

            if (isSuccess)
            {
                _OnSuccess?.Invoke();
            } else {
                _OnFailure?.Invoke();
            }
        }
        
        public static void DisconnectAccount()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().DisconnectAccount();
        }

    }
}

