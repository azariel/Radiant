using Microsoft.EntityFrameworkCore;
using Radiant.Common.Exceptions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.DtoConverters;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductDefinitions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions
{
    /// <summary>
    /// Actions related to Product Definitions
    /// </summary>
    public class ProductDefinitionsWorkflow : IProductDefinitionsWorkflow
    {
        public async Task<ProductDefinitionsResponseDto> GetAsync(ProductDefinitionsGetRequestDto productDefinitionsRequestDto)
        {
            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);
            //await _DataBaseContext.ProductsHistory.LoadAsync().ConfigureAwait(false);

            RadiantServerProductDefinitionModel[] _FilteredProducts = productDefinitionsRequestDto == null ? _DataBaseContext.ProductDefinitions.ToArray() : _DataBaseContext.ProductDefinitions.Where(w => w.ProductDefinitionId == productDefinitionsRequestDto.ProductDefinitionId).ToArray();

            if (!_FilteredProducts.Any())
                throw new ApiException(HttpStatusCode.NotFound, $"Product Definition Id [{productDefinitionsRequestDto?.ProductDefinitionId}] not found.");

            return ProductDefinitionsDtoConverter.ConvertToProductDefinitionsResponseDto(_FilteredProducts);
        }

        public async Task<ProductDefinitionsResponseDto> PostAsync(ProductDefinitionsPostRequestDto productDefinitionsRequestDto)
        {
            if (productDefinitionsRequestDto == null || string.IsNullOrWhiteSpace(productDefinitionsRequestDto.Url))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product definition [{nameof(ProductDefinitionsPostRequestDto.Url)}] is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);

            // Check that the definition match an existing product
            var _Product = _DataBaseContext.Products.Where(w => w.ProductId == productDefinitionsRequestDto.ProductId).ToArray();

            if(_Product.Length > 1)
                throw new ApiException(HttpStatusCode.Conflict, $"Multiple products matching Id [{productDefinitionsRequestDto.ProductId}] found.");

            if(_Product.Length <= 0)
                throw new ApiException(HttpStatusCode.NotFound, $"No product matching Id [{productDefinitionsRequestDto.ProductId}] to associate the new definition was found.");

            var _LowerInvariantProductDefinitionsRequestUrl = productDefinitionsRequestDto.Url.ToLowerInvariant();

            // Check that product definition doesn't already exists for this url
            if (_DataBaseContext.ProductDefinitions.Any(w => w.Url.ToLower() == _LowerInvariantProductDefinitionsRequestUrl))
            {
                throw new ApiException(HttpStatusCode.Conflict, $"The product definition url [{productDefinitionsRequestDto.Url}] already exists.");
            }

            // Add the new product Definition
            _DataBaseContext.ProductDefinitions.Add(ProductDefinitionsDtoConverter.ConvertToServerProductDefinitionModel(productDefinitionsRequestDto));
            await _DataBaseContext.SaveChangesAsync();

            // Get product Definition from DB
            var _FilteredProductDefinitions = _DataBaseContext.ProductDefinitions.Where(w => w.Url == productDefinitionsRequestDto.Url).ToArray();

            if (_FilteredProductDefinitions.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product Definition [{productDefinitionsRequestDto.Url}] was added to the DB, but couldn't be found afterwards.");

            return ProductDefinitionsDtoConverter.ConvertToProductDefinitionsResponseDto(new[] { _FilteredProductDefinitions.Single() });
        }

        public async Task DeleteAsync(ProductDefinitionsGetRequestDto productDefinitionsRequestDto)
        {
            var _DataToDelete = await GetAsync(productDefinitionsRequestDto).ConfigureAwait(false);

            if (_DataToDelete.ProductDefinitions.Count != 1)
                throw new ApiException(HttpStatusCode.Conflict, $"Can't delete product Definition id [{productDefinitionsRequestDto.ProductDefinitionId}]. [{_DataToDelete.ProductDefinitions.Count}] products found with specified Id.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);

            await _DataBaseContext.ProductDefinitions.Where(w => w.ProductDefinitionId == _DataToDelete.ProductDefinitions.Single().ProductDefinitionId).ExecuteDeleteAsync().ConfigureAwait(false);
            await _DataBaseContext.SaveChangesAsync();
        }

        public async Task<ProductDefinitionsResponseDto> PatchAsync(ProductDefinitionsPatchRequestDto productDefinitionsRequestDto)
        {
            if (productDefinitionsRequestDto == null || string.IsNullOrWhiteSpace(productDefinitionsRequestDto.Url))
                throw new ApiException(HttpStatusCode.BadRequest, $"A product Definition [{nameof(ProductDefinitionsPatchRequestDto.Url)}] is required.");

            if (productDefinitionsRequestDto == null || productDefinitionsRequestDto.ProductDefinitionId < 0)
                throw new ApiException(HttpStatusCode.NotFound, $"A valid product Definition [{nameof(ProductDefinitionsPatchRequestDto.ProductDefinitionId)}] is required.");

            if (productDefinitionsRequestDto == null || productDefinitionsRequestDto.ProductId < 0)
                throw new ApiException(HttpStatusCode.NotFound, $"A valid product [{nameof(ProductDefinitionsPatchRequestDto.ProductId)}] is required.");

            // Load database
            await using var _DataBaseContext = new ServerProductsDbContext();
            await _DataBaseContext.Products.LoadAsync().ConfigureAwait(false);
            await _DataBaseContext.ProductDefinitions.LoadAsync().ConfigureAwait(false);

            // Check that associated product exists
            if(_DataBaseContext.Products.All(w=>w.ProductId != productDefinitionsRequestDto.ProductId))
                throw new ApiException(HttpStatusCode.NotFound, $"The product Id [{productDefinitionsRequestDto.ProductId}] couldn't be found.");

            var _ProductDefinitionsToUpdate = _DataBaseContext.ProductDefinitions.Where(w => w.ProductDefinitionId == productDefinitionsRequestDto.ProductDefinitionId).ToArray();
            foreach (var _ProductDefinitionToUpdate in _ProductDefinitionsToUpdate)
            {
                // Cherry picking what we update as we don't want the user to be able to update certain fields
                _ProductDefinitionToUpdate.Url = productDefinitionsRequestDto.Url;
                _ProductDefinitionToUpdate.FetchProductHistoryEnabled = productDefinitionsRequestDto.FetchProductHistoryEnabled;
                _ProductDefinitionToUpdate.FetchProductHistoryEveryX = productDefinitionsRequestDto.FetchProductHistoryEveryX;
                _ProductDefinitionToUpdate.FetchProductHistoryTimeSpanNoiseInPerc = productDefinitionsRequestDto.FetchProductHistoryTimeSpanNoiseInPerc;
            }

            await _DataBaseContext.SaveChangesAsync();

            // Get product definition from DB
            var _FilteredProductDefinitions = _DataBaseContext.ProductDefinitions.Where(w => w.ProductDefinitionId == productDefinitionsRequestDto.ProductDefinitionId).ToArray();

            if (_FilteredProductDefinitions.Length != 1)
                throw new ApiException(HttpStatusCode.InternalServerError, $"The product Definition [{productDefinitionsRequestDto.ProductDefinitionId}] was updated, but couldn't be found afterwards.");

            return ProductDefinitionsDtoConverter.ConvertToProductDefinitionsResponseDto(new[] { _FilteredProductDefinitions.Single() });
        }
    }
}
