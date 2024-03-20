using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
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
        public double? DiscountPrice { get; set; }

        [FromBody]
        [JsonPropertyName("discountPercentage")]
        public double? DiscountPercentage { get; set; }

        [FromBody]
        [JsonPropertyName("price")]
        public double? Price { get; set; }

        [FromBody]
        [JsonPropertyName("shippingCost")]
        public double? ShippingCost { get; set; }
    }
}
