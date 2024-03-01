using Microsoft.EntityFrameworkCore;
using Radiant.Common.API.Google.Sheets;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration;
using Radiant.Custom.ProductsWatcher.ProductsHistory.WebApiClient;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductDefinitions;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.Products;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.ResponseModels.ProductsHistory;

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
            GoogleSheetRowData _HeaderRowData = new();
            _HeaderRowData.RowCellData.Add(null);// Empty

            ProductsResponseDto _ProductsDto = RadiantProductsHistoryWebApiClient.GetAllProducts().Result;

            foreach (var _Product in _ProductsDto.Products)
            {
                _HeaderRowData.RowCellData.Add(_Product.Name);// Name
            }

            _GoogleSheetData.RowDataCollection.Add(_HeaderRowData);

            var _Now = DateTime.UtcNow;
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

            foreach (var _Product in _ProductsDto.Products)
            {
                // Get product definition
                var _ProductDefinitionResponseDto = RadiantProductsHistoryWebApiClient.GetProductDefinition(_Product.ProductId, true).Result;

                // Add prices by date day by day
                IGrouping<DateTime, ProductHistoryResponseDto>[] _DefinitionHistoryGroupedByDay = _ProductDefinitionResponseDto.ProductDefinitions.SelectMany(sm => sm.ProductHistoryCollection.ProductHistory).GroupBy(g => g.InsertDateTime.Date).ToArray();

                // Keep x last years
                for (int i = 0; i < _Config.GoogleSheetProductsExportData.NbDaysToExport; i++)
                {
                    DateTime _DateToFind = _Now.Date.AddDays(-i);
                    IGrouping<DateTime, ProductHistoryResponseDto> _HistoryModelsForSpecificDay = _DefinitionHistoryGroupedByDay.FirstOrDefault(w => w.Key == _DateToFind);

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
