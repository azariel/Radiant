using Radiant.Common.API.Google.Sheets;
using Radiant.Common.Diagnostics;
using Radiant.Common.Utils;
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

        internal static void Dispose()
        {
            fGoogleSheetsApi?.Dispose();
            fGoogleSheetsApi = null;
        }

        internal static bool UpdateDataSheet(GoogleSheetData googleSheetData, string dataSheetId, string spreadSheetFileId)
        {
            return fGoogleSheetsApi.UpdateSheet(dataSheetId, spreadSheetFileId, googleSheetData);
        }

        internal static bool UpdateDataSheetWithTransactionsHistory(string aCsvContainingDataHistoryFilePath, string dataSheetId, string spreadSheetFileId)
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

                return new MyBudgetBookCsvEntryModel
                {
                    Date = DateTimeUtils.TryConvertToDateFormat(splitLine[0], "M-d-yyyy"),
                    CreatedAt = DateTimeUtils.TryConvertToDateFormat(splitLine[1]),
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
            IOrderedEnumerable<MyBudgetBookCsvEntryModel> _OrderedDictionary = csvAsDictionary.OrderByDescending(o =>
            {
                if (!DateTime.TryParse(o.Date, out DateTime _DateTimeResult))
                    _DateTimeResult = default;

                return _DateTimeResult;
            }).ThenByDescending(o =>
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
            _FirstRowData.RowCellData.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
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

            foreach (MyBudgetBookCsvEntryModel csvLine in _OrderedDictionary)
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

        internal static bool UpdateDataSheetWithQuestradeActivities(string excelFilePath, string dataSheetId, string spreadSheetFileId)
        {
            // Convert excel to csv
            string csvFilePath = CsvUtils.ConvertExcelFileToCsv(excelFilePath);

            //2024-04-04 12:00:00 AM,2024-04-04 12:00:00 AM,EFT,,TRANSFERT,0.00000,0.00000000,0.00,0.00,-21700.00,CAD,28862932,Withdrawals,Individual margin
            var csvAsDictionary = CsvUtils.LoadFile(csvFilePath, strLineInput =>
            {
                var splitLine = strLineInput.Replace("\"", string.Empty).Split(";");
                if (splitLine.Length != 14)
                {
                    var message = $"Invalid nb of columns in CSV file [{csvFilePath}]. Expecting [10] columns but found [{splitLine.Length}].";
                    LoggingManager.LogToFile("3d112221-ea15-4ff0-916b-8a9c4702e2d8", message);
                    throw new Exception(message);
                }

                return new QuestradeActivitiesCsvEntryModel
                {
                    TransactionDate = DateTimeUtils.TryConvertToDateFormat(splitLine[0], "M-d-yyyy"),
                    SettlementDate = DateTimeUtils.TryConvertToDateFormat(splitLine[1], "M-d-yyyy"),
                    Action = splitLine[2],
                    Symbol = splitLine[3],
                    Description = splitLine[4],
                    Quantity = splitLine[5],
                    Price = splitLine[6],
                    GrossAmount = splitLine[7],
                    Commission = splitLine[8],
                    NetAmount = splitLine[9],
                    Currency = splitLine[10],
                    AccountNumber = splitLine[11],
                    ActivityType = splitLine[12],
                    AccountType = splitLine[13],
                };
            }, 1);

            // Order dict
            IOrderedEnumerable<QuestradeActivitiesCsvEntryModel> _OrderedDictionary = csvAsDictionary.OrderByDescending(o =>
            {
                if (!DateTime.TryParse(o.TransactionDate, out DateTime _DateTimeResult))
                    _DateTimeResult = default;

                return _DateTimeResult;
            }).ThenByDescending(o => o.AccountType);

            // Build payload
            GoogleSheetData _GoogleSheetData = new();

            // Very first line is reserved for the last update date
            GoogleSheetRowData _FirstRowData = new();
            _FirstRowData.RowCellData.Add("Last Update");
            _FirstRowData.RowCellData.Add($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            _GoogleSheetData.RowDataCollection.Add(_FirstRowData);

            // Then second line is headers
            GoogleSheetRowData _HeaderRowData = new();
            _HeaderRowData.RowCellData.Add("Transaction Date");
            _HeaderRowData.RowCellData.Add("Settlement Date");
            _HeaderRowData.RowCellData.Add("Action");
            _HeaderRowData.RowCellData.Add("Symbol");
            _HeaderRowData.RowCellData.Add("Description");
            _HeaderRowData.RowCellData.Add("Quantity");
            _HeaderRowData.RowCellData.Add("Price");
            _HeaderRowData.RowCellData.Add("Gross Amount");
            _HeaderRowData.RowCellData.Add("Commission");
            _HeaderRowData.RowCellData.Add("Net Amount");
            _HeaderRowData.RowCellData.Add("Currency");
            _HeaderRowData.RowCellData.Add("Account Number");
            _HeaderRowData.RowCellData.Add("Activity Type");
            _HeaderRowData.RowCellData.Add("Account Type");

            _GoogleSheetData.RowDataCollection.Add(_HeaderRowData);

            foreach (QuestradeActivitiesCsvEntryModel csvLine in _OrderedDictionary)
            {
                GoogleSheetRowData _RowData = new();

                _RowData.RowCellData.Add(csvLine.TransactionDate);
                _RowData.RowCellData.Add(csvLine.SettlementDate);
                _RowData.RowCellData.Add(csvLine.Action);
                _RowData.RowCellData.Add(csvLine.Symbol);
                _RowData.RowCellData.Add(csvLine.Description);
                _RowData.RowCellData.Add(csvLine.Quantity);
                _RowData.RowCellData.Add(csvLine.Price);
                _RowData.RowCellData.Add(csvLine.GrossAmount);
                _RowData.RowCellData.Add(csvLine.Commission);
                _RowData.RowCellData.Add(csvLine.NetAmount);
                _RowData.RowCellData.Add(csvLine.Currency);
                _RowData.RowCellData.Add(csvLine.AccountNumber);
                _RowData.RowCellData.Add(csvLine.ActivityType);
                _RowData.RowCellData.Add(csvLine.AccountType);

                _GoogleSheetData.RowDataCollection.Add(_RowData);
            }

            return fGoogleSheetsApi.UpdateSheet(dataSheetId, spreadSheetFileId, _GoogleSheetData);
        }
    }
}
