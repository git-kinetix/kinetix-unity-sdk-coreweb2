// // ----------------------------------------------------------------------------
// // <copyright file="BlockchainConstants.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix.Internal
{
    public static class KinetixConstants
    {
        public const  string version                = "1.8.0";
        public static bool   C_ShouldUGCBeAvailable = true;
        
#if STAGING_KINETIX
        public static readonly string c_SDK_API_URL = "https://sdk-api.staging.kinetix.tech";
#elif DEV_KINETIX
        public static readonly string c_SDK_API_URL = "https://sdk-api.dev.kinetix.tech";
#else
        public static readonly string c_SDK_API_URL = "https://sdk-api.kinetix.tech";
#endif

        public static readonly int c_TimeOutCreateQRCode = 305;
        
        
        #region StreamingAssets
        
        public static string C_FreeAnimationsManifestPath => "Kinetix/FreeEmoteManifest.json";

        public static readonly string SDK_USER_AGENT = "SDK-Unity/" + version;

        public const string C_FreeAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/FreeAnimations";
        public const string C_FreeAnimationsPath = "Kinetix/FreeAnimations";
        public const string C_FreeCustomAnimationsPath = "Kinetix/CustomFreeAnimations";
        public const string C_FreeCustomAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/CustomFreeAnimations";
        
        #endregion
    }
}
