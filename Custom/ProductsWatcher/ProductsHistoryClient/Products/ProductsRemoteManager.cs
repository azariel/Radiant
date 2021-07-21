using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using ProductsHistoryClient.Configuration;
using ProductsHistoryClient.Configuration.State;
using Radiant.Common.API.GoogleDrive;
using Radiant.Common.Diagnostics;

namespace ProductsHistoryClient.Products
{
    public static class ProductsRemoteManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        public const string GOOGLE_API_SERVICE_KEYS_FILE_NAME = "ProductsRemoteServiceKeys.json";

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static bool IsRemoteDatabaseMoreRecentThanLocal()
        {
            ProductsHistoryClientState _State = ProductsHistoryClientStateManager.ReloadConfig();
            ProductsHistoryClientConfiguration _Config = ProductsHistoryClientConfigurationManager.ReloadConfig();

            GoogleDriveManager _DriveManager = new GoogleDriveManager(GOOGLE_API_SERVICE_KEYS_FILE_NAME);
            File _DatabaseFile = _DriveManager.TryGetFile(_Config.GoogleDriveAPIConfig.DatabaseFileId, "modifiedTime");
            _State.RemoteDataBaseState.LastTimeEvaluated = DateTime.Now;
            ProductsHistoryClientStateManager.SaveConfigInMemoryToDisk();

            if (_DatabaseFile == null)
            {
                ClientException.ThrowNewException("0F382E1A-8AFD-4261-B3DF-954A7EA2B704", $"Couldn't check if remote database was more recent than local. Distant file of file id [{_Config.GoogleDriveAPIConfig.DatabaseFileId}] wasn't found.");
                return false;
            }

            return _State.RemoteDataBaseState.LastFetchedDataBaseModifiedDateTime < DateTime.Now;
        }

        public static byte[] FetchRemoteDatabaseContent()
        {
            ProductsHistoryClientState _State = ProductsHistoryClientStateManager.ReloadConfig();
            ProductsHistoryClientConfiguration _Config = ProductsHistoryClientConfigurationManager.ReloadConfig();

            GoogleDriveManager _DriveManager = new GoogleDriveManager(GOOGLE_API_SERVICE_KEYS_FILE_NAME);
            byte[] _DataBaseContent = _DriveManager.TryFetchDocumentContentAsByteArray(_Config.GoogleDriveAPIConfig.DatabaseFileId);

            // Update State
            File _DatabaseFile = _DriveManager.TryGetFile(_Config.GoogleDriveAPIConfig.DatabaseFileId, "modifiedTime");

            if (_DatabaseFile.ModifiedTime.HasValue)
            {
                _State.RemoteDataBaseState.LastFetchedDataBaseModifiedDateTime = _DatabaseFile.ModifiedTime.Value;
                ProductsHistoryClientStateManager.SaveConfigInMemoryToDisk();
            } else
                LoggingManager.LogToFile("8AF6F7B3-37C7-4CAE-B781-23D0AF12BFD8", "Fetched database file, but modifiedTime was null. The local State won't be updated.");

            if (_DataBaseContent == null || _DataBaseContent.Length <= 0)
                LoggingManager.LogToFile("2A988902-F065-461B-B455-E7FB7C55E43F", $"Alert. Fetched remote database, but length was [{_DataBaseContent?.Length}].");

            return _DataBaseContent;
        }
    }
}
