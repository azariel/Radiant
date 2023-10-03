using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.Configuration;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient.Configuration
{
    public class ProductsHistoryClientConfiguration 
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public GoogleDriveAPIConfig GoogleDriveAPIConfig { get; set; } = new();
    }
}
