using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels
{
    /// <summary>
    /// Request DTO to fetch products from product Definitions controller
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProductDefinitionsPostRequestDto
    {
        [FromBody]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [FromBody]
        [JsonPropertyName("productId")]
        public long ProductId { get; set; } = -1;

        [FromBody]
        [JsonPropertyName("fetchProductHistoryEnabled")]
        public bool FetchProductHistoryEnabled { get; set; }

        [FromBody]
        [JsonPropertyName("fetchProductHistoryEveryX")]
        public TimeSpan FetchProductHistoryEveryX { get; set; }

        [FromBody]
        [JsonPropertyName("fetchProductHistoryTimeSpanNoiseInPerc")]
        public float FetchProductHistoryTimeSpanNoiseInPerc { get; set; }
    }
}
