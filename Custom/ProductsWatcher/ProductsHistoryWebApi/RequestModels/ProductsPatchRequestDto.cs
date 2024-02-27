using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
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
