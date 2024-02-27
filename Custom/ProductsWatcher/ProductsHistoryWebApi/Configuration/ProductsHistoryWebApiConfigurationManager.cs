using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Configuration
{
    public static class ProductsHistoryWebApiConfigurationManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string CONFIG_FILE_NAME = "RadiantProductsHistoryWebApiConfig.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static ProductsHistoryWebApiConfiguration fProductsHistoryWebApiConfiguration;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static ProductsHistoryWebApiConfiguration GetConfigFromMemory()
        {
            return fProductsHistoryWebApiConfiguration ?? ReloadConfig();
        }

        public static ProductsHistoryWebApiConfiguration ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(CONFIG_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(CONFIG_FILE_NAME);
            fProductsHistoryWebApiConfiguration = JsonCommonSerializer.DeserializeFromString<ProductsHistoryWebApiConfiguration>(_ConfigFileContent);
            return fProductsHistoryWebApiConfiguration;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fProductsHistoryWebApiConfiguration == null)
                fProductsHistoryWebApiConfiguration = new ProductsHistoryWebApiConfiguration();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fProductsHistoryWebApiConfiguration);
            File.WriteAllText(CONFIG_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(ProductsHistoryWebApiConfiguration aProductsHistoryConfiguration)
        {
            fProductsHistoryWebApiConfiguration = aProductsHistoryConfiguration;
        }
    }
}
