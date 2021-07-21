using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Common;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    /// TODO: RadiantCommonDbContext contains USERS and we're sharing the database with clients.... so yeah... we need a dedicated database for our products
    public class ProductsDbContext : RadiantCommonDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ProductsDbContext()
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

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantProductModel> Products { get; set; }
        public DbSet<RadiantProductHistoryModel> ProductsHistory { get; set; }
        public DbSet<RadiantProductSubscriptionModel> Subscriptions { get; set; }
        public new DbSet<RadiantUserProductsHistoryModel> Users { get; set; }
    }
}
