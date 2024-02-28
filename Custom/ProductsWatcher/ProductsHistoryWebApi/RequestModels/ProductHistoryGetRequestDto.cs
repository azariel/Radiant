using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
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
