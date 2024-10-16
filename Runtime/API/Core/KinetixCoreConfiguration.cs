// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCoreConfiguration.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using Kinetix.Internal;

namespace Kinetix
{
    public class KinetixCoreConfiguration: IKinetixConfiguration
    {
        public string GameAPIKey;
        public string APIBaseURL = "https://sdk-api.kinetix.tech";
        public ContextualEmoteSO EmoteContexts;
        public EKinetixNodeProvider EmoteProvider = EKinetixNodeProvider.SDK_API;

        // Play Animation Automatically on Animators
        public bool   PlayAutomaticallyAnimationOnAnimators = true;
        
        // Enable Analytics
        public bool   EnableAnalytics = true;
        
        // Show Logs
        public bool   ShowLogs = false;

        // Are the User Generated Content available?
        public bool EnableUGC = true;

        // Network Configuration        
        public KinetixNetworkConfiguration NetworkConfiguration;

        // How many emotes have to be cached?
        public int CachedEmotesNb = 10;

    }
}
