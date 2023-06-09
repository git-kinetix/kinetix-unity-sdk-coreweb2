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

        /// <summary>
        /// Allows a user to use the wanted emote
        /// </summary>
        /// <param name="_EmoteID">AnimationIds of the emote</param>
        [Obsolete("Please use the overload with (string, Action, Action).", false)]
        public void AssociateEmotesToUser(AnimationIds _EmoteID, Action _OnSuccess = null, Action<string> _OnFailure = null)
        {
            KinetixAccountBehaviour.AssociateEmotesToUser(_EmoteID.UUID, _OnSuccess, _OnFailure);
        }

        /// <summary>
        /// Allows a user to use the wanted emote
        /// </summary>
        /// <param name="_EmoteUUID">UUID (unique id) of the emote</param>
        public void AssociateEmotesToUser(string _EmoteUUID, Action _OnSuccess = null, Action<string> _OnFailure = null)
        {
            KinetixAccountBehaviour.AssociateEmotesToUser(_EmoteUUID, _OnSuccess, _OnFailure);
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
