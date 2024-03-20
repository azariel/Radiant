using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels
{
    /// <summary>
    /// Request DTO to fetch product definitions from product definitions controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductDefinitionsGetRequestDto
    {
        [FromRoute]
        [JsonPropertyName("productDefinitionId")]
        public long ProductDefinitionId { get; set; }
    }
}
