using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to fetch product History from product History controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductHistoryGetRequestDto
    {
        [FromRoute]
        [JsonPropertyName("productHistoryId")]
        public long ProductHistoryId { get; set; }
    }
}
