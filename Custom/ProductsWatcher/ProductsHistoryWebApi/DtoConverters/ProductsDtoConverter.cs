using System;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.Products;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.DtoConverters
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
    }
}
