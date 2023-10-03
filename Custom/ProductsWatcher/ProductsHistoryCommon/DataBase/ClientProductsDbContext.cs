using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Sqlite;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    /// <summary>
    /// This DbContext is to be used ONLY in server side. It may contains sensible information
    /// </summary>
    public class ClientProductsDbContext : RadiantSqliteDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ClientProductsDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;

            // We're having too much performance issue with this... we'll deal with load/include what we need manually..
            // this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public override string GetDataBaseFileName() => "RadiantClientProducts.db";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantClientProductDefinitionModel> ProductDefinitions { get; set; }
        public DbSet<RadiantClientProductModel> Products { get; set; }
        public DbSet<RadiantClientProductHistoryModel> ProductsHistory { get; set; }
        public DbSet<RadiantClientProductSubscriptionModel> Subscriptions { get; set; }

        /// <summary>
        /// Users is using a special ClientUsers DataTable unique to client side.
        /// It doesn't contains any sensitive information and can be freely shared or disclosed
        /// </summary>
        public DbSet<RadiantClientUserProductsHistoryModel> Users { get; set; }
    }
}
