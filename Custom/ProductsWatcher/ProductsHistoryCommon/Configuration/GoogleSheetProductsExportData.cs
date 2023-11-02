namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.Configuration
{
    public class GoogleSheetProductsExportData
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DataSheetTabId { get; set; } = "AUTO_DATA";
        public int NbDaysToExport { get; set; } = 0;
    }
}
