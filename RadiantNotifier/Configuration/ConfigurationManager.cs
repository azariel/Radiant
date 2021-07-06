using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Notifier.Configuration
{
    public static class NotificationConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string RADIANT_CONFIG_FILE_NAME = "RadiantNotificationConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static RadiantNotificationConfig fRadiantConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantNotificationConfig GetConfigFromMemory()
        {
            return fRadiantConfig ?? ReloadConfig();
        }

        public static RadiantNotificationConfig ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(RADIANT_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(RADIANT_CONFIG_FILE_NAME);
            fRadiantConfig = JsonCommonSerializer.DeserializeFromString<RadiantNotificationConfig>(_ConfigFileContent);
            return fRadiantConfig;
        }

        public static void SetConfigInMemory(RadiantNotificationConfig aRadiantConfig)
        {
            fRadiantConfig = aRadiantConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fRadiantConfig == null)
                fRadiantConfig = new RadiantNotificationConfig();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fRadiantConfig);
            File.WriteAllText(RADIANT_CONFIG_FILE_NAME, _SerializedConfig);
        }
    }
}
