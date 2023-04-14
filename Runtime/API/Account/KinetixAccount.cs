using System;
using System.Collections.Generic;

namespace Kinetix.Internal
{
    public class KinetixAccount
    {
        /// <summary>
        /// Event called upon new connection or disconnection
        /// </summary>
        public event Action OnUpdatedAccount;
        
        
        #region Internal

        public KinetixAccount()
        {
            AccountManager.OnUpdatedAccount += UpdatedAccount;
        }

        private void UpdatedAccount()
        {
            OnUpdatedAccount?.Invoke();
        }

        #endregion
    }
}
