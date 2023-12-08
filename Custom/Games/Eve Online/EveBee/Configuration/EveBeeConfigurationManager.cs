using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radiant.Common.Serialization;

namespace EveBee.Configuration
{
    public static class EveBeeConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string EVE_FIGHT_CONFIG_FILE_NAME = "EveBeeConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static EveBeeConfiguration fEveBeeConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static EveBeeConfiguration GetConfigFromMemory()
        {
            return fEveBeeConfig ?? ReloadConfig();
        }

        public static EveBeeConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(EVE_FIGHT_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(EVE_FIGHT_CONFIG_FILE_NAME);
            fEveBeeConfig = JsonCommonSerializer.DeserializeFromString<EveBeeConfiguration>(_ConfigFileContent);
            return fEveBeeConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fEveBeeConfig == null)
                fEveBeeConfig = new EveBeeConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fEveBeeConfig);
            File.WriteAllText(EVE_FIGHT_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(EveBeeConfiguration aProductsHistoryConfiguration)
        {
            fEveBeeConfig = aProductsHistoryConfiguration;
        }
    }
}
