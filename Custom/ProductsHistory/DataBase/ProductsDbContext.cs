using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistory.DataBase
{
    public class ProductsDbContext : RadiantSqliteDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ProductsDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;
            this.ChangeTracker.LazyLoadingEnabled = true;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<Product> Products { get; set; }
    }
}
