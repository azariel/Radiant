using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.API.Google.Sheets;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.GoogleSheets
{
    public static class ProductsHistoryGoogleSheetsManager
    {
        private const string DATA_SPREAD_SHEET_ID = "13TnLPl-iUP-7FbvJXaLBPwhd3-F92Uzo-dtLcr-icBM"; // TODO: config
        private const string DATA_SHEET_ID = "AUTO_Data";
        private const int INT_DAYS_TO_EXPORT = 1825; // 5 years
        private static RadiantGoogleSheetsApi fGoogleSheetsApi;

        public static void Authenticate()
        {
            if (fGoogleSheetsApi != null)
                return;

            fGoogleSheetsApi = new RadiantGoogleSheetsApi(null);
            fGoogleSheetsApi.Authenticate();
        }

        public static void Dispose()
        {
            fGoogleSheetsApi?.Dispose();
            fGoogleSheetsApi = null;
        }

        public static bool UpdateDataSheet(GoogleSheetData googleSheetData, string dataSheetId = null)
        {
            return fGoogleSheetsApi.UpdateSheet(dataSheetId, DATA_SPREAD_SHEET_ID, googleSheetData);
        }

        public static bool UpdateDataSheetWithProductsInStorage(string dataSheetId = null)
        {
            // Build payload
            GoogleSheetData _GoogleSheetData = new();

            using var _DataBaseContext = new ServerProductsDbContext();
            _DataBaseContext.Products.Load();
            _DataBaseContext.ProductDefinitions.Load();
            _DataBaseContext.ProductsHistory.Load();

            GoogleSheetRowData _HeaderRowData = new();
            _HeaderRowData.RowCellData.Add(null);// Empty

            foreach (RadiantServerProductModel _Product in _DataBaseContext.Products)
            {
                _HeaderRowData.RowCellData.Add(_Product.Name);// Name
            }

            _GoogleSheetData.RowDataCollection.Add(_HeaderRowData);

            var _Now = DateTime.Now;
            List<GoogleSheetRowData> _RowsData = new(INT_DAYS_TO_EXPORT);
            for (int i = 0; i < INT_DAYS_TO_EXPORT; i++)
            {
                _RowsData.Add(new GoogleSheetRowData
                {
                    // First column is the price's date
                    RowCellData = new List<object>
                    {
                        $"{_Now.AddDays(-i):yyyy-MM-dd}"
                    }
                });
            }

            foreach (RadiantServerProductModel _Product in _DataBaseContext.Products)
            {
                // Add prices by date day by day
                IGrouping<DateTime, RadiantServerProductHistoryModel>[] _DefinitionHistoryGroupedByDay = _Product.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection).GroupBy(g => g.InsertDateTime.Date).ToArray();


                // Keep 10 last years
                for (int i = 0; i < INT_DAYS_TO_EXPORT; i++)
                {
                    DateTime _DateToFind = _Now.Date.AddDays(-i);
                    IGrouping<DateTime, RadiantServerProductHistoryModel> _HistoryModelsForSpecificDay = _DefinitionHistoryGroupedByDay.FirstOrDefault(w => w.Key == _DateToFind);

                    if (_HistoryModelsForSpecificDay == null)
                    {
                        _RowsData[i].RowCellData.Add(null);// no data for that specific day
                        continue;
                    }

                    var _BestPriceForThatDay = _HistoryModelsForSpecificDay.MinBy(m => m.Price);
                    _RowsData[i].RowCellData.Add(_BestPriceForThatDay.Price);// Best price for that day
                }
            }

            _GoogleSheetData.RowDataCollection.AddRange(_RowsData);

            return fGoogleSheetsApi.UpdateSheet(dataSheetId ?? DATA_SHEET_ID, DATA_SPREAD_SHEET_ID, _GoogleSheetData);
        }
    }
}
