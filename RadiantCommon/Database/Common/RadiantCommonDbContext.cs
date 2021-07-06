using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Common.Database.Common
{
    public class RadiantCommonDbContext : RadiantSqliteDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public RadiantCommonDbContext()
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

        protected override string GetDataBaseFileName() => "RadiantCommon.db";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantUserModel> Users { get; set; }
    }
}

