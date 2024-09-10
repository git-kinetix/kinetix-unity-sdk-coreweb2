using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kinetix.Internal.Cache;
using Kinetix.Internal.Utils;
using Kinetix.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Kinetix.Internal
{
    public class AccountManager: AKinetixManager
    {
        public Action OnUpdatedAccount;
        public Action OnConnectedAccount;
        public Action OnDisconnectedAccount;

        private List<Account> Accounts;
        private string        GameAPIKey;
        
        public UserAccount LoggedAccount => loggedAccount;

        private UserAccount   loggedAccount;
        private AccountPoller accountPoller;

        public AccountManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config) {}


        protected override void Initialize(KinetixCoreConfiguration _Config)
        {
            Initialize(_Config.GameAPIKey);
        }

        protected void Initialize()
        {
            Initialize(string.Empty);
        }

        protected void Initialize(string _GameAPIKey)
        {
            GameAPIKey = _GameAPIKey;
            
            Accounts      = new List<Account>();
            accountPoller = new AccountPoller(GameAPIKey);
        }
        
        public async Task<bool> ConnectAccount(string _UserId)
        {
            if (string.IsNullOrEmpty(GameAPIKey))
            {
                KinetixDebug.LogWarning("No GameAPIKey found, please check the KinetixCoreConfiguration.");

                return false;
            }

            if (IsAccountAlreadyConnected(_UserId))
            {
                KinetixDebug.LogWarning("Account is already connected");
            }

            try
            {
                if (!await AccountExists(_UserId))
                {
                    if (!await TryCreateAccount(_UserId))
                        return false;
                }
            }
            catch (Exception e)
            {
                KinetixDebug.LogWarning("Issue while connecting account : " + e.Message);
                return false;
            }

            if (loggedAccount != null)
            {
                Debug.LogWarning("An account was already connected, disconnecting the previous!");
                DisconnectAccount();
            }

            loggedAccount = new UserAccount(_UserId);

            await loggedAccount.FetchMetadatas();

            if (loggedAccount == null)
                return false;

            Accounts.Add(loggedAccount);

            
            accountPoller.StartPolling();
            
            OnUpdatedAccount?.Invoke();
            OnConnectedAccount?.Invoke();

            return true;
        }

        public void DisconnectAccount()
        {
            if (loggedAccount == null)
                return;

            int foundIndex = -1;

            for (int i = 0; i < Accounts.Count; i++)
            {
                if (Accounts[i] is UserAccount)
                {
                    foundIndex = i;
                }
            }

            RemoveEmotesAndAccount(foundIndex);
            loggedAccount = null;
            
            accountPoller.StopPolling();
            OnDisconnectedAccount?.Invoke();
        }

        private async Task<bool> TryCreateAccount(string _UserId)
        {
            if (String.IsNullOrEmpty(GameAPIKey))
            {
                KinetixDebug.LogWarning("No GameAPIKey found, please check the KinetixCoreConfiguration.");
                return false;
            }

            // Try to create account
            string url = KinetixConstants.c_SDK_API_URL + "/v1/virtual-world/users";
            string payload = "{\"id\":\"" + _UserId + "\"}";
            
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Content-type", "application/json" },
                { "Accept", "application/json" },
                { "x-api-key", GameAPIKey }
            };
            
            try
            {
                WebRequestDispatcher webRequest = new WebRequestDispatcher();

                RawResponse response = await webRequest.SendRequest<RawResponse>(url, WebRequestDispatcher.HttpMethod.POST, headers, payload);
                if (!response.IsSuccess)
                    throw new Exception(response.Error);
                return response.IsSuccess;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<bool> AccountExists(string _UserId)
        {
            if (string.IsNullOrEmpty(GameAPIKey))
            {
                KinetixDebug.LogWarning("No GameAPIKey found, please check the KinetixCoreConfiguration.");
                return false;
            }

            string uri = KinetixConstants.c_SDK_API_URL + "/v1/virtual-world/users/" + _UserId;

            GetRawAPIResultConfig   apiResultOpConfig = new GetRawAPIResultConfig(uri, GameAPIKey);
            GetRawAPIResult         apiResultOp = new GetRawAPIResult(apiResultOpConfig);
            GetRawAPIResultResponse response = await OperationManagerShortcut.Get().RequestExecution(apiResultOp);

            string result = response.json;
            
            bool accountExist = !string.IsNullOrEmpty(result);

            return accountExist;
        }


        public bool IsAccountAlreadyConnected(string _AccountId)
        {
            foreach (Account acc in Accounts)
            {
                if (acc.AccountId == _AccountId)
                {
                    return true;
                }
            }

            return false;
        }

        public async void GetAllUserProcesses(Action<SdkApiProcess[]> _OnSuccess, Action _OnFailure = null)
        {
            try
            {
                SdkApiProcess[] processes = await KinetixCoreBehaviour.ServiceLocator.Get<ProviderService>().GetUserProcesses(loggedAccount.AccountId);
                
                _OnSuccess?.Invoke(processes);
            }
            catch (Exception e)
            {
                _OnFailure?.Invoke();
                KinetixDebug.LogWarning(e.Message);
            }
        }

        public async void ValidateEmoteProcess(string _ProcessId, Action<SdkApiProcess> _OnSuccess, Action _OnFailure = null)
        {
            try
            {
                SdkApiProcess process = await KinetixCoreBehaviour.ServiceLocator.Get<ProviderService>().ValidateEmote(_ProcessId);

                _OnSuccess?.Invoke(process);
            }
            catch (Exception e)
            {
                _OnFailure?.Invoke();
                KinetixDebug.LogWarning(e.Message);
            }
        }

        public async void RetakeEmoteProcess(string _ProcessId, Action<string> _OnSuccess, Action _OnFailure = null)
        {
            try
            {
                SdkTokenValidityResult process = await KinetixCoreBehaviour.ServiceLocator.Get<ProviderService>().RetakeEmote(_ProcessId);

                _OnSuccess?.Invoke(process.url.ToString());
            }
            catch (Exception e)
            {
                _OnFailure?.Invoke();
                KinetixDebug.LogWarning(e.Message);
            }
        }

        public async void GetAllUserEmotes(Action<AnimationMetadata[]> _OnSuccess, Action _OnFailure = null)
        {
            List<KinetixEmote> emotesAccountAggregation = new List<KinetixEmote>();
            int                countAccount             = Accounts.Count;


            if (Accounts.Count == 0)
            {
                _OnSuccess?.Invoke(emotesAccountAggregation.Select(emote => emote.Metadata).ToArray());
                return;
            }

            try
            {
                for (int i = 0; i < Accounts.Count; i++)
                {
                    KinetixEmote[] accountEmotes = await Accounts[i].FetchMetadatas();

                    List<KinetixEmote> accountEmotesList = accountEmotes.ToList();

                    // Remove all animations with are duplicated and not owned
                    emotesAccountAggregation.RemoveAll(metadata => accountEmotesList.Exists(emote =>
                        emote.Ids.UUID == metadata.Ids.UUID && emote.Metadata.Ownership != EOwnership.OWNER));

                    emotesAccountAggregation.AggregateAndDistinct(accountEmotes);
                    countAccount--;

                    if (countAccount == 0)
                    {
                        emotesAccountAggregation =
                            emotesAccountAggregation.OrderBy(emote => emote.Metadata.Ownership).ToList();
                        KinetixEmote[] metadatasAccountsAggregationArray = emotesAccountAggregation.ToArray();
                        _OnSuccess?.Invoke(metadatasAccountsAggregationArray.Select(emote => emote.Metadata).ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                _OnSuccess?.Invoke(emotesAccountAggregation.Select(emote => emote.Metadata).ToArray());
                KinetixDebug.LogWarning(e.Message);
            }
        }

        public void IsAnimationOwnedByUser(AnimationIds _AnimationIds, Action<bool> _OnSuccess,
            Action                                             _OnFailure = null)
        {
            GetAllUserEmotes(
                metadatas =>
                {
                    _OnSuccess.Invoke(metadatas.ToList().Exists(metadata => metadata.Ids.Equals(_AnimationIds)));
                }, _OnFailure);
        }

        public void GetUserAnimationsMetadatasByPage(int _Count,    int    _Page,
            Action<AnimationMetadata[]>                         _Callback, Action _OnFailure)
        {
            GetAllUserEmotes(animationMetadatas =>
            {
                if ((_Page + 1) * _Count <= animationMetadatas.Length)
                {
                    _Callback?.Invoke(animationMetadatas.ToList().GetRange((_Page * _Count), _Count).ToArray());
                }
                else
                {
                    int lastPageCount = animationMetadatas.Length % _Count;
                    _Callback?.Invoke(animationMetadatas.ToList()
                        .GetRange(animationMetadatas.Length - lastPageCount, lastPageCount).ToArray());
                }
            }, () => { _OnFailure?.Invoke(); });
        }

        public void GetUserAnimationsTotalPagesCount(int _CountByPage, Action<int> _Callback, Action _OnFailure)
        {
            GetAllUserEmotes(animationMetadatas =>
            {
                if (animationMetadatas.Length == 0)
                {
                    _Callback?.Invoke(1);
                    return;
                }

                int totalPage = Mathf.CeilToInt((float)animationMetadatas.Length / (float)_CountByPage);
                _Callback?.Invoke(totalPage);
            }, () => { _OnFailure?.Invoke(); });
        }


        private void RemoveEmotesAndAccount(int _AccountIndex)
        {
            if (_AccountIndex == -1)
                return;

            GetAllUserEmotes(beforeAnimationMetadatas =>
            {
                List<AnimationIds> idsBeforeRemoveWallet =
                    beforeAnimationMetadatas.ToList().Select(metadata => metadata.Ids).ToList();

                Accounts.RemoveAt(_AccountIndex);

                GetAllUserEmotes(afterAnimationMetadatas =>
                {
                    List<AnimationIds> idsAfterRemoveWallet =
                        afterAnimationMetadatas.ToList().Select(metadata => metadata.Ids).ToList();
                    idsBeforeRemoveWallet = idsBeforeRemoveWallet.Except(idsAfterRemoveWallet).ToList();

                    KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.ForceUnloadPlayerAnimations(idsBeforeRemoveWallet.ToArray());
                    KinetixCoreBehaviour.ManagerLocator.Get<PlayersManager>().LocalPlayer.RemovePlayerEmotesToPreload(idsBeforeRemoveWallet.ToArray());
                    
                    OnUpdatedAccount?.Invoke();
                });
            });
        }
    }
}
