using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Notifier.DataBase
{
    public class NotificationsDbContext : RadiantSqliteDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public NotificationsDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;

            // We're having too much performance issue with this... we'll deal with load/include what we need manually..
            // this.ChangeTracker.LazyLoadingEnabled = true;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override string GetDataBaseFileName() => "Notifications.db";

        protected override void OnConfiguring(DbContextOptionsBuilder aOptionsBuilder)
        {
            base.OnConfiguring(aOptionsBuilder);

            //aOptionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder aModelBuilder)
        {
            base.OnModelCreating(aModelBuilder);

            // Handle list of strings in a single string field
            aModelBuilder.Entity<RadiantNotificationModel>()
                .Property(e => e.EmailTo)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantNotificationModel> Notifications { get; set; }
    }
}
