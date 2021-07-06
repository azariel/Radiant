using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.ProductsHistory.Configuration
{
    public static class ProductsHistoryConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string PRODUCT_HISTORY_CONFIG_FILE_NAME = "RadiantProductsHistoryConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static ProductsHistoryConfiguration fProductsHistoryConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static ProductsHistoryConfiguration GetConfigFromMemory()
        {
            return fProductsHistoryConfiguration ?? ReloadConfig();
        }

        public static ProductsHistoryConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(PRODUCT_HISTORY_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME);
            fProductsHistoryConfiguration = JsonCommonSerializer.DeserializeFromString<ProductsHistoryConfiguration>(_ConfigFileContent);
            return fProductsHistoryConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fProductsHistoryConfiguration == null)
                fProductsHistoryConfiguration = new ProductsHistoryConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fProductsHistoryConfiguration);
            File.WriteAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(ProductsHistoryConfiguration aProductsHistoryConfiguration)
        {
            fProductsHistoryConfiguration = aProductsHistoryConfiguration;
        }
    }
}
