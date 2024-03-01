using System.Threading.Tasks;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products
{
    /// <summary>
    /// The structure of the Products workflow.
    /// </summary>
    public interface IProductsWorkflow
    {
        /// <summary>
        /// Execute GET accordingly to the Products request model DTO.
        /// </summary>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>Products matching request dto criteria.</returns>
        public Task<ProductsResponseDto> GetAsync(ProductsGetRequestDto productsRequestDto);

        /// <summary>
        /// Execute POST accordingly to the Products request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly created product.</returns>
        public Task<ProductsResponseDto> PostAsync(ProductsPostRequestDto productsRequestDto);

        /// <summary>
        /// Execute DELETE accordingly to the Products request model DTO.
        /// </summary>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        public Task DeleteAsync(ProductsGetRequestDto productsRequestDto);

        /// <summary>
        /// Execute PATCH accordingly to the Products request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly modified product.</returns>
        public Task<ProductsResponseDto> PatchAsync(ProductsPatchRequestDto productsRequestDto);

        /// <summary>
        /// Get the next product that is pending refresh
        /// </summary>
        public Task<ProductWithDefinitionsResponseDto> GetNextPendingAsync();
    }
}
