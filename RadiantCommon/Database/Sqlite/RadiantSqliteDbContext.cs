using Microsoft.EntityFrameworkCore;

namespace Radiant.Common.Database.Sqlite
{
    public class RadiantSqliteDbContext : RadiantDbContext
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        public const string DATABASE_FILENAME = "Radiant.db";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        // The following configures EF to create a Sqlite database file in executing directory.
        protected override void OnConfiguring(DbContextOptionsBuilder aDbContextOptionsBuilder)
            => aDbContextOptionsBuilder.UseSqlite(@$"Data Source={DATABASE_FILENAME};");
    }
}
