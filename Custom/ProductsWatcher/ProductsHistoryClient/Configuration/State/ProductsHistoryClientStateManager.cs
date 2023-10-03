using System.IO;
using Radiant.Common.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient.Configuration.State
{
    public static class ProductsHistoryClientStateManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string PRODUCT_HISTORY_STATE_FILE_NAME = "ProductsHistoryClientState.json";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static ProductsHistoryClientState fProductsHistoryClientState;

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static ProductsHistoryClientState GetConfigFromMemory()
        {
            return fProductsHistoryClientState ?? ReloadConfig();
        }

        public static ProductsHistoryClientState ReloadConfig()
        {
            // Create default config if doesn't exists
            if (!File.Exists(PRODUCT_HISTORY_STATE_FILE_NAME))
                SaveConfigInMemoryToDisk();

            string _ConfigFileContent = File.ReadAllText(PRODUCT_HISTORY_STATE_FILE_NAME);
            fProductsHistoryClientState = JsonCommonSerializer.DeserializeFromString<ProductsHistoryClientState>(_ConfigFileContent);
            return fProductsHistoryClientState;
        }

        public static void SaveConfigInMemoryToDisk()
        {
            if (fProductsHistoryClientState == null)
                fProductsHistoryClientState = new ProductsHistoryClientState();

            string _SerializedConfig = JsonCommonSerializer.SerializeToString(fProductsHistoryClientState);
            File.WriteAllText(PRODUCT_HISTORY_STATE_FILE_NAME, _SerializedConfig);
        }

        public static void SetConfigInMemory(ProductsHistoryClientState aProductsHistoryClientState)
        {
            fProductsHistoryClientState = aProductsHistoryClientState;
        }
    }
}

