using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Controllers
{
    /// <summary>
    /// Controller around Product Definitions actions
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductDefinitionsController : ControllerBase
    {
        private readonly IProductDefinitionsWorkflow fProductDefinitionsWorkflow;

        public ProductDefinitionsController(IProductDefinitionsWorkflow productDefinitionsWorkflow)
        {
            this.fProductDefinitionsWorkflow = productDefinitionsWorkflow;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return new JsonResult(await fProductDefinitionsWorkflow.GetAsync(null).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("product/{productId}")]
        public async Task<IActionResult> GetByProductId([FromRoute] ProductDefinitionsGetByProductRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductDefinitionsWorkflow.GetByProductAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("{productDefinitionId}")]
        public async Task<IActionResult> GetByProductDefinitionId([FromRoute] ProductDefinitionsGetRequestDto productsRequestDto)
        {
            return new JsonResult(await fProductDefinitionsWorkflow.GetAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductDefinitionsPostRequestDto productDefinitionsRequestDto)
        {
            return new JsonResult(await fProductDefinitionsWorkflow.PostAsync(productDefinitionsRequestDto).ConfigureAwait(false));
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(ProductDefinitionsPatchRequestDto productDefinitionsRequestDto)
        {
            return new JsonResult(await fProductDefinitionsWorkflow.PatchAsync(productDefinitionsRequestDto).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("{productDefinitionId}")]
        public async Task Delete([FromRoute] ProductDefinitionsGetRequestDto productDefinitionsRequestDto)
        {
            await fProductDefinitionsWorkflow.DeleteAsync(productDefinitionsRequestDto).ConfigureAwait(false);
        }
    }
}
