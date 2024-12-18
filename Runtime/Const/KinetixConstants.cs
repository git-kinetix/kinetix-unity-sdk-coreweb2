// // ----------------------------------------------------------------------------
// // <copyright file="BlockchainConstants.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

namespace Kinetix.Internal
{
    public static class KinetixConstants
    {
        public const  string version                = "1.10.2";
        public static bool   C_ShouldUGCBeAvailable = true;
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
