using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
{
    /// <summary>
    /// Request DTO to fetch products from product History controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductHistoryPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("productDefinitionId")]
        public long ProductDefinitionId { get; set; } = -1;

        [FromBody]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [FromBody]
        [JsonPropertyName("discountPrice")]
        public float DiscountPrice { get; set; }

        [FromBody]
        [JsonPropertyName("discountPercentage")]
        public float DiscountPercentage { get; set; }

        [FromBody]
        [JsonPropertyName("price")]
        public float Price { get; set; }

        [FromBody]
        [JsonPropertyName("shippingCost")]
        public float ShippingCost { get; set; }
    }
}
