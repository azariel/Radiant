using Radiant.Common.Database.Sqlite;
using Xunit;

namespace Radiant.Common.Tests.Database.Sqlite
{
    public class SqliteManagerTests
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string TEST_CONNECTION_STRING = "";

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
