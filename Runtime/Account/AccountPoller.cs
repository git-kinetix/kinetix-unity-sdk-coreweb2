using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kinetix.Internal
{
    public class AccountPoller
    {
        // Constants
        private const    int                     INTERVAL_TIME_IN_SECONDS = 60;
        
        // Events
        private event Action OnEndPoll;
        
        // Cache
        private readonly string                  GameAPIKey;
        private          CancellationTokenSource cancellationTokenSource;
        
        public AccountPoller(string _GameAPIKey)
        {
            GameAPIKey =  _GameAPIKey;
            OnEndPoll  += HandleOnEndPoll;
        }

        public async void StartPolling()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource?.Cancel();
                await Task.Yield();
            }

            string url = KinetixConstants.c_SDK_API_URL + "/v1/users/" +
                         KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AccountId +
                         "/emotes";

            GetNewEmoteByPollingConfig getNewEmoteConfig = new GetNewEmoteByPollingConfig(url, GameAPIKey, INTERVAL_TIME_IN_SECONDS);
            GetNewEmoteByPolling       operation         = new GetNewEmoteByPolling(getNewEmoteConfig);
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                GetNewEmoteByPollingResponse getNewEmoteResponse = await OperationManagerShortcut.Get().RequestExecution(operation, cancellationTokenSource);
                KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().LoggedAccount.AddEmotesFromMetadata(getNewEmoteResponse.newAnimationsMetadata);
                KinetixCoreBehaviour.ManagerLocator.Get<AccountManager>().OnUpdatedAccount?.Invoke();
                OnEndPoll?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // Nothing on cancel
            }
            catch (Exception)
            {
                OnEndPoll?.Invoke();
            }
        }

        private void HandleOnEndPoll()
        {
            StartPolling();
        }

        public void StopPolling()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource?.Cancel();
            }
        }
    }
}
