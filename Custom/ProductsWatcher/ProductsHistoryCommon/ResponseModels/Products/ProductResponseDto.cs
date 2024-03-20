using System.Text.Json.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductResponseDto
    {
        public ProductResponseDto(RadiantProductModel productFromDal)
        {
            if (productFromDal == null)
                return;

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
