using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductDefinitions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DtoConverters
{
    /// <summary>
    /// Convert a <see cref="RadiantServerProductDefinitionModel"/> to and from a <see cref="ProductDefinitionsResponseDto"/>
    /// </summary>
    public class ProductDefinitionsDtoConverter
    {
        public static ProductDefinitionsResponseDto ConvertToProductDefinitionsResponseDto(RadiantServerProductDefinitionModel[] productDefinitions)
        {
            if (productDefinitions == null)
                return null;

            var _ProductDefinitionsResponseDto = new ProductDefinitionsResponseDto
            {
                ProductDefinitions = productDefinitions.Select(s => new ProductDefinitionResponseDto(s)).ToList()
            };

            return _ProductDefinitionsResponseDto;
        }

        public static RadiantServerProductDefinitionModel ConvertToServerProductDefinitionModel(ProductDefinitionsPostRequestDto productDefinitionsRequestDto)
        {
            if (productDefinitionsRequestDto == null)
                return null;

            var _Data = new RadiantServerProductDefinitionModel
            {
                ProductId = productDefinitionsRequestDto.ProductId,
                Url = productDefinitionsRequestDto.Url,
                FetchProductHistoryEnabled = productDefinitionsRequestDto.FetchProductHistoryEnabled,
                FetchProductHistoryEveryX = productDefinitionsRequestDto.FetchProductHistoryEveryX,
                NextFetchProductHistory = productDefinitionsRequestDto.NextFetchProductHistory,
                FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionsRequestDto.FetchProductHistoryTimeSpanNoiseInPerc,
                InsertDateTime = DateTime.UtcNow,
            };

            if (productDefinitionsRequestDto is ProductDefinitionsPatchRequestDto _PatchRequestDto)
                _Data.ProductDefinitionId = _PatchRequestDto.ProductDefinitionId;

            return _Data;
        }

        public static RadiantServerProductDefinitionModel ConvertToServerProductDefinitionModel(ProductDefinitionResponseDto productDefinitionResponseDto, RadiantServerProductModel radiantServerProductModel = null)
        {
            if (productDefinitionResponseDto == null)
                return null;

            var _Data = new RadiantServerProductDefinitionModel
            {
                ProductId = productDefinitionResponseDto.ProductId,
                Url = productDefinitionResponseDto.Url,
                FetchProductHistoryEnabled = productDefinitionResponseDto.FetchProductHistoryEnabled,
                FetchProductHistoryEveryX = productDefinitionResponseDto.FetchProductHistoryEveryX,
                NextFetchProductHistory = productDefinitionResponseDto.NextFetchProductHistory,
                FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionResponseDto.FetchProductHistoryTimeSpanNoiseInPerc,
                ProductDefinitionId = productDefinitionResponseDto.ProductDefinitionId,
                InsertDateTime = DateTime.UtcNow,
                ProductHistoryCollection = productDefinitionResponseDto.ProductHistoryCollection?.ProductHistory?.Select(ProductHistoryDtoConverter.ConvertToServerProductHistoryModel).ToList(),
                Product = radiantServerProductModel,
            };

            return _Data;
        }

        public static ProductDefinitionsPatchRequestDto ConvertToProductDefinitionsPatchRequestDto(RadiantServerProductDefinitionModel productDefinitionDalModel)
        {
            if (productDefinitionDalModel == null)
                return null;

            var _ProductDefinitionsResponseDto = new ProductDefinitionsPatchRequestDto
            {
                ProductId = productDefinitionDalModel.ProductId,
                Url = productDefinitionDalModel.Url,
                FetchProductHistoryEnabled = productDefinitionDalModel.FetchProductHistoryEnabled,
                FetchProductHistoryEveryX = productDefinitionDalModel.FetchProductHistoryEveryX,
                FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionDalModel.FetchProductHistoryTimeSpanNoiseInPerc,
                NextFetchProductHistory = productDefinitionDalModel.NextFetchProductHistory,
                ProductDefinitionId = productDefinitionDalModel.ProductDefinitionId,
            };

            return _ProductDefinitionsResponseDto;
        }
    }
}
