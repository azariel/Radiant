using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to fetch products from products controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductsGetRequestDto
    {
        [FromRoute]
        [JsonPropertyName("productId")]
        public long ProductId { get; set; }
    }
}
