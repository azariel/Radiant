using System;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistory.Resources;
using Radiant.Custom.ProductsHistory.Scraper;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Parsers
{
    public class DOMInformationParserTests
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        private enum ProductSourceCodeFile
        {
            AmazonTypicalProduct,
            BestBuyTypicalProduct,
            NeweggTypicalProduct,
            AmazonProduct2,
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private string GetDOMFromResource(ProductSourceCodeFile aProductSourceCodeFile)
        {
            switch (aProductSourceCodeFile)
            {
                case ProductSourceCodeFile.AmazonTypicalProduct:
                    return UnitTestsResources.ResourceManager.GetObject("Amazon_Product1_SourceCode") as string;
                case ProductSourceCodeFile.BestBuyTypicalProduct:
                    return UnitTestsResources.ResourceManager.GetObject("BestBuy_Product1_SourceCode") as string;
                case ProductSourceCodeFile.NeweggTypicalProduct:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product1_SourceCode") as string;
                case ProductSourceCodeFile.AmazonProduct2:
                    return UnitTestsResources.ResourceManager.GetObject("Amazon_Product2_SourceCode") as string;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aProductSourceCodeFile), aProductSourceCodeFile, null);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestParseTypicalAmazonPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonTypicalProduct);

            double? _Price = DOMProductInformationParser.ParsePrice(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            double _ExpectedPrice = 89.96;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTypicalAmazonTitle()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonTypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "PlayStation DualSense Wireless Controller – Midnight Black - Midnight Black Edition: PlayStation: Computer and Video Games - Amazon.ca";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestParseTypicalNeweggPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggTypicalProduct);

            double? _Price = DOMProductInformationParser.ParsePrice(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            double _ExpectedPrice = 52.99;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTypicalNeweggTitle()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggTypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "DualShock 4 PS4 Controller Wireless for PlayStation 4 - Jet Black - Newegg.ca";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestParseTypicalBestBuyPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            string _DOM = GetDOMFromResource(ProductSourceCodeFile.BestBuyTypicalProduct);

            double? _Price = DOMProductInformationParser.ParsePrice(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            double _ExpectedPrice = 90.19;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTypicalBestBuyTitle()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.BestBuyTypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "PlayStation 5 DualSense Wireless Controller - White | Best Buy Canada";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestAmazonBulkProducts()
        {
            // Product 2
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _Product2DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonProduct2);

            double? _Price = DOMProductInformationParser.ParsePrice("www.amazon.ca", _Product2DOM, _Config.DOMParserItems);
            double _ExpectedPrice = 54.99;
            Assert.Equal(_ExpectedPrice, _Price);

            string _Title = DOMProductInformationParser.ParseTitle("www.amazon.ca", _Product2DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "Fire TV Stick 4K streaming device with Alexa built in, Ultra HD, Dolby Vision, includes the Alexa Voice Remote : Amazon.ca: Amazon Devices &amp; Accessories";
            Assert.Equal(_ExpectedTitle, _Title);
        }
    }
}
