using Microsoft.Extensions.DependencyInjection;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductDefinitions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.Products;
using Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Workflows.ProductsHistory;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryWebApi.Installers
{
    public static class Installer
    {
        public static void ConfigureDependencies(IServiceCollection services)
        {
            // Workflows
            services.AddSingleton<IProductsWorkflow, ProductsWorkflow>();
            services.AddSingleton<IProductDefinitionsWorkflow, ProductDefinitionsWorkflow>();
            services.AddSingleton<IProductHistoryWorkflow, ProductHistoryWorkflow>();
        }
    }
}
