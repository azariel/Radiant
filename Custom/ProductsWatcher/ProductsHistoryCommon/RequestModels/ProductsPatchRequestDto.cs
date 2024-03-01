using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to update product
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductsPatchRequestDto : ProductsPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("productId")]
        public long ProductId { get; set; } = -1;
    }
}
