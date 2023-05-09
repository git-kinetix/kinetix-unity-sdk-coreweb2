<<<<<<< HEAD
<<<<<<< Updated upstream
// // ----------------------------------------------------------------------------
// // <copyright file="BlockchainConstants.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------


using System.Collections.Generic;

namespace Kinetix.Internal
{
    public static class KinetixConstants
    {
        public const string version = "0.5.1";
        
        #region StreamingAssets
        
        public const string C_FreeAnimationsAssetPluginPath = "Packages/com.kinetix.coreweb2/Resources/FreeAnimations";
        
        public static string C_FreeAnimationsManifestPath => "Kinetix/FreeEmoteManifest.json";
        public const string C_FreeAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/FreeAnimations";
        public const string C_FreeAnimationsPath = "Kinetix/FreeAnimations";
        public const string C_FreeCustomAnimationsPath = "Kinetix/CustomFreeAnimations";
        public const string C_FreeCustomAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/CustomFreeAnimations";
        
        #endregion

    }
}
=======
=======
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702
// // ----------------------------------------------------------------------------
// // <copyright file="BlockchainConstants.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Kinetix.Internal
{
    public static class KinetixConstants
    {
<<<<<<< HEAD
        public const string version = "0.6.1";
=======
        public const string version = "0.6.0";
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702

        public static bool C_ShouldUGCBeAvailable = true;
        
#if STAGING_KINETIX
        public static readonly string c_SDK_API_URL = "https://sdk-api.staging.kinetix.tech";
#else
        public static readonly string c_SDK_API_URL = "https://sdk-api.kinetix.tech";
#endif


        public static readonly int c_TimeOutCreateQRCode = 305;
        
        
        #region StreamingAssets
        
<<<<<<< HEAD
#if DEV_KINETIX
        public const string C_FreeAnimationsAssetPluginPath = "Assets/Plugins/Kinetix/Core/Resources/FreeAnimations"; 
#elif WEB2
        public const string C_FreeAnimationsAssetPluginPath = "Packages/com.kinetix.coreweb2/Resources/FreeAnimations";
#endif
=======
        public const string C_FreeAnimationsAssetPluginPath = "Packages/com.kinetix.coreweb2/Resources/FreeAnimations";
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702
        
        public static string C_FreeAnimationsManifestPath => "Kinetix/FreeEmoteManifest.json";
        public const string C_FreeAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/FreeAnimations";
        public const string C_FreeAnimationsPath = "Kinetix/FreeAnimations";
        public const string C_FreeCustomAnimationsPath = "Kinetix/CustomFreeAnimations";
        public const string C_FreeCustomAnimationsAssetSAPath = "Assets/StreamingAssets/Kinetix/CustomFreeAnimations";
        
        #endregion

    }
}
<<<<<<< HEAD
>>>>>>> Stashed changes
=======
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702
