using System.Linq;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;
using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Scraper
{
    public class ProductScraperTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private ProductFetchedInformation TestProductFetchForSpecificUrl(string aUrl, bool aMustSucceedWithManualOperation)
        {
            // We'll take a screenshot while we're at it for possible manual reference
            ManualScraper _ManualScraper = new ManualScraper();
            ProductTargetScraper _ProductScraper = new ProductTargetScraper(ManualBaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, aUrl, _ProductScraper, _Config.ManualScraperSequenceItems.Select(s => (IScraperItemParser)s).ToList(), _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList());

            Assert.NotEmpty(_ProductScraper.Screenshot);
            Assert.NotNull(_ProductScraper.Information);
            Assert.NotNull(_ProductScraper.Information.Price);
            //Assert.NotNull(_ProductScraper.Information.Title);
            //Assert.NotEmpty(_ProductScraper.Information.Title);

            // If we needed a fallback, it means that a primary method isn't working
            if (aMustSucceedWithManualOperation)
                Assert.False(_ProductScraper.OneOrMoreStepFailedAndRequiredAFallback);

            return _ProductScraper.Information;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void AmazonBasicTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL, true);
            Assert.Equal(89.47, _Product.Price);
            Assert.Equal("PlayStation DualSense Wireless Controller – Midnight Black - Midnight Black Edition: PlayStation: Computer and Video Games - Amazon.ca", _Product.Title);
        }

        [Fact]
        public void AmazonProductAlternative2Test()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.AMAZON_ALTERNATIVE_PRODUCT_2_URL, true);
            Assert.Equal(69.99, _Product.Price);
            Assert.Equal("Fire TV Stick 4K streaming device with Alexa built in, Ultra HD, Dolby Vision, includes the Alexa Voice Remote : Amazon.ca: Amazon Devices &amp; Accessories", _Product.Title);
        }

        [Fact]
        public void AmazonProductAlternative3Test()
        {
            // List Price: <striked>$89.99</striked>
            // and just below, we have Price: $76.00
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.AMAZON_ALTERNATIVE_PRODUCT_3_URL, true);
            Assert.Equal(82.95, _Product.Price);
            Assert.Equal("DualSense Wireless Controller - DualSense Controller Edition: PlayStation 5: Video Games - Amazon.ca", _Product.Title);
        }

        [Fact]
        public void BestBuyBasicTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL, true);
            Assert.Equal(90.19, _Product.Price);
            Assert.Equal("PlayStation 5 DualSense Wireless Controller - White", _Product.Title);
        }

        [Fact]
        public void NeweggProductWithDiscountInDollarsTest()
        {
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.NEWEGG_DISCOUNT_IN_AMOUNT_PRODUCT_URL, true);
            Assert.Equal(98.99, _Product.Price - _Product.DiscountPrice);
            Assert.Equal("HUAWEI WiFi AX3 Quad Core Router with Wi-Fi 6 Plus, Speed up to 3000 Mbps, Quad-Core 1.4GHz CPU, 160 MHz frequency bandwidth, supports 1024-QAM (Canada Warranty)", _Product.Title);
        }

        [Fact]
        public void NeweggBasicTest()
        {
            // Note that for Newegg, we still don't have a manual operation way to get the price.. The "$" ctr+f find response vary too much... the DOM parser is more stable
            ProductFetchedInformation _Product = TestProductFetchForSpecificUrl(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, true);
            Assert.Equal(87.69, _Product.Price);
            Assert.Equal("PlayStation 3005739 PS5 Accessories", _Product.Title);
        }
    }
}
