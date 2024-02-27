using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductDefinitions;
using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.DtoConverters
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
                FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionsRequestDto.FetchProductHistoryTimeSpanNoiseInPerc,
                InsertDateTime = DateTime.UtcNow,
            };

            if (productDefinitionsRequestDto is ProductDefinitionsPatchRequestDto _PatchRequestDto)
                _Data.ProductDefinitionId = _PatchRequestDto.ProductDefinitionId;

            return _Data;
        }
    }
}
