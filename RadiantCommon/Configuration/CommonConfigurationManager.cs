using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Common.Configuration
{
    public static class CommonConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string RADIANT_CONFIG_FILE_NAME = "RadiantConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static RadiantCommonConfig fRadiantConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantCommonConfig GetConfigFromMemory()
        {
            return fRadiantConfig ?? ReloadConfig();
        }

        public static RadiantCommonConfig ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(RADIANT_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(RADIANT_CONFIG_FILE_NAME);
            fRadiantConfig = JsonCommonSerializer.DeserializeFromString<RadiantCommonConfig>(_ConfigFileContent);
            return fRadiantConfig;
        }

        public static void SetConfigInMemory(RadiantCommonConfig aRadiantConfig)
        {
            fRadiantConfig = aRadiantConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantConfig == null)
                fRadiantConfig = new RadiantCommonConfig();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantConfig);
            File.WriteAllText(RADIANT_CONFIG_FILE_NAME, _SerializedConfig);
        }
    }
}
