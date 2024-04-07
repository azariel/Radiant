using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Tasks
{
    /// <summary>
    /// Update a specific google sheet with the updated Questrade Activities
    /// </summary>
    public class UpdateRemoteGoogleSheetWithQuestradeActivitiesExcelTask : RadiantTask
    {
        private const string FILE_NAME_START_WITH = "QT-Activities_";

        protected override void TriggerNowImplementation()
        {
            var _Config = RadiantBudgetBridgeConfigurationManager.ReloadConfig();
            string[] excelFiles = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.xlsx").ToArray();
            excelFiles = excelFiles.Where(w => Path.GetFileName(w).StartsWith(FILE_NAME_START_WITH)).ToArray();

            if (!excelFiles.Any())
                return;

            foreach (var excelFile in excelFiles)
            {
                if (!File.Exists(excelFile))
                {
                    continue;
                }

                // find the configuration relating to the processing file
                ConfigByFileTag configByFileTag = _Config.QuestradeActivitiesExportData.ConfigByFileTagCollection.FirstOrDefault(w => excelFile.Contains(w.Tag, StringComparison.InvariantCultureIgnoreCase));

                if (configByFileTag == null)
                {
                    LoggingManager.LogToFile("", $"Couldn't process file [{excelFile}] as no matching configuration could be found for Questrade Activities processing.");
                    continue;
                }

                // Save model to remote google sheet
                BudgetBridgeGoogleSheetsManager.Authenticate();

                try
                {
                    BudgetBridgeGoogleSheetsManager.UpdateDataSheetWithQuestradeActivities(excelFile, configByFileTag.DataSheetId, configByFileTag.SpreadSheetFileId);
                } finally
                {
                    BudgetBridgeGoogleSheetsManager.Dispose();
                }

                try
                {
                    // Move file to backup
                    if (!string.IsNullOrWhiteSpace(_Config.QuestradeActivitiesExportData.BackupFolderPath) && !Directory.Exists(_Config.QuestradeActivitiesExportData.BackupFolderPath))
                        Directory.CreateDirectory(_Config.QuestradeActivitiesExportData.BackupFolderPath);

                    File.Copy(excelFile, Path.Combine(_Config.QuestradeActivitiesExportData.BackupFolderPath, $"bkp_{DateTime.UtcNow:yyyy.MM.dd HH.mm.ss}-{Path.GetFileName(excelFile)}"), false);
                    File.Delete(excelFile);
                } catch (Exception ex)
                {
                    LoggingManager.LogToFile("819aaa53-8d39-4181-a04d-c4ae5c0cac9e", $"Couldn't move file [{excelFile}] to backup folder.", ex);
                }
            }
        }
    }
}
