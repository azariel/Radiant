using System;
using System.Linq;
using System.Text.Json.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductDefinitions
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductDefinitionResponseDto
    {
        public ProductDefinitionResponseDto(RadiantServerProductDefinitionModel productDefinitionFromDal)
        {
            if (productDefinitionFromDal == null)
                return;

            Url = productDefinitionFromDal.Url;
            ProductDefinitionId = productDefinitionFromDal.ProductDefinitionId;
            ProductId = productDefinitionFromDal.ProductId;
            FetchProductHistoryEnabled = productDefinitionFromDal.FetchProductHistoryEnabled;
            FetchProductHistoryEveryX = productDefinitionFromDal.FetchProductHistoryEveryX;
            FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionFromDal.FetchProductHistoryTimeSpanNoiseInPerc;
            NextFetchProductHistory = productDefinitionFromDal.NextFetchProductHistory;
            InsertDateTime = productDefinitionFromDal.InsertDateTime;
            ProductHistoryCollection = new ProductsHistoryResponseDto
            {
                ProductHistory = productDefinitionFromDal.ProductHistoryCollection.Select(s=> new ProductHistoryResponseDto(s)).ToList()
            };;
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

        [JsonPropertyName("nextFetchProductHistory")]
        public DateTime? NextFetchProductHistory { get; set; }

        [JsonPropertyName("insertDateTime")]
        public DateTime InsertDateTime { get; set; }

        [JsonPropertyName("productHistoryCollection")]
        public ProductsHistoryResponseDto ProductHistoryCollection { get; set; }
    }
}
