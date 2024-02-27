using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.Products
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductResponseDto
    {
        public ProductResponseDto(RadiantProductModel productFromDal)
        {
            FetchProductHistoryEnabled = productFromDal.FetchProductHistoryEnabled;
            Name = productFromDal.Name;
            ProductId = productFromDal.ProductId;
        }

        [JsonPropertyName("fetchProductHistoryEnabled")]
        public bool FetchProductHistoryEnabled { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }
    }
}
