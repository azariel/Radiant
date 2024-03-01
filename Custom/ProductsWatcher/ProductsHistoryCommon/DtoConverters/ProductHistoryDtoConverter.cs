using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DtoConverters
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
                Price = productHistoryRequestDto.Price ?? -1,
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

        public static RadiantServerProductHistoryModel ConvertToServerProductHistoryModel(ProductHistoryResponseDto productHistoryResponseDto)
        {
            if (productHistoryResponseDto == null)
                return null;

            var _Data = new RadiantServerProductHistoryModel
            {
                DiscountPrice = productHistoryResponseDto.DiscountPrice,
                Price = productHistoryResponseDto.Price,
                DiscountPercentage = productHistoryResponseDto.DiscountPercentage,
                Title = productHistoryResponseDto.Title,
                ProductDefinitionId = productHistoryResponseDto.ProductDefinitionId,
                ShippingCost = productHistoryResponseDto.ShippingCost,
                InsertDateTime = DateTime.UtcNow,
                ProductHistoryId = productHistoryResponseDto.ProductHistoryId,
            };

            return _Data;
        }

        public static ProductHistoryPostRequestDto ConvertToProductHistoryPostRequestDto(RadiantServerProductHistoryModel radiantServerProductHistoryModel)
        {
            if (radiantServerProductHistoryModel == null)
                return null;

            var _ProductHistoryResponseDto = new ProductHistoryPostRequestDto
            {
                DiscountPrice = radiantServerProductHistoryModel.DiscountPrice,
                Price = radiantServerProductHistoryModel.Price,
                DiscountPercentage = radiantServerProductHistoryModel.DiscountPercentage,
                Title = radiantServerProductHistoryModel.Title,
                ProductDefinitionId = radiantServerProductHistoryModel.ProductDefinitionId,
                ShippingCost = radiantServerProductHistoryModel.ShippingCost,
            };

            return _ProductHistoryResponseDto;
        }
    }
}
