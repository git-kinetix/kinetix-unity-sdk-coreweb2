using System;

namespace Kinetix.Internal
{
    internal static class KinetixAccountBehaviour
    {

        public static async void ConnectAccount(string _UserId, Action _OnSuccess = null, Action _OnFailure = null)
        {
            bool isSuccess = await AccountManager.ConnectAccount(_UserId);

            if (isSuccess)
            {
                _OnSuccess?.Invoke();
            } else {
                _OnFailure?.Invoke();
            }
        }
        
        public static void DisconnectAccount()
        {
            AccountManager.DisconnectAccount();
        }

        public static async void AssociateEmotesToUser(AnimationIds emote, Action _OnSuccess = null, Action _OnFailure = null)
        {
            bool isSuccess = await AccountManager.AssociateEmotesToUser(emote);

            if (isSuccess)
            {
                _OnSuccess?.Invoke();
            } else
            {
                _OnFailure?.Invoke();
            }
        }

    }
}

