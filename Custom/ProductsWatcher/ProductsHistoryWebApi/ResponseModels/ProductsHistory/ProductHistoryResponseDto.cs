using System;
using System.Text.Json.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductsHistory
{
    /// <summary>
    /// Dto to use in response model. Those models are more restrictive than Dal models
    /// </summary>
    public class ProductHistoryResponseDto
    {
        public ProductHistoryResponseDto(RadiantServerProductHistoryModel productHistoryFromDal)
        {
            this.Title = productHistoryFromDal.Title;
            this.ProductHistoryId = productHistoryFromDal.ProductHistoryId;
            this.ProductDefinitionId = productHistoryFromDal.ProductDefinitionId;
            this.DiscountPercentage = productHistoryFromDal.DiscountPercentage;
            this.DiscountPrice = productHistoryFromDal.DiscountPrice;
            this.Price = productHistoryFromDal.Price;
            this.InsertDateTime = productHistoryFromDal.InsertDateTime;
        }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("productHistoryId")]
        public long ProductHistoryId { get; set; }

        [JsonPropertyName("productDefinitionId")]
        public long ProductDefinitionId { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("discountPrice")]
        public double? DiscountPrice { get; set; }

        [JsonPropertyName("discountPercentage")]
        public double? DiscountPercentage { get; set; }

        [JsonPropertyName("insertDateTime")]
        public DateTime InsertDateTime { get; set; }
    }
}
