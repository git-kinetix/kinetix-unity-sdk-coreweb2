// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCoreBehaviour.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using Kinetix.Internal.Cache;

namespace Kinetix.Internal
{
    internal static class KinetixCoreBehaviour
    {
        private static bool initialized;
        public static  bool Initialized => initialized;

        public static void Initialize(KinetixCoreConfiguration _Configuration, Action _OnInitialized)
        {
            KinetixCore.Account   = new KinetixAccount();
            KinetixCore.Metadata  = new KinetixMetadata();
            KinetixCore.Animation = new KinetixAnimation();
            KinetixCore.Network   = new KinetixNetwork();
            KinetixCore.UGC   = new KinetixUGC();
            KinetixCore.Context   = new KinetixContext();

            InitializeManagers(_Configuration);

            initialized = true;
            _OnInitialized?.Invoke();
        }

        private static void InitializeManagers(KinetixCoreConfiguration _Configuration)
        {
            KinetixDebug.c_ShowLog = _Configuration.ShowLogs;
            
            MemoryManager.Initialize();
            
            AssetManager.Initialize();
            ProviderManager.Initialize(EKinetixNodeProvider.SDK_API, _Configuration.VirtualWorldKey);
            EmotesManager.Initialize();
            LocalPlayerManager.Initialize(_Configuration.PlayAutomaticallyAnimationOnAnimators);
            AccountManager.Initialize(_Configuration.VirtualWorldKey);
            UGCManager.Initialize(_Configuration.VirtualWorldKey, _Configuration.EnableUGC);
            ContextManager.Initialize(_Configuration.EmoteContexts);
            NetworkManager.Initialize(_Configuration.NetworkConfiguration);
            KinetixAnalytics.Initialize(_Configuration.EnableAnalytics);
        }

        public static bool IsInitialized()
        {
            return Initialized;
        }
    }
}
