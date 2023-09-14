using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kinetix.Internal
{
    internal class AliasManager : AKinetixManager
    {
        public const string ALIAS_ROUTE = "/v1/virtual-world/alias/";
        private      string GameAPIKey;

        public AliasManager(ServiceLocator _ServiceLocator, KinetixCoreConfiguration _Config) : base(_ServiceLocator, _Config)
        {
        }

        protected override void Initialize(KinetixCoreConfiguration _Config)
        {
            Initialize(_Config.GameAPIKey);
        }

        private void Initialize(string _GameAPIKey)
        {
            GameAPIKey = _GameAPIKey;
        }

        public async Task<string> GetEmoteIDByAlias(string _Alias)
        {
            if (string.IsNullOrEmpty(GameAPIKey))
                throw new Exception("No GameAPIKey found, please check the KinetixCoreConfiguration.");

            string                  url       = KinetixConstants.c_SDK_API_URL + ALIAS_ROUTE + _Alias;
            GetEmoteIDByAliasConfig config    = new GetEmoteIDByAliasConfig(GameAPIKey, url);
            GetEmoteIDByAlias       operation = new GetEmoteIDByAlias(config);

            try
            {
                GetEmoteIDByAliasResponse response = await OperationManagerShortcut.Get().RequestExecution(operation);
                return response.EmoteID;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
