using Microsoft.EntityFrameworkCore;
using Radiant.Common.Exceptions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DtoConverters;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products
{
    /// <summary>
    /// Actions related to Products
    /// </summary>
    public class ProductsWorkflow : IProductsWorkflow
    {
        public async Task<ProductsResponseDto> GetAsync(ProductsGetRequestDto productRequestDto)
        {
            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            //await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            //await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            RadiantServerProductModel[] _FilteredProducts = productRequestDto == null ? _DataBaseContext.Products.ToArray() : _DataBaseContext.Products.Where(w => w.ProductId == productRequestDto.ProductId).ToArray();

            if (!_FilteredProducts.Any())
                throw new ApiException(HttpStatusCode.NotFound, $"Product Id [{productRequestDto?.ProductId}] not found.");

            return ProductsDtoConverter.ConvertToProductsResponseDto(_FilteredProducts);
        }

        public async Task<ProductsResponseDto> PostAsync(ProductsPostRequestDto productsRequestDto)
        {
            if (productsRequestDto == null || string.IsNullOrWhiteSpace(productsRequestDto.Name))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product [{nameof(ProductsPostRequestDto.Name)}] is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);

            var _LowerInvariantProductsRequestName = productsRequestDto.Name.ToLowerInvariant();

            // Check that product doesn't already exists
            if (_DataBaseContext.Products.Any(w => w.Name.ToLower() == _LowerInvariantProductsRequestName))
            {
                throw new ApiException(HttpStatusCode.Conflict, $"The product [{productsRequestDto.Name}] already exists.");
            }

            // Add the new product
            _DataBaseContext.Products.Add(ProductsDtoConverter.ConvertToServerProductModel(productsRequestDto));
            await _DataBaseContext.SaveChangesAsync();

            // Get product from DB
            var _FilteredProducts = _DataBaseContext.Products.Where(w => w.Name == productsRequestDto.Name).ToArray();

            if (_FilteredProducts.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product [{productsRequestDto.Name}] was added to the DB, but couldn't be found afterwards.");

            return ProductsDtoConverter.ConvertToProductsResponseDto(new[] { _FilteredProducts.Single() });
        }

        public async Task DeleteAsync(ProductsGetRequestDto productsRequestDto)
        {
            var _DataToDelete = await GetAsync(productsRequestDto).ConfigureAwait(false);

            if (_DataToDelete.Products.Count != 1)
                throw new ApiException(HttpStatusCode.Conflict, $"Can't delete product id [{productsRequestDto.ProductId}]. [{_DataToDelete.Products.Count}] products found with specified Id.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);

            await _DataBaseContext.Products.Where(w => w.ProductId == _DataToDelete.Products.Single().ProductId).ExecuteDeleteAsync().ConfigureAwait(false);
            await _DataBaseContext.SaveChangesAsync();
        }

        public async Task<ProductsResponseDto> PatchAsync(ProductsPatchRequestDto productsRequestDto)
        {
            if (productsRequestDto == null || string.IsNullOrWhiteSpace(productsRequestDto.Name))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product [{nameof(ProductsPatchRequestDto.Name)}] is required.");

            if (productsRequestDto == null || productsRequestDto.ProductId < 0)
                throw new ApiException(HttpStatusCode.NotFound, $"A valid product [{nameof(ProductsPatchRequestDto.ProductId)}] is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);

            var _ProductsToUpdate = _DataBaseContext.Products.Where(w => w.ProductId == productsRequestDto.ProductId).ToArray();
            foreach (var _ProductToUpdate in _ProductsToUpdate)
            {
                // Cherry picking what we update as we don't want the user to be able to update certain fields
                _ProductToUpdate.Name = productsRequestDto.Name;
                _ProductToUpdate.FetchProductHistoryEnabled = productsRequestDto.FetchProductHistoryEnabled;
            }

            await _DataBaseContext.SaveChangesAsync();

            // Get product from DB
            var _FilteredProducts = _DataBaseContext.Products.Where(w => w.ProductId == productsRequestDto.ProductId).ToArray();

            if (_FilteredProducts.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product [{productsRequestDto.ProductId}] was updated, but couldn't be found afterwards.");

            return ProductsDtoConverter.ConvertToProductsResponseDto(new[] { _FilteredProducts.Single() });
        }

        public async Task<ProductWithDefinitionsResponseDto> GetNextPendingAsync()
        {
            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            RadiantServerProductModel _FilteredProduct = _DataBaseContext.Products.FirstOrDefault(w => w.FetchProductHistoryEnabled && w.ProductDefinitionCollection.Any(a=>a.FetchProductHistoryEnabled && (a.NextFetchProductHistory == null || a.NextFetchProductHistory <= DateTime.UtcNow)));

            if (_FilteredProduct == null)
                return null;

            return ProductsDtoConverter.ConvertToProductWithDefinitionsResponseDto(_FilteredProduct);
        }
    }
}
