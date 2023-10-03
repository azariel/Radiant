using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.Games.EveOnline.EveFight.Configuration
{
    public static class EveFightConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string EVE_FIGHT_CONFIG_FILE_NAME = "EveFightConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static EveFightConfiguration fEveFightConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static EveFightConfiguration GetConfigFromMemory()
        {
            return fEveFightConfig ?? ReloadConfig();
        }

        public static EveFightConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(EVE_FIGHT_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(EVE_FIGHT_CONFIG_FILE_NAME);
            fEveFightConfig = JsonCommonSerializer.DeserializeFromString<EveFightConfiguration>(_ConfigFileContent);
            return fEveFightConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fEveFightConfig == null)
                fEveFightConfig = new EveFightConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fEveFightConfig);
            File.WriteAllText(EVE_FIGHT_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(EveFightConfiguration aProductsHistoryConfiguration)
        {
            fEveFightConfig = aProductsHistoryConfiguration;
        }
    }
}
