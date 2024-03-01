using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products
{
    /// <summary>
    /// Response DTO representing products from products controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductsResponseDto
    {
        [JsonPropertyName("products")]
        public List<ProductResponseDto> Products { get; set; }
    }
}
