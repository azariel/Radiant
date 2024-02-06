using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Radiant.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radiant.Common.API.Google.Sheets
{
    public class RadiantGoogleSheetsApi : RadiantGoogleApi, IDisposable
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private SheetsService fSheetsService;

        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public RadiantGoogleSheetsApi(string jsonServiceKeyFilePath) : base(jsonServiceKeyFilePath, Scopes) { }

        public void Authenticate()
        {
            GoogleCredential _Credential = GetCredentials();

            // Create Google Sheets API service.
            fSheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _Credential,
                ApplicationName = APPLICATION_NAME
            });
        }

        public override void Dispose()
        {
            base.Dispose();

            fSheetsService?.Dispose();
            fSheetsService = null;
        }

        public bool UpdateSheet(string sheetId, string spreadSheetId, GoogleSheetData googleSheetData)
        {
            if (fSheetsService == null)
                return false;

            try
            {
                string _Range = $"{sheetId}!A1:ZZ";
                const string valueInputOption = "USER_ENTERED";

                // The new values to apply to the spreadsheet.
                List<ValueRange> _Data = new();
                var _Values = new ValueRange();

                _Values.Range = _Range;
                _Values.Values = googleSheetData.RowDataCollection.Select(s => s.RowCellData).ToArray();

                // add values to the data ValueRange List
                _Data.Add(_Values);

                BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
                requestBody.ValueInputOption = valueInputOption;
                //requestBody.IncludeValuesInResponse = true;
                requestBody.Data = _Data;

                var _Request = fSheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadSheetId);

                _Request.Execute();
                //IList<IList<object>> updatedValues = response.Responses[0].UpdatedData.Values;
                // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

                // TODO: Validate that the update was correctly executed
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("72a168d4-d80d-4e63-a85f-1caed24d85ff", $"Couldn't update remote google sheet (tab) [{sheetId}] of spreadSheetId [{spreadSheetId}].", _Ex);
                return false;
            }
            return true;
        }
    }
}
