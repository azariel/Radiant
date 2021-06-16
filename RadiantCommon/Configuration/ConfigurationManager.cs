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
        private static RadiantConfig fRadiantConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantConfig GetConfigFromMemory()
        {
            return fRadiantConfig ?? ReloadConfig();
        }

        public static RadiantConfig ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(RADIANT_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(RADIANT_CONFIG_FILE_NAME);
            fRadiantConfig = JsonCommonSerializer.DeserializeFromString<RadiantConfig>(_ConfigFileContent);
            return fRadiantConfig;
        }

        public static void SetConfigInMemory(RadiantConfig aRadiantConfig)
        {
            fRadiantConfig = aRadiantConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantConfig == null)
                fRadiantConfig = new RadiantConfig();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantConfig);
            File.WriteAllText(RADIANT_CONFIG_FILE_NAME, _SerializedConfig);
        }
    }
}
