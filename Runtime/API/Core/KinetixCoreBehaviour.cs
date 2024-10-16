// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCoreBehaviour.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using Kinetix.Internal.Cache;
using UnityEngine;

namespace Kinetix.Internal
{
	internal static class KinetixCoreBehaviour
	{
		private static bool initialized;
		public static  bool Initialized => initialized;

		public static ServiceLocator ServiceLocator => serviceLocator;
		private static ServiceLocator serviceLocator;

		public static ManagerLocator ManagerLocator => managerLocator;
		private static ManagerLocator managerLocator;

		public static void Initialize(KinetixCoreConfiguration _Configuration, Action _OnInitialized)
		{    
			InitializeServices(_Configuration);

			InitializeManagers(_Configuration);

			KinetixCore.Account = new KinetixAccount();
			KinetixCore.Metadata = new KinetixMetadata();
			KinetixCore.Animation = new KinetixAnimation();
			KinetixCore.Network = new KinetixNetwork();
			KinetixCore.UGC = new KinetixUGC();
			KinetixCore.Process = new KinetixProcess();
			
			KinetixAnalytics.Initialize(_Configuration.EnableAnalytics);

			initialized = true;
			_OnInitialized?.Invoke();
		}

		private static void InitializeServices(KinetixCoreConfiguration _Configuration)
		{
			if (serviceLocator != null)
			{
				serviceLocator.Dispose();
				RetargetTaskManager.ClearTasks();
			}

			serviceLocator = new ServiceLocator();

			serviceLocator.Register<EmoteDownloadSpeedService>(new EmoteDownloadSpeedService());
			serviceLocator.Register<AnimationProgressService>(new AnimationProgressService(serviceLocator));
			serviceLocator.Register<AssetService>(new AssetService());
			serviceLocator.Register<LockService>(new LockService());
			serviceLocator.Register<MemoryService>(new MemoryService(_Configuration));
			serviceLocator.Register<ProviderService>(new ProviderService(_Configuration));
			serviceLocator.Register<EmotesService>(new EmotesService(serviceLocator, _Configuration));
			serviceLocator.Register<LoadAnimService>(new LoadAnimService(serviceLocator));
			serviceLocator.Register<RetargetingService>(new RetargetingService(serviceLocator));

			serviceLocator.Get<LockService>().OnRequestEmoteUnload += serviceLocator.Get<RetargetingService>().ClearAvatar;
		}

		private static void InitializeManagers(KinetixCoreConfiguration _Configuration)
		{
			KinetixDebug.c_ShowLog = _Configuration.ShowLogs;
			
			managerLocator = new ManagerLocator();
			
			managerLocator.Register<PlayersManager>(new PlayersManager(serviceLocator, _Configuration));
			managerLocator.Register<AccountManager>(new AccountManager(serviceLocator, _Configuration));
			managerLocator.Register<UGCManager>(new UGCManager(serviceLocator, _Configuration));
			managerLocator.Register<ContextManager>(new ContextManager(serviceLocator, _Configuration));
			managerLocator.Register<NetworkManager>(new NetworkManager(serviceLocator, _Configuration));
			
			managerLocator.Get<AccountManager>().OnDisconnectedAccount += () => managerLocator.Get<UGCManager>().ClearPolling(true);
		}

		public static bool IsInitialized()
		{
			return Initialized;
		}
	}
}
