using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DtoConverters
{
    /// <summary>
    /// Convert a <see cref="RadiantServerProductModel"/> to and from a <see cref="ProductsResponseDto"/>
    /// </summary>
    public class ProductsDtoConverter
    {
        public static ProductsResponseDto ConvertToProductsResponseDto(RadiantServerProductModel[] products)
        {
            if (products == null)
                return null;

            var _ProductsResponseDto = new ProductsResponseDto
            {
                Products = products.Select(s => new ProductResponseDto(s)).ToList()
            };

            return _ProductsResponseDto;
        }

        public static RadiantServerProductModel ConvertToServerProductModel(ProductsPostRequestDto productsRequestDto)
        {
            if (productsRequestDto == null)
                return null;

            var _Data = new RadiantServerProductModel
            {
                Name = productsRequestDto.Name,
                FetchProductHistoryEnabled = productsRequestDto.FetchProductHistoryEnabled,
                InsertDateTime = DateTime.UtcNow,
            };

            if (productsRequestDto is ProductsPatchRequestDto _PatchRequestDto)
                _Data.ProductId = _PatchRequestDto.ProductId;

            return _Data;
        }

        public static RadiantServerProductModel ConvertToServerProductModel(ProductWithDefinitionsResponseDto productWithDefinitionsResponseDto)
        {
            if (productWithDefinitionsResponseDto == null)
                return null;

            var _Data = new RadiantServerProductModel
            {
                Name = productWithDefinitionsResponseDto.Name,
                FetchProductHistoryEnabled = productWithDefinitionsResponseDto.FetchProductHistoryEnabled,
                InsertDateTime = DateTime.UtcNow,
                ProductId = productWithDefinitionsResponseDto.ProductId,
                ProductDefinitionCollection = productWithDefinitionsResponseDto.Definitions?.ProductDefinitions?.Select(s => ProductDefinitionsDtoConverter.ConvertToServerProductDefinitionModel(s)).ToList(),
                //ProductSubscriptions = TODO
            };

            return _Data;
        }

        public static ProductWithDefinitionsResponseDto ConvertToProductWithDefinitionsResponseDto(RadiantServerProductModel product)
        {
            if (product == null)
                return null;

            return new ProductWithDefinitionsResponseDto(product);
        }
    }
}
