using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductDefinitions
{
    /// <summary>
    /// Response DTO representing products from products controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductDefinitionsResponseDto
    {
        [JsonPropertyName("productDefinitions")]
        public List<ProductDefinitionResponseDto> ProductDefinitions { get; set; }
    }
}
