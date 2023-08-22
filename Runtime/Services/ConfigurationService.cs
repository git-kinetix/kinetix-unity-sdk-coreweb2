// // ----------------------------------------------------------------------------
// // <copyright file="ProviderService.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;

namespace Kinetix.Internal
{
    internal class ConfigurationService: IKinetixService
    {
        private Dictionary<Type, IKinetixConfiguration> configurations;

        public ConfigurationService(KinetixCoreConfiguration _CoreConfiguration)
        {
            configurations = new Dictionary<Type, IKinetixConfiguration>();

            configurations.Add(typeof(KinetixCoreConfiguration), _CoreConfiguration);
        }

        public bool Register<TConfiguration>(TConfiguration _Config) where TConfiguration: IKinetixConfiguration
        {
            if (configurations.ContainsKey(typeof(TConfiguration))) return false;

            configurations.Add(typeof(TConfiguration), _Config);

            return true;
        }

        public TConfiguration Get<TConfiguration>() where TConfiguration: IKinetixConfiguration
        {
            configurations.TryGetValue(typeof(TConfiguration), out IKinetixConfiguration returnValue);
            
            return (TConfiguration) returnValue;
        }
    }
}
