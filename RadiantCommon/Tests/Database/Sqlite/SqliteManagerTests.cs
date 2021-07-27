using Radiant.Common.Database.Sqlite;
using Xunit;

namespace Radiant.Common.Tests.Database.Sqlite
{
    public class SqliteManagerTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void SqliteBasicTest()
        {
            using var _DataBaseContext = new RadiantSqliteDbContext();
        }
    }
}
