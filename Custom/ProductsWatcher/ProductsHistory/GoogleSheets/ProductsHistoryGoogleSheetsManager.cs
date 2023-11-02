using Microsoft.EntityFrameworkCore;
using Radiant.Common.API.Google.Sheets;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.GoogleSheets
{
    public static class ProductsHistoryGoogleSheetsManager
    {
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
            var _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            return fGoogleSheetsApi.UpdateSheet(dataSheetId, _Config.GoogleSheetAPIConfig.SpreadSheetFileId, googleSheetData);
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
            var _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            List<GoogleSheetRowData> _RowsData = new(_Config.GoogleSheetProductsExportData.NbDaysToExport);
            for (int i = 0; i < _Config.GoogleSheetProductsExportData.NbDaysToExport; i++)
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

                // Keep x last years
                for (int i = 0; i < _Config.GoogleSheetProductsExportData.NbDaysToExport; i++)
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

            return fGoogleSheetsApi.UpdateSheet(dataSheetId ?? _Config.GoogleSheetProductsExportData.DataSheetTabId, _Config.GoogleSheetAPIConfig.SpreadSheetFileId, _GoogleSheetData);
        }
    }
}
