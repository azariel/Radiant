using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Exceptions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DtoConverters;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductsHistory
{
    /// <summary>
    /// Actions related to Product History
    /// </summary>
    public class ProductHistoryWorkflow : IProductHistoryWorkflow
    {
        public async Task<ProductsHistoryResponseDto> GetAsync(ProductHistoryGetRequestDto productHistoryRequestDto)
        {
            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            RadiantServerProductHistoryModel[] _FilteredProducts = productHistoryRequestDto == null ? _DataBaseContext.ProductsHistory.ToArray() : _DataBaseContext.ProductsHistory.Where(w => w.ProductHistoryId == productHistoryRequestDto.ProductHistoryId).ToArray();

            if (!_FilteredProducts.Any())
                throw new ApiException(HttpStatusCode.NotFound, $"Product History Id [{productHistoryRequestDto?.ProductHistoryId}] not found.");

            return ProductHistoryDtoConverter.ConvertToProductHistoryResponseDto(_FilteredProducts);
        }

        public async Task<ProductsHistoryResponseDto> PostAsync(ProductHistoryPostRequestDto productHistoryRequestDto)
        {
            if (productHistoryRequestDto == null || string.IsNullOrWhiteSpace(productHistoryRequestDto.Title))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product History [{nameof(ProductHistoryPostRequestDto.Title)}] is required.");

            if (productHistoryRequestDto.Price < 0)
                throw new ApiException(HttpStatusCode.BadRequest, $"A [{nameof(ProductHistoryPostRequestDto.Price)}] greater or equals to 0 is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            // Check that the History match an existing product definition
            var _Product = _DataBaseContext.ProductDefinitions.Where(w => w.ProductDefinitionId == productHistoryRequestDto.ProductDefinitionId).ToArray();

            if(_Product.Length > 1)
                throw new ApiException(HttpStatusCode.Conflict, $"Multiple Product Definition matching Id [{productHistoryRequestDto.ProductDefinitionId}] found.");

            if(_Product.Length <= 0)
                throw new ApiException(HttpStatusCode.NotFound, $"No Product Definition matching Id [{productHistoryRequestDto.ProductDefinitionId}] to associate the new History was found.");

            var _LowerInvariantProductHistoryRequestTitle = productHistoryRequestDto.Title.ToLowerInvariant();

            // Add the new product History
            var productHistoryToAdd = ProductHistoryDtoConverter.ConvertToServerProductHistoryModel(productHistoryRequestDto);
            _DataBaseContext.ProductsHistory.Add(productHistoryToAdd);
            await _DataBaseContext.SaveChangesAsync();

            // Get product History from DB
            var _FilteredProductHistory = _DataBaseContext.ProductsHistory.Where(w => w.InsertDateTime == productHistoryToAdd.InsertDateTime).ToArray();

            if (_FilteredProductHistory.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product History [{productHistoryRequestDto.Title}] was added to the DB, but couldn't be found afterwards.");

            return ProductHistoryDtoConverter.ConvertToProductHistoryResponseDto(new[] { _FilteredProductHistory.Single() });
        }

        public async Task DeleteAsync(ProductHistoryGetRequestDto productHistoryRequestDto)
        {
            var _DataToDelete = await GetAsync(productHistoryRequestDto).ConfigureAwait(false);

            if (_DataToDelete.ProductHistory.Count != 1)
                throw new ApiException(HttpStatusCode.Conflict, $"Can't delete product History id [{productHistoryRequestDto.ProductHistoryId}]. [{_DataToDelete.ProductHistory.Count}] products history found with specified Id.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);

            await _DataBaseContext.ProductsHistory.Where(w => w.ProductHistoryId == _DataToDelete.ProductHistory.Single().ProductHistoryId).ExecuteDeleteAsync().ConfigureAwait(false);
            await _DataBaseContext.SaveChangesAsync();
        }

        public async Task<ProductsHistoryResponseDto> PatchAsync(ProductHistoryPatchRequestDto productHistoryRequestDto)
        {
            if (productHistoryRequestDto == null || string.IsNullOrWhiteSpace(productHistoryRequestDto.Title))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product History [{nameof(ProductHistoryPatchRequestDto.Title)}] is required.");

            if (productHistoryRequestDto == null || productHistoryRequestDto.ProductHistoryId < 0)
                throw new ApiException(HttpStatusCode.NotFound, $"A valid product History [{nameof(ProductHistoryPatchRequestDto.ProductHistoryId)}] is required.");

            if (productHistoryRequestDto == null || productHistoryRequestDto.ProductDefinitionId < 0)
                throw new ApiException(HttpStatusCode.NotFound, $"A valid product definition [{nameof(ProductHistoryPatchRequestDto.ProductDefinitionId)}] is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            // Check that product history exists
            if(_DataBaseContext.ProductsHistory.All(w=> w.ProductHistoryId != productHistoryRequestDto.ProductHistoryId))
                throw new ApiException(HttpStatusCode.NotFound, $"The product history Id [{productHistoryRequestDto.ProductHistoryId}] couldn't be found.");

            // Check that product definition exists
            if(_DataBaseContext.ProductDefinitions.All(w=> w.ProductDefinitionId != productHistoryRequestDto.ProductDefinitionId))
                throw new ApiException(HttpStatusCode.NotFound, $"The product definition Id [{productHistoryRequestDto.ProductDefinitionId}] couldn't be found.");

            var _ProductsHistoryToUpdate = _DataBaseContext.ProductsHistory.Where(w => w.ProductHistoryId == productHistoryRequestDto.ProductHistoryId).ToArray();
            foreach (var _ProductHistoryToUpdate in _ProductsHistoryToUpdate)
            {
                // Cherry picking what we update as we don't want the user to be able to update certain fields
                _ProductHistoryToUpdate.Title = productHistoryRequestDto.Title;
                _ProductHistoryToUpdate.ProductDefinitionId = productHistoryRequestDto.ProductDefinitionId;
                _ProductHistoryToUpdate.ShippingCost = productHistoryRequestDto.ShippingCost;
                _ProductHistoryToUpdate.DiscountPercentage = productHistoryRequestDto.DiscountPercentage;
                _ProductHistoryToUpdate.DiscountPrice = productHistoryRequestDto.DiscountPrice;
                _ProductHistoryToUpdate.Price = productHistoryRequestDto.Price ?? -1;
            }

            await _DataBaseContext.SaveChangesAsync();

            // Get product History from DB
            var _FilteredProductHistory = _DataBaseContext.ProductsHistory.Where(w => w.ProductHistoryId == productHistoryRequestDto.ProductHistoryId).ToArray();

            if (_FilteredProductHistory.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product History [{productHistoryRequestDto.ProductHistoryId}] was updated, but couldn't be found afterwards.");

            return ProductHistoryDtoConverter.ConvertToProductHistoryResponseDto(new[] { _FilteredProductHistory.Single() });
        }
    }
}
