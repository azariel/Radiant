using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Common;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    /// <summary>
    /// This DbContext is to be used ONLY in server side. It may contains sensible information
    /// </summary>
    public class ServerProductsDbContext : RadiantCommonDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ServerProductsDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;

            // We're having too much performance issue with this... we'll deal with load/include what we need manually..
            // this.ChangeTracker.LazyLoadingEnabled = true;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void OnModelCreating(ModelBuilder aModelBuilder)
        {
            base.OnModelCreating(aModelBuilder);

            //aModelBuilder.Entity<RadiantProductModel>()
            //    .HasMany(h => h.ProductHistoryCollection)
            //    .WithOne()
            //    .HasForeignKey(h=>h.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //aModelBuilder.Entity<RadiantProductModel>()
            //    .HasMany(h => h.ProductSubscriptions)
            //    .WithOne()
            //    .HasForeignKey(h=>h.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //aModelBuilder.Entity<RadiantUserProductsHistoryModel>()
            //    .HasMany(h => h.ProductSubscriptions)
            //    .WithOne()
            //    .HasForeignKey(h=>h.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
        public override string GetDataBaseFileName() => "RadiantServerProducts.db";
        
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantServerProductDefinitionModel> ProductDefinitions { get; set; }
        public DbSet<RadiantServerProductModel> Products { get; set; }
        public DbSet<RadiantServerProductHistoryModel> ProductsHistory { get; set; }
        public DbSet<RadiantServerProductSubscriptionModel> Subscriptions { get; set; }

        public new DbSet<RadiantServerUserProductsHistoryModel> Users { get; set; }
    }
}
