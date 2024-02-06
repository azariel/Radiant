using Radiant.Common.API.Google.Sheets;
using Radiant.Common.Diagnostics;
using Radiant.Common.Utils;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Models;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets
{
    internal static class BudgetBridgeGoogleSheetsManager
    {
        private static RadiantGoogleSheetsApi fGoogleSheetsApi;

        internal static void Authenticate()
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

        public static bool UpdateDataSheet(GoogleSheetData googleSheetData, string dataSheetId, string spreadSheetFileId)
        {
            return fGoogleSheetsApi.UpdateSheet(dataSheetId, spreadSheetFileId, googleSheetData);
        }

        public static bool UpdateDataSheetWithTransactionsHistory(string aCsvContainingDataHistoryFilePath, string dataSheetId, string spreadSheetFileId)
        {
            string filePath = aCsvContainingDataHistoryFilePath;
            var csvAsDictionary = CsvUtils.LoadFile(filePath, strLineInput =>
            {
                //strLineInput ex: "1/28/2024";"1/29/2023 10:21:55 AM";"Internet";"";"Transfer";"Utilities";"0_Internet";"B - Yan cashflow";"-50.13";"No"
                // HEADERS = "Date";"Created at";"Title";"Comment";"Payment type";"Main category";"Subcategory";"Account";"Amount";"Cleared"
                var splitLine = strLineInput.Replace("\"", string.Empty).Split(";");
                if (splitLine.Length != 10)
                {
                    var message = $"Invalid nb of columns in CSV file [{filePath}]. Expecting [10] columns but found [{splitLine.Length}].";
                    LoggingManager.LogToFile("ed639ec3-d218-47fe-9ce2-726a20c8cda2", message);
                    throw new Exception(message);
                }

                return new CsvEntryModel
                {
                    Date = splitLine[0],
                    CreatedAt = splitLine[1],
                    Title = splitLine[2],
                    Comment = splitLine[3],
                    PaymentType = splitLine[4],
                    MainCategory = splitLine[5],
                    SubCategory = splitLine[6],
                    Account = splitLine[7],
                    Amount = splitLine[8],
                    Cleared = splitLine[9],
                };
            }, 1);

            // Order dict
            IOrderedEnumerable<CsvEntryModel> _OrderedDictionary = csvAsDictionary.OrderByDescending(o =>
            {
                if (!DateTime.TryParse(o.CreatedAt, out DateTime _DateTimeResult))
                    _DateTimeResult = default;

                return _DateTimeResult;
            });

            // Build payload
            GoogleSheetData _GoogleSheetData = new();

            // Very first line is reserved for the last update date
            GoogleSheetRowData _FirstRowData = new();
            _FirstRowData.RowCellData.Add("Last Update");
            _FirstRowData.RowCellData.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _GoogleSheetData.RowDataCollection.Add(_FirstRowData);

            // Then second line is headers
            GoogleSheetRowData _HeaderRowData = new();
            _HeaderRowData.RowCellData.Add("CreatedAt");
            _HeaderRowData.RowCellData.Add("Date");
            _HeaderRowData.RowCellData.Add("Account");
            _HeaderRowData.RowCellData.Add("MainCategory");
            _HeaderRowData.RowCellData.Add("SubCategory");
            _HeaderRowData.RowCellData.Add("PaymentType");
            _HeaderRowData.RowCellData.Add("Amount");
            _HeaderRowData.RowCellData.Add("Cleared");
            _HeaderRowData.RowCellData.Add("Comment");

            _GoogleSheetData.RowDataCollection.Add(_HeaderRowData);

            foreach (CsvEntryModel csvLine in _OrderedDictionary)
            {
                GoogleSheetRowData _RowData = new();

                _RowData.RowCellData.Add(csvLine.CreatedAt);
                _RowData.RowCellData.Add(csvLine.Date);
                _RowData.RowCellData.Add(csvLine.Account);
                _RowData.RowCellData.Add(csvLine.MainCategory);
                _RowData.RowCellData.Add(csvLine.SubCategory);
                _RowData.RowCellData.Add(csvLine.PaymentType);
                _RowData.RowCellData.Add(csvLine.Amount);
                _RowData.RowCellData.Add(csvLine.Cleared);
                _RowData.RowCellData.Add(csvLine.Comment);

                _GoogleSheetData.RowDataCollection.Add(_RowData);
            }

            return fGoogleSheetsApi.UpdateSheet(dataSheetId, spreadSheetFileId, _GoogleSheetData);
        }
    }
}
