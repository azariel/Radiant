using System.Threading.Tasks;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductsHistory
{
    /// <summary>
    /// The structure of the Products workflow.
    /// </summary>
    public interface IProductHistoryWorkflow
    {
        /// <summary>
        /// Execute GET accordingly to the Product History request model DTO.
        /// </summary>
        /// <param name="productHistoryRequestDto">requestDto model related to action to execute.</param>
        /// <returns>Product History matching request dto criteria.</returns>
        public Task<ProductsHistoryResponseDto> GetAsync(ProductHistoryGetRequestDto productHistoryRequestDto);

        /// <summary>
        /// Execute POST accordingly to the Product History request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productHistoryRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly created product definition.</returns>
        public Task<ProductsHistoryResponseDto> PostAsync(ProductHistoryPostRequestDto productHistoryRequestDto);

        /// <summary>
        /// Execute DELETE accordingly to the Product History request model DTO.
        /// </summary>
        /// <param name="productsRequestDto">requestDto model related to action to execute.</param>
        public Task DeleteAsync(ProductHistoryGetRequestDto productHistoryRequestDto);

        /// <summary>
        /// Execute PATCH accordingly to the Product History request model DTO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productHistoryRequestDto">requestDto model related to action to execute.</param>
        /// <returns>newly modified product definition.</returns>
        public Task<ProductsHistoryResponseDto> PatchAsync(ProductHistoryPatchRequestDto productHistoryRequestDto);
    }
}
