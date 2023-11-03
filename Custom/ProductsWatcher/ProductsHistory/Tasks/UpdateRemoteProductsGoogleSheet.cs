using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsWatcher.ProductsHistory.GoogleSheets;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Tasks
{
    /// <summary>
    /// Task to update a remote google sheet with the prices of the monitored products over time
    /// </summary>
    public class UpdateRemoteProductsGoogleSheet : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            ProductsHistoryGoogleSheetsManager.Authenticate();

            try
            {
                ProductsHistoryGoogleSheetsManager.UpdateDataSheetWithProductsInStorage();
            }
            finally
            {
                ProductsHistoryGoogleSheetsManager.Dispose();
            }
        }
    }
}
