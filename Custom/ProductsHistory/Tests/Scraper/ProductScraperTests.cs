using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Scraper
{
    public class ProductScraperTests
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string AMAZON_DUMMY_PRODUCT_URL = "https://www.amazon.ca/PlayStation-DualSense-Wireless-Controller-Midnight/dp/B0951JZDWT";// Ok so this will definitely change...
        //https://www.amazon.ca/dp/B08C1TR9X6

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void AmazonBasicTest()
        {
            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, AMAZON_DUMMY_PRODUCT_URL, _ProductScraper);

            //Assert.NotEqual("", _ProductScraper.Information);
            Assert.NotEmpty(_ProductScraper.Screenshot);
        }
    }
}
