using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.Readers.RadiantReader.DataBase
{
    /// <summary>
    /// This DbContext is to be used ONLY in server side. It may contains sensible information
    /// </summary>
    public class RadiantReaderDbContext : RadiantSqliteDbContext
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public RadiantReaderDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = true;

            // We're having too much performance issue with this... we'll deal with load/include what we need manually..
            // this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public override string GetDataBaseFileName() => "RadiantReader.db";

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public DbSet<RadiantReaderHostModel> Hosts { get; set; }
        public DbSet<RadiantReaderBookDefinitionModel> BookDefinitions { get; set; }
        public DbSet<RadiantReaderBookChapter> BookContent { get; set; }
    }
}

