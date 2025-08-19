using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Tasks
{
    /// <summary>
    /// Update a specific google sheet with the updated MyBudgetBook data history
    /// </summary>
    public class UpdateRemoteGoogleSheetWithMyBudgetBookDataHistoryTask : RadiantTask
    {
        private const string FILE_NAME_START_WITH = "MBB-HistoryTransactions";

        protected override void TriggerNowImplementation()
        {
            var _Config = RadiantBudgetBridgeConfigurationManager.ReloadConfig();
            string[] csvFiles = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.csv").ToArray();
            csvFiles = csvFiles.Where(w => Path.GetFileName(w).StartsWith(FILE_NAME_START_WITH)).ToArray();

            if (!csvFiles.Any())
                return;

            foreach (var csvFile in csvFiles)
            {
                if (!File.Exists(csvFile))
                {
                    continue;
                }

                // find the configuration relating to the processing file
                ConfigByFileTag configByFileTag = _Config.GoogleSheetTransactionsHistoryExportData.ConfigByFileTagCollection.FirstOrDefault(w => csvFile.Contains(w.Tag, StringComparison.InvariantCultureIgnoreCase));

                if (configByFileTag == null)
                {
                    LoggingManager.LogToFile("", $"Couldn't process file [{csvFile}] as no matching configuration could be found for My Budget Book processing.");
                    continue;
                }

                // Save model to remote google sheet
                BudgetBridgeGoogleSheetsManager.Authenticate();

                try
                {
                    BudgetBridgeGoogleSheetsManager.UpdateDataSheetWithTransactionsHistory(csvFile, configByFileTag.DataSheetId, configByFileTag.SpreadSheetFileId);
                }catch (Exception e)
                {
                    LoggingManager.LogToFile("/*3de567da-d946-48ca-8280-bb3d5e51b6da*/", $"Couldn't update remote data sheet.", e);
                    return;
                }
                finally
                {
                    BudgetBridgeGoogleSheetsManager.Dispose();
                }

                try
                {
                    // Move file to backup
                    if (!string.IsNullOrWhiteSpace(_Config.GoogleSheetTransactionsHistoryExportData.BackupFolderPath) && !Directory.Exists(_Config.GoogleSheetTransactionsHistoryExportData.BackupFolderPath))
                        Directory.CreateDirectory(_Config.GoogleSheetTransactionsHistoryExportData.BackupFolderPath);

                    File.Copy(csvFile, Path.Combine(_Config.GoogleSheetTransactionsHistoryExportData.BackupFolderPath, $"bkp_{DateTime.UtcNow:yyyy.MM.dd HH.mm.ss}-{Path.GetFileName(csvFile)}"), false);
                    File.Delete(csvFile);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogToFile("f3ebb529-12e1-4f4a-883b-8a90c5bfd28e", $"Couldn't move file [{csvFile}] to backup folder.", ex);
                }
            }
        }
    }
}
