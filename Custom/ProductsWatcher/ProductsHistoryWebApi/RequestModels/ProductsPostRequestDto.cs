using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
{
    /// <summary>
    /// Request DTO to fetch products from products controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductsPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [FromBody]
        [JsonPropertyName("fetchProductHistoryEnabled")]
        public bool FetchProductHistoryEnabled { get; set; }
    }
}
