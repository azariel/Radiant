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
            string _DatabaseFileName = new ProductsDbContext().GetDataBaseFileName();

            if (!File.Exists(_DatabaseFileName))
            {
                LoggingManager.LogToFile("C8B77ADD-C96E-4423-8485-28E11A62E5DC", $"Database [{_DatabaseFileName}] didn't exists. Can't update remote database with missing local database. Aborting.");

                return;
            }

            byte[] _DatabaseContent = File.ReadAllBytes(_DatabaseFileName);

            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            GoogleDriveManager _DriveManager = new GoogleDriveManager("ProductsRemoteServiceKeys.json");

            _DriveManager.TryUpdateFileContent(_Config.GoogleDriveAPIConfig.DatabaseFileId, _DatabaseContent);
        }
    }
}
