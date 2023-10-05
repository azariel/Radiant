using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Radiant.Common.Serialization;

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
                string range = $"{sheetId}!A1:ZZ";
                string valueInputOption = "USER_ENTERED";

                // The new values to apply to the spreadsheet.
                List<ValueRange> _Data = new();
                var _Values = new ValueRange();

                _Values.Range = range;
                _Values.Values = googleSheetData.RowDataCollection.Select(s => s.RowCellData).ToArray();

                // add values to the data ValueRange List
                _Data.Add(_Values);

                BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
                requestBody.ValueInputOption = valueInputOption;
                //requestBody.IncludeValuesInResponse = true;
                requestBody.Data = _Data;

                var request = fSheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadSheetId);

                request.Execute();
                //IList<IList<object>> updatedValues = response.Responses[0].UpdatedData.Values;
                // Data.BatchUpdateValuesResponse response = await request.ExecuteAsync(); // For async 

                // TODO: Validate that the update was correctly executed
            }
            catch (Exception ex)
            {
                // TODO: logs
                return false;
            }
            return true;
        }
    }
}
