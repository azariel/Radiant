using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Controllers
{
    /// <summary>
    /// Controller around Products actions
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsWorkflow fProductsWorkflow;

        public ProductsController(IProductsWorkflow productsWorkflow)
        {
            this.fProductsWorkflow = productsWorkflow;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(await fProductsWorkflow.GetAsync(null).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> GetByProductId([FromRoute] ProductsGetRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductsWorkflow.GetAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductsPostRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductsWorkflow.PostAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(ProductsPatchRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductsWorkflow.PatchAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("{productId}")]
        public async Task Delete([FromRoute] ProductsGetRequestDto productsRequestDto)
        {
            await fProductsWorkflow.DeleteAsync(productsRequestDto).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the next product that is pending refresh
        /// </summary>
        [HttpGet]
        [Route("nextPending")]
        public async Task<IActionResult> GetNextPendingProduct()
        {
            return new JsonResult(await fProductsWorkflow.GetNextPendingAsync().ConfigureAwait(false));
        }
    }
}
