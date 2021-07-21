using Radiant.Custom.ProductsHistoryCommon.Configuration;

namespace ProductsHistoryClient.Configuration
{
    public class ProductsHistoryClientConfiguration 
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public GoogleDriveAPIConfig GoogleDriveAPIConfig { get; set; } = new();
    }
}
