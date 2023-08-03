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

        public static async void AssociateEmotesToUser(string emote, Action _OnSuccess = null, Action<string> _OnFailure = null)
        {
            try
            {
                bool isSuccess = await KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().AssociateEmotesToUser(new AnimationIds(emote));

                if (isSuccess)
                {
                    _OnSuccess?.Invoke();
                } else
                {
                    // We did not received a message for failure via exception
                    _OnFailure?.Invoke("");
                }
            }
            catch (Exception e)
            {
                _OnFailure?.Invoke(e.Message);
            }
        }

    }
}

