namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets;

public class QuestradeActivitiesExportDataConfig
{
    public string BackupFolderPath { get; set; } = "BackupsQuestradeActivitiesDataHistory";
    public ConfigByFileTag[] ConfigByFileTagCollection { get; set; }
}