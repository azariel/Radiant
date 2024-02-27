using System.Threading.Tasks;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.ResponseModels.ProductDefinitions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions
{
    /// <summary>
    /// The structure of the Products workflow.
    /// </summary>
    public interface IProductDefinitionsWorkflow
    {
        /// <summary>
        /// Execute GET accordingly to the Product Definitions request model DTO.
        /// </summary>
        /// <param name="productDefinitionsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>Product Definitions matching request dto criteria.</returns>
        public Task<ProductDefinitionsResponseDto> GetAsync(ProductDefinitionsGetRequestDto productDefinitionsRequestDto);

        /// <summary>
        /// Execute POST accordingly to the Product Definitions request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productDefinitionsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly created product definition.</returns>
        public Task<ProductDefinitionsResponseDto> PostAsync(ProductDefinitionsPostRequestDto productDefinitionsRequestDto);

        /// <summary>
        /// Execute DELETE accordingly to the Product Definitions request model DTO.
        /// </summary>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        public Task DeleteAsync(ProductDefinitionsGetRequestDto productDefinitionsRequestDto);

        /// <summary>
        /// Execute PATCH accordingly to the Product Definitions request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productDefinitionsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly modified product definition.</returns>
        public Task<ProductDefinitionsResponseDto> PatchAsync(ProductDefinitionsPatchRequestDto productDefinitionsRequestDto);
    }
}
