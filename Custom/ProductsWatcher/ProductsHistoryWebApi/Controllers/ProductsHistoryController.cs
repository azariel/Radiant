using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Controllers
{
    /// <summary>
    /// Controller around Product History actions
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductsHistoryController : ControllerBase
    {
        private readonly IProductHistoryWorkflow fProductHistoryWorkflow;

        public ProductsHistoryController(IProductHistoryWorkflow productHistoryWorkflow)
        {
            this.fProductHistoryWorkflow = productHistoryWorkflow;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(await fProductHistoryWorkflow.GetAsync(null).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("{productHistoryId}")]
        public async Task<IActionResult> GetByProductId([FromRoute] ProductHistoryGetRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductHistoryWorkflow.GetAsync(productsRequestDto).ConfigureAwait(false));
        }


        [HttpPost]
        public async Task<IActionResult> Post(ProductHistoryPostRequestDto productHistoryRequestDto)
        {
            return new JsonResult(await fProductHistoryWorkflow.PostAsync(productHistoryRequestDto).ConfigureAwait(false));
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(ProductHistoryPatchRequestDto productHistoryRequestDto)
        {
            return new JsonResult(await fProductHistoryWorkflow.PatchAsync(productHistoryRequestDto).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("{productHistoryId}")]
        public async Task Delete([FromRoute] ProductHistoryGetRequestDto productHistoryRequestDto)
        {
            await fProductHistoryWorkflow.DeleteAsync(productHistoryRequestDto).ConfigureAwait(false);
        }
    }
}
