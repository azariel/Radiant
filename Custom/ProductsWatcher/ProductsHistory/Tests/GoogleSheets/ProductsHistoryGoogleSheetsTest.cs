using System.Collections.Generic;
using Radiant.Common.API.Google.Sheets;
using Radiant.Custom.ProductsWatcher.ProductsHistory.GoogleSheets;
using Xunit;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Tests.GoogleSheets
{
    public class ProductsHistoryGoogleSheetsTest
    {
        [Fact]
        public void BasicTest()
        {
            ProductsHistoryGoogleSheetsManager.Authenticate();
            var _Result = ProductsHistoryGoogleSheetsManager.UpdateDataSheet(new GoogleSheetData
            {
                RowDataCollection = new List<GoogleSheetRowData>
                {
                    new()
                    {
                        RowCellData = new List<object> {"unit_test_row_1", 1 }
                    },
                    new()
                    {
                        RowCellData = new List<object> {"unit_test_row_2", 2 }
                    },
                    new()
                    {
                        RowCellData = new List<object> {"unit_test_row_3", 3 }
                    }
                }
            }, "UnitTest_AUTO_Data");

            Assert.True(_Result, "Couldn't update sheet.");

            ProductsHistoryGoogleSheetsManager.Dispose();
        }

        [Fact]
        public void ExportDatabaseInfoToRemoteSheet()
        {
            ProductsHistoryGoogleSheetsManager.Authenticate();
            var _Result = ProductsHistoryGoogleSheetsManager.UpdateDataSheetWithProductsInStorage();

            Assert.True(_Result, "Couldn't update sheet.");

            ProductsHistoryGoogleSheetsManager.Dispose();
        }
    }
}
