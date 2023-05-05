using System;

namespace Kinetix.Internal
{
    internal static class KinetixAccountBehaviour
    {

        public static async void ConnectAccount(string _UserId, Action<bool> finishedCallback = null)
        {
            bool isSuccess = await AccountManager.ConnectAccount(_UserId);
            
            finishedCallback?.Invoke(isSuccess);
        }
        
        public static void DisconnectAccount()
        {
            AccountManager.DisconnectAccount();
        }

        public static async void AssociateEmotesToUser(AnimationIds emote, Action<bool> finishedCallback = null)
        {
            bool isSuccess = await AccountManager.AssociateEmotesToUser(emote);

            finishedCallback?.Invoke(isSuccess);
        }

    }
}

