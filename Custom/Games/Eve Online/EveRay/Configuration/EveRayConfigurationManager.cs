using System.IO;
using Radiant.Common.Serialization;

namespace EveRay.Configuration
{
    public static class EveRayConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string EVE_RAY_CONFIG_FILE_NAME = "EveRayConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static EveRayConfiguration fEveRayConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static EveRayConfiguration GetConfigFromMemory()
        {
            return fEveRayConfiguration ?? ReloadConfig();
        }

        public static EveRayConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(EVE_RAY_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(EVE_RAY_CONFIG_FILE_NAME);
            fEveRayConfiguration = JsonCommonSerializer.DeserializeFromString<EveRayConfiguration>(_ConfigFileContent);
            return fEveRayConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fEveRayConfiguration == null)
                fEveRayConfiguration = new EveRayConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fEveRayConfiguration);
            File.WriteAllText(EVE_RAY_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(EveRayConfiguration aProductsHistoryConfiguration)
        {
            fEveRayConfiguration = aProductsHistoryConfiguration;
        }
    }
}
