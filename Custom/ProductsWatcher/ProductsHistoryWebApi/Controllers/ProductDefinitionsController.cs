using Microsoft.AspNetCore.Mvc;
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions;
using System.Threading.Tasks;

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
        public async Task<string> Get()
        {
            return JsonCommonSerializer.SerializeToString(await fProductDefinitionsWorkflow.GetAsync(null).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("{productDefinitionId}")]
        public async Task<string> GetByProductId([FromRoute] ProductDefinitionsGetRequestDto productsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductDefinitionsWorkflow.GetAsync(productsRequestDto).ConfigureAwait(false));
        }


        [HttpPost]
        public async Task<string> Post(ProductDefinitionsPostRequestDto productDefinitionsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductDefinitionsWorkflow.PostAsync(productDefinitionsRequestDto).ConfigureAwait(false));
        }

        [HttpPatch]
        public async Task<string> Patch(ProductDefinitionsPatchRequestDto productDefinitionsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductDefinitionsWorkflow.PatchAsync(productDefinitionsRequestDto).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("{productDefinitionId}")]
        public async Task Delete([FromRoute] ProductDefinitionsGetRequestDto productDefinitionsRequestDto)
        {
            await fProductDefinitionsWorkflow.DeleteAsync(productDefinitionsRequestDto).ConfigureAwait(false);
        }
    }
}
