using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
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
