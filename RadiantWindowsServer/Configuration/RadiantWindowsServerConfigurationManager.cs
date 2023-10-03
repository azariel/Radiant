using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Servers.RadiantWindowsServer.Configuration
{
    public static class RadiantWindowsServerConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string PRODUCT_HISTORY_CONFIG_FILE_NAME = "RadiantWindowsServerConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static RadiantWindowsServerConfiguration fRadiantWindowsServerConsoleConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantWindowsServerConfiguration GetConfigFromMemory()
        {
            return fRadiantWindowsServerConsoleConfiguration ?? ReloadConfig();
        }

        public static RadiantWindowsServerConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(PRODUCT_HISTORY_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME);
            fRadiantWindowsServerConsoleConfiguration = JsonCommonSerializer.DeserializeFromString<RadiantWindowsServerConfiguration>(_ConfigFileContent);
            return fRadiantWindowsServerConsoleConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantWindowsServerConsoleConfiguration == null)
                fRadiantWindowsServerConsoleConfiguration = new RadiantWindowsServerConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantWindowsServerConsoleConfiguration);
            File.WriteAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(RadiantWindowsServerConfiguration aRadiantWindowsServerConsoleConfiguration)
        {
            fRadiantWindowsServerConsoleConfiguration = aRadiantWindowsServerConsoleConfiguration;
        }
    }
}
