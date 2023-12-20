using System;

namespace Kinetix.Internal
{
    public class KinetixAccount
    {
        /// <summary>
        /// Event called upon new connection or disconnection
        /// </summary>
        public event Action OnUpdatedAccount;

        /// <summary>
        /// Event called upon new account connected
        /// </summary>
        public event Action OnConnectedAccount;

        /// <summary>
        /// Event called upon account disconnection
        /// </summary>
        public event Action OnDisconnectedAccount;
        
        /// <summary>
        /// Connect account with UserId
        /// </summary>
        /// <param name="_UserId">UserId of user</param>
        public void ConnectAccount(string _UserId, Action _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixAccountBehaviour.ConnectAccount(_UserId, _OnSuccess, _OnFailure);
        }

        /// <summary>
        /// Disconnect account with account address
        /// </summary>
        public void DisconnectAccount()
        {
            KinetixAccountBehaviour.DisconnectAccount();
        }

        /// <summary>
        /// Allows a user to use the wanted emote
        /// </summary>
        /// <param name="_EmoteID">UUID (unique id) of the emote</param>
        [Obsolete("Kinetix SDK no longer supports emote association, this will be removed in the next version")]
        public void AssociateEmotesToUser(string _EmoteID, Action _OnSuccess = null, Action<string> _OnFailure = null)
        {
            KinetixAccountBehaviour.AssociateEmotesToUser(_EmoteID, _OnSuccess, _OnFailure);
        }
        
        #region Internal

        public KinetixAccount()
        {
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().OnUpdatedAccount      += UpdatedAccount;
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().OnConnectedAccount    += ConnectedAccount;
            KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().OnDisconnectedAccount += DisconnectedAccount;
        }

        private void UpdatedAccount()
        {
            OnUpdatedAccount?.Invoke();
        }

        private void ConnectedAccount()
        {
            OnConnectedAccount?.Invoke();
        }

        private void DisconnectedAccount()
        {
            OnDisconnectedAccount?.Invoke();
        }

        #endregion
    }
}
