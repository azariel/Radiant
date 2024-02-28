using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.DtoConverters
{
    /// <summary>
    /// Convert a <see cref="RadiantServerProductHistoryModel"/> to and from a <see cref="ProductHistoryResponseDto"/>
    /// </summary>
    public class ProductHistoryDtoConverter
    {
        public static ProductsHistoryResponseDto ConvertToProductHistoryResponseDto(RadiantServerProductHistoryModel[] productHistory)
        {
            if (productHistory == null)
                return null;

            var _ProductHistoryResponseDto = new ProductsHistoryResponseDto
            {
                ProductHistory = productHistory.Select(s => new ProductHistoryResponseDto(s)).ToList()
            };

            return _ProductHistoryResponseDto;
        }

        public static RadiantServerProductHistoryModel ConvertToServerProductHistoryModel(ProductHistoryPostRequestDto productHistoryRequestDto)
        {
            if (productHistoryRequestDto == null)
                return null;

            var _Data = new RadiantServerProductHistoryModel
            {
                DiscountPrice = productHistoryRequestDto.DiscountPrice,
                Price = productHistoryRequestDto.Price,
                DiscountPercentage = productHistoryRequestDto.DiscountPercentage,
                Title = productHistoryRequestDto.Title,
                ProductDefinitionId = productHistoryRequestDto.ProductDefinitionId,
                ShippingCost = productHistoryRequestDto.ShippingCost,
                InsertDateTime = DateTime.UtcNow,
            };

            if (productHistoryRequestDto is ProductHistoryPatchRequestDto _PatchRequestDto)
                _Data.ProductHistoryId = _PatchRequestDto.ProductHistoryId;

            return _Data;
        }
    }
}
