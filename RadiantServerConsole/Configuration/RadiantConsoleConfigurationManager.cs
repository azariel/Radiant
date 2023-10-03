using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Servers.RadiantServerConsole.Configuration
{
    public static class RadiantConsoleConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string PRODUCT_HISTORY_CONFIG_FILE_NAME = "RadiantServerConsoleConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static RadiantServerConsoleConfiguration fRadiantServerConsoleConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantServerConsoleConfiguration GetConfigFromMemory()
        {
            return fRadiantServerConsoleConfiguration ?? ReloadConfig();
        }

        public static RadiantServerConsoleConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(PRODUCT_HISTORY_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME);
            fRadiantServerConsoleConfiguration = JsonCommonSerializer.DeserializeFromString<RadiantServerConsoleConfiguration>(_ConfigFileContent);
            return fRadiantServerConsoleConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantServerConsoleConfiguration == null)
                fRadiantServerConsoleConfiguration = new RadiantServerConsoleConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantServerConsoleConfiguration);
            File.WriteAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(RadiantServerConsoleConfiguration aRadiantServerConsoleConfiguration)
        {
            fRadiantServerConsoleConfiguration = aRadiantServerConsoleConfiguration;
        }
    }
}
