using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory
{
    /// <summary>
    /// Response DTO representing products from products controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductsHistoryResponseDto
    {
        [JsonPropertyName("productHistory")]
        public List<ProductHistoryResponseDto> ProductHistory { get; set; }
    }
}
