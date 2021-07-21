using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProductsHistoryClient.Configuration.State;
using ProductsHistoryClient.Products.Database;
using Radiant.Common.Diagnostics;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace ProductsHistoryClient.Products
{
    public static class ProductsManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        public const string DATABASE_FILE_NAME = "RadiantCommon.db";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        /// <summary>
        /// Fetch remote database and replace local one with that remote one
        /// </summary>
        /// <returns></returns>
        private static async Task TryRefreshLocalDataBaseWithRemoteDataBase()
        {
            try
            {
                byte[] _DatabaseContent = ProductsRemoteManager.FetchRemoteDatabaseContent();

                if (File.Exists(DATABASE_FILE_NAME))
                    File.Delete(DATABASE_FILE_NAME);

                File.WriteAllBytes(DATABASE_FILE_NAME, _DatabaseContent);
                LoggingManager.LogToFile("0872B1E6-B9AE-4EDC-9DD8-AF5B4DCFAAA5", "Local Database was successfully updated.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("0872B1E6-B9AE-4EDC-9DD8-AF5B4DCFAAA5", "Tried to update local Database with remote Database, but failed.", _Exception);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static async Task<List<RadiantProductModel>> LoadProductsFromDisk()
        {
            if (!File.Exists(DATABASE_FILE_NAME))
            {
                ClientException.ThrowNewException("0284AA63-09BB-4838-A073-6D9056D005FC", $"Database was fetched, but wasn't found on disk [{DATABASE_FILE_NAME}].");
                return null;
            }

            return StorageManager.LoadProducts(true);
        }

        public static async Task<List<RadiantProductModel>> LoadProductsFromRemote()
        {
            ProductsHistoryClientState _State = ProductsHistoryClientStateManager.ReloadConfig();

            // To avoid spamming the API, execute an update check only once every X hours
            if ((DateTime.Now - _State.RemoteDataBaseState.LastTimeEvaluated).TotalHours > 1)
            {
                if (ProductsRemoteManager.IsRemoteDatabaseMoreRecentThanLocal())
                {
                    LoggingManager.LogToFile("9DA3D4EA-4B97-4E80-9EE3-CF713721B574", "Remote Database was updated since the last time it was fetched.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                    // Update local database
                    await TryRefreshLocalDataBaseWithRemoteDataBase();
                } else
                {
                    LoggingManager.LogToFile("DF1B12CB-97D4-47BA-B9DC-737617CFB2F0", "Remote Database wasn't updated since the last time it was fetched.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                }
            }

            // If the local database was deleted, we'll fetch the remote one
            if (!File.Exists(DATABASE_FILE_NAME))
                await TryRefreshLocalDataBaseWithRemoteDataBase();

            return await LoadProductsFromDisk();
        }
    }
}
