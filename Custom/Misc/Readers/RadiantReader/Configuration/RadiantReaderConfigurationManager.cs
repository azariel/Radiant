using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.Readers.RadiantReader.Configuration
{
    public static class RadiantReaderConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string RADIANT_READER_CONFIG_FILE_NAME = "RadiantReaderConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static RadiantReaderConfiguration fRadiantReaderConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantReaderConfiguration GetConfigFromMemory()
        {
            return fRadiantReaderConfiguration ?? ReloadConfig();
        }

        public static RadiantReaderConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(RADIANT_READER_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(RADIANT_READER_CONFIG_FILE_NAME);
            fRadiantReaderConfiguration = JsonCommonSerializer.DeserializeFromString<RadiantReaderConfiguration>(_ConfigFileContent);
            return fRadiantReaderConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantReaderConfiguration == null)
                fRadiantReaderConfiguration = new RadiantReaderConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantReaderConfiguration);
            File.WriteAllText(RADIANT_READER_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(RadiantReaderConfiguration aProductsHistoryConfiguration)
        {
            fRadiantReaderConfiguration = aProductsHistoryConfiguration;
        }
    }
}
