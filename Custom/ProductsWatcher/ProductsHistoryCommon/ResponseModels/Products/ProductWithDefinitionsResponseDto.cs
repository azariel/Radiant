using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductDefinitions;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductWithDefinitionsResponseDto
    {
        public ProductWithDefinitionsResponseDto(RadiantServerProductModel productFromDal)
        {
            if (productFromDal == null)
                return;

            FetchProductHistoryEnabled = productFromDal.FetchProductHistoryEnabled;
            Name = productFromDal.Name;
            ProductId = productFromDal.ProductId;
            InsertDateTime = productFromDal.InsertDateTime;
            Definitions = new ProductDefinitionsResponseDto
            {
                ProductDefinitions = productFromDal.ProductDefinitionCollection.Select(s=> new ProductDefinitionResponseDto(s)).ToList()
            };
        }

        [JsonPropertyName("fetchProductHistoryEnabled")]
        public bool FetchProductHistoryEnabled { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("insertDateTime")]
        public DateTime InsertDateTime { get; set; }

        [JsonPropertyName("definitions")]
        public ProductDefinitionsResponseDto Definitions { get; set; }
    }
}
