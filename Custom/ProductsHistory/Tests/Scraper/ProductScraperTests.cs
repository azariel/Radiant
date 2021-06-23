using System.Linq;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Scraper
{
    public class ProductScraperTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void TestProductFetchForSpecificUrl(string aUrl)
        {
            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, aUrl, _ProductScraper, _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList());

            Assert.NotEmpty(_ProductScraper.Screenshot);
            Assert.NotNull(_ProductScraper.Information);
            Assert.NotNull(_ProductScraper.Information.Price);
            Assert.NotNull(_ProductScraper.Information.Title);
            Assert.NotEmpty(_ProductScraper.Information.Title);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void AmazonBasicTest()
        {
            TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL);
        }

        [Fact]
        public void BestBuyBasicTest()
        {
            TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL);
        }

        [Fact]
        public void NeweggBasicTest()
        {
            TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL);
        }
    }
}
