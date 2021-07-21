using Microsoft.EntityFrameworkCore;

namespace Radiant.Common.Database.Sqlite
{
    public class RadiantSqliteDbContext : RadiantDbContext
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        // The following configures EF to create a Sqlite database file in executing directory.
        protected override void OnConfiguring(DbContextOptionsBuilder aDbContextOptionsBuilder)
        {
            aDbContextOptionsBuilder.UseSqlite(@$"Data Source={GetDataBaseFileName()};");//Foreign Keys = False;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public virtual string GetDataBaseFileName() => "Radiant.db";
    }
}
