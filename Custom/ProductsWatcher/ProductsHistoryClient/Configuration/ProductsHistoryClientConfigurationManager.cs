using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient.Configuration
{
    public static class ProductsHistoryClientConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string PRODUCT_HISTORY_CONFIG_FILE_NAME = "ProductsHistoryClientConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static ProductsHistoryClientConfiguration fProductsHistoryClientConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static ProductsHistoryClientConfiguration GetConfigFromMemory()
        {
            return fProductsHistoryClientConfiguration ?? ReloadConfig();
        }

        public static ProductsHistoryClientConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(PRODUCT_HISTORY_CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME);
            fProductsHistoryClientConfiguration = JsonCommonSerializer.DeserializeFromString<ProductsHistoryClientConfiguration>(_ConfigFileContent);
            return fProductsHistoryClientConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fProductsHistoryClientConfiguration == null)
                fProductsHistoryClientConfiguration = new ProductsHistoryClientConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fProductsHistoryClientConfiguration);
            File.WriteAllText(PRODUCT_HISTORY_CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(ProductsHistoryClientConfiguration aProductsHistoryClientConfiguration)
        {
            fProductsHistoryClientConfiguration = aProductsHistoryClientConfiguration;
        }
    }
}

