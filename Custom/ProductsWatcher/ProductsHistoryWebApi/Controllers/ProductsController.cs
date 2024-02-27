using Microsoft.AspNetCore.Mvc;
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.RequestModels;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products;
using System.Threading.Tasks;

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
        public async Task<string> Get()
        {
            return JsonCommonSerializer.SerializeToString(await fProductsWorkflow.GetAsync(null).ConfigureAwait(false));
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<string> GetByProductId([FromRoute] ProductsGetRequestDto productsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductsWorkflow.GetAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<string> Post(ProductsPostRequestDto productsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductsWorkflow.PostAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpPatch]
        public async Task<string> Patch(ProductsPatchRequestDto productsRequestDto)
        {
            return JsonCommonSerializer.SerializeToString(await fProductsWorkflow.PatchAsync(productsRequestDto).ConfigureAwait(false));
        }

        [HttpDelete]
        [Route("{productId}")]
        public async Task Delete([FromRoute] ProductsGetRequestDto productsRequestDto)
        {
            await fProductsWorkflow.DeleteAsync(productsRequestDto).ConfigureAwait(false);
        }
    }
}
