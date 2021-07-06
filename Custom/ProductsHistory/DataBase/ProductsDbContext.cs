﻿using Microsoft.EntityFrameworkCore;
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

            // We're having too much performance issue with this... we'll deal with load/include what we need manually..
           // this.ChangeTracker.LazyLoadingEnabled = true;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void OnConfiguring(DbContextOptionsBuilder aOptionsBuilder)
        {
            base.OnConfiguring(aOptionsBuilder);

            //aOptionsBuilder.UseLazyLoadingProxies();
        }

        protected override string GetDataBaseFileName() => "RadiantProductsHistory.db";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantProductModel> Products { get; set; }
    }
}
