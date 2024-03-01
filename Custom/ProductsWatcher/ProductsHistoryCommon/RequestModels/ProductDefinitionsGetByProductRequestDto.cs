using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to fetch product definitions from product definitions controller by product id
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductDefinitionsGetByProductRequestDto
    {
        [FromRoute]
        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [FromQuery]
        [JsonPropertyName("includeHistory")]
        public bool IncludeHistory { get; set; }
    }
}
