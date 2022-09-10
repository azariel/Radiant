using Radiant.Common.Serialization;

namespace EveChatMonitorer.Configuration
{
    public static class EveChatMonitorerConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string EVE_CHAT_MONITORER_CONFIG_FILE_NAME = "EveChatMonitorerConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static EveChatMonitorerConfiguration fEveChatMonitorerConfig;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static EveChatMonitorerConfiguration GetConfigFromMemory()
        {
            return fEveChatMonitorerConfig ?? ReloadConfig();
        }

        public static EveChatMonitorerConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(EVE_CHAT_MONITORER_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(EVE_CHAT_MONITORER_CONFIG_FILE_NAME);
            fEveChatMonitorerConfig = JsonCommonSerializer.DeserializeFromString<EveChatMonitorerConfiguration>(_ConfigFileContent);
            return fEveChatMonitorerConfig;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fEveChatMonitorerConfig == null)
                fEveChatMonitorerConfig = new EveChatMonitorerConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fEveChatMonitorerConfig);
            File.WriteAllText(EVE_CHAT_MONITORER_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(EveChatMonitorerConfiguration aProductsHistoryConfiguration)
        {
            fEveChatMonitorerConfig = aProductsHistoryConfiguration;
        }
    }
}
