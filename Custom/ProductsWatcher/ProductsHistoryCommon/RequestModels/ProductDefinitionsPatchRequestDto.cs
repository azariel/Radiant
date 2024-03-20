using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to update product Definition
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductDefinitionsPatchRequestDto : ProductDefinitionsPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("productDefinitionId")]
        public long ProductDefinitionId { get; set; } = -1;
    }
}
