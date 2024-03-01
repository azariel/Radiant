using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
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
