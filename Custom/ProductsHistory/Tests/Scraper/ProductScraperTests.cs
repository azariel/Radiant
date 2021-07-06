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
        private ProductFetchedInformation TestProductFetchForSpecificUrl(string aUrl)
        {
            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, aUrl, _ProductScraper, _Config.ManualScraperSequenceItems.Select(s => (ManualScraperItemParser)s).ToList(), _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList());

            Assert.NotEmpty(_ProductScraper.Screenshot);
            Assert.NotNull(_ProductScraper.Information);
            Assert.NotNull(_ProductScraper.Information.Price);
            Assert.NotNull(_ProductScraper.Information.Title);
            Assert.NotEmpty(_ProductScraper.Information.Title);

            // If we needed a fallback, it means that a primary method isn't working
            Assert.False(_ProductScraper.OneOrMoreStepFailedAndRequiredAFallback);

            return _ProductScraper.Information;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void AmazonBasicTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL);
            Assert.Equal(89.96, _Product.Price);
            Assert.Equal("PlayStation DualSense Wireless Controller – Midnight Black - Midnight Black Edition: PlayStation: Video Games - Amazon.ca", _Product.Title);
        }

        [Fact]
        public void BestBuyBasicTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL);
            Assert.Equal(90.19, _Product.Price);
            Assert.Equal("PlayStation 5 DualSense Wireless Controller - White | Best Buy Canada", _Product.Title);
        }

        [Fact]
        public void NeweggBasicTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL);
            Assert.Equal(52.99, _Product.Price);
            Assert.Equal("DualShock 4 PS4 Controller Wireless for PlayStation 4 - Jet Black - Newegg.ca", _Product.Title);
        }
    }
}
