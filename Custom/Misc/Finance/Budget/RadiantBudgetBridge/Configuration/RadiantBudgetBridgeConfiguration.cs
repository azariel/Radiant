using Radiant.Common.Emails.Mailkit;
using Radiant.Custom.Finance.Budget.RadiantBudgetBridge.GoogleSheets;

namespace Radiant.Custom.Finance.Budget.RadiantBudgetBridge.Configuration
{
    public class RadiantBudgetBridgeConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public EmailServerConfiguration ImapConfiguration { get; set; } = new();
        public GoogleTransactionsHistoryExportDataConfig GoogleSheetTransactionsHistoryExportData { get; set; } = new();
        public QuestradeActivitiesExportDataConfig QuestradeActivitiesExportData { get; set; } = new();
    }
}