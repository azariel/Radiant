namespace Radiant.Custom.ProductsHistory.Scraper
{
    /// <summary>
    /// Information about a fetched product.
    /// </summary>
    public class ProductFetchedInformation
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double DiscountPercentage { get; set; } = 0;
        public double DiscountPrice { get; set; } = 0;
        public double? Price { get; set; }
        public double? ShippingCost { get; set; }
        public string Title { get; set; }
    }
}
