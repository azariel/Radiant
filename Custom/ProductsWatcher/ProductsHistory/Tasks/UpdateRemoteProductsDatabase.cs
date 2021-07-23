using System.IO;
using Radiant.Common.API.GoogleDrive;
using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsHistory.Tasks
{
    public class UpdateRemoteProductsDatabase : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            string _ServerDatabaseFileName = new ServerProductsDbContext().GetDataBaseFileName();

            if (!File.Exists(_ServerDatabaseFileName))
            {
                LoggingManager.LogToFile("C8B77ADD-C96E-4423-8485-28E11A62E5DC", $"Server Database [{_ServerDatabaseFileName}] didn't exists. Can't update remote database with missing local database. Aborting.");

                return;
            }

            ProductsHistoryStorageMigrationManager.OverrideLocalClientDatabaseWithServerDatabase();

            string _ClientDatabaseFileName = new ClientProductsDbContext().GetDataBaseFileName();

            if (!File.Exists(_ClientDatabaseFileName))
            {
                LoggingManager.LogToFile("BD9172EF-790D-4415-8D5E-DA5B71B48195", $"Client Database [{_ClientDatabaseFileName}] didn't exists. Can't update remote database with missing local database. Aborting.");

                return;
            }

            byte[] _DatabaseContent = File.ReadAllBytes(_ClientDatabaseFileName);

            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            GoogleDriveManager _DriveManager = new GoogleDriveManager("ProductsRemoteServiceKeys.json");

            _DriveManager.TryUpdateFileContent(_Config.GoogleDriveAPIConfig.DatabaseFileId, _DatabaseContent);
        }
    }
}
