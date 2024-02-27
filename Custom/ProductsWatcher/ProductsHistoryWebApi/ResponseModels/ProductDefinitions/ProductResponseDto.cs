using System;
using System.Text.Json.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductDefinitions
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductDefinitionResponseDto
    {
        public ProductDefinitionResponseDto(RadiantServerProductDefinitionModel productDefinitionFromDal)
        {
            Url = productDefinitionFromDal.Url;
            ProductDefinitionId = productDefinitionFromDal.ProductDefinitionId;
            ProductId = productDefinitionFromDal.ProductId;
            FetchProductHistoryEnabled = productDefinitionFromDal.FetchProductHistoryEnabled;
            FetchProductHistoryEveryX = productDefinitionFromDal.FetchProductHistoryEveryX;
            FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionFromDal.FetchProductHistoryTimeSpanNoiseInPerc;
            InsertDateTime = productDefinitionFromDal.InsertDateTime;
        }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("productDefinitionId")]
        public long ProductDefinitionId { get; set; }

        [JsonPropertyName("productId")]
        public long ProductId { get; set; }

        [JsonPropertyName("fetchProductHistoryEnabled")]
        public bool FetchProductHistoryEnabled { get; set; }

        [JsonPropertyName("fetchProductHistoryEveryX")]
        public TimeSpan FetchProductHistoryEveryX { get; set; }

        [JsonPropertyName("fetchProductHistoryTimeSpanNoiseInPerc")]
        public float FetchProductHistoryTimeSpanNoiseInPerc { get; set; }

        [JsonPropertyName("insertDateTime")]
        public DateTime InsertDateTime { get; set; }
    }
}
