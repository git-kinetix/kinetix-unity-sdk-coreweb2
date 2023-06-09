// // ----------------------------------------------------------------------------
// // <copyright file="KinetixCoreConfiguration.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix
{
    public class KinetixCoreConfiguration
    {
        public string VirtualWorldKey;
        public ContextualEmoteSO EmoteContexts;

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
        
    }
}
