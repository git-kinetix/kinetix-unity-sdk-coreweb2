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

        public void AssociateEmotesToUser(AnimationIds emote, Action _OnSuccess = null, Action _OnFailure = null)
        {
            KinetixAccountBehaviour.AssociateEmotesToUser(emote, _OnSuccess, _OnFailure);
        }

        

        
        
        #region Internal

        public KinetixAccount()
        {
            AccountManager.OnUpdatedAccount += UpdatedAccount;
            AccountManager.OnConnectedAccount += ConnectedAccount;
        }

        private void UpdatedAccount()
        {
            OnUpdatedAccount?.Invoke();
        }

        private void ConnectedAccount()
        {
            OnConnectedAccount?.Invoke();
        }

        #endregion
    }
}
