using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
{
    /// <summary>
    /// Request DTO to update product History
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductHistoryPatchRequestDto : ProductHistoryPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("productHistoryId")]
        public long ProductHistoryId { get; set; } = -1;
    }
}
