using Microsoft.Extensions.DependencyInjection;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Installers
{
    public static class Installer
    {
        public static void ConfigureDependencies(IServiceCollection services)
        {
            // Workflows
            services.AddSingleton<IProductsWorkflow, ProductsWorkflow>();
            services.AddSingleton<IProductDefinitionsWorkflow, ProductDefinitionsWorkflow>();
        }
    }
}
