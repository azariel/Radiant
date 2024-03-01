using Radiant.Common.HttpClients.RestClient;
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products;
using System.Threading.Tasks;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.WebApiClient
{
    internal static class RadiantProductsHistoryWebApiClient
    {
        private static HttpRestClient httpRestClient = new();

        /// <summary>
        /// Get next product ready for a refresh
        /// </summary>
        internal static async Task<ProductWithDefinitionsResponseDto> GetNextPendingProductAsync()
        {
            var _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string url = $"{_Config.ProductsWebApiBaseUrl}/products/nextPending";

            var _RawJsonObject = await httpRestClient.GetAsync(url, 10000, true);

            // deserialize object
            return JsonCommonSerializer.DeserializeFromString<ProductWithDefinitionsResponseDto>(_RawJsonObject);
        }

        public static async Task<bool> UpdateProductDefinitionAsync(ProductDefinitionsPatchRequestDto productDefinitionRequest)
        {
            var _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string url = $"{_Config.ProductsWebApiBaseUrl}/productDefinitions";

            var _RawJsonObject = await httpRestClient.PatchAsync(url, JsonCommonSerializer.SerializeToString(productDefinitionRequest), 10000, true);

            // TODO: validate object

            return _RawJsonObject != null;
        }

        public static async Task<bool> AddProductHistory(ProductHistoryPostRequestDto productHistoryPostRequestDto)
        {
            var _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string url = $"{_Config.ProductsWebApiBaseUrl}/productsHistory";

            var _RawJsonObject = await httpRestClient.PostAsync(url, JsonCommonSerializer.SerializeToString(productHistoryPostRequestDto), 10000, true);

            // TODO: validate object

            return _RawJsonObject != null;
        }
    }
}
