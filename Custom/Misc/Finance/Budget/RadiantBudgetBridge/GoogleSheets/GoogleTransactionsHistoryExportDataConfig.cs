namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets;

public class GoogleTransactionsHistoryExportDataConfig
{
    public string BackupFolderPath { get; set; } = "BackupsBudgetBridgeDataHistory";
    public ConfigByFileTag[] ConfigByFileTagCollection { get; set; }
}