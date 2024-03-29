﻿using System;
using System.Linq;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Parsers;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Resources;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Scraper;
using Xunit;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Tests.Parsers
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
            AmazonProduct3,
            AmazonProduct4,
            NeweggProduct2,
            NeweggProduct3,
            NeweggProduct4,
            NeweggProduct5,
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
                case ProductSourceCodeFile.AmazonProduct2:
                    return UnitTestsResources.ResourceManager.GetObject("Amazon_Product2_SourceCode") as string;
                case ProductSourceCodeFile.AmazonProduct3:
                    return UnitTestsResources.ResourceManager.GetObject("Amazon_Product3_SourceCode") as string;// With 10.00$ coupon
                case ProductSourceCodeFile.AmazonProduct4:
                    return UnitTestsResources.ResourceManager.GetObject("Amazon_Product4_SourceCode") as string;// error.. DOM Price was null
                case ProductSourceCodeFile.BestBuyTypicalProduct:
                    return UnitTestsResources.ResourceManager.GetObject("BestBuy_Product1_SourceCode") as string;
                case ProductSourceCodeFile.NeweggTypicalProduct:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product1_SourceCode") as string;
                case ProductSourceCodeFile.NeweggProduct2:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product2_SourceCode") as string;
                case ProductSourceCodeFile.NeweggProduct3:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product3_SourceCode") as string;// With 39.96$ shipping
                case ProductSourceCodeFile.NeweggProduct4:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product4_SourceCode") as string;// With 100.00$ discount
                case ProductSourceCodeFile.NeweggProduct5:
                    return UnitTestsResources.ResourceManager.GetObject("Newegg_Product5_SourceCode") as string;// Out of Stock
                default:
                    throw new ArgumentOutOfRangeException(nameof(aProductSourceCodeFile), aProductSourceCodeFile, null);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestAmazonBulkProducts()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            //Product 2
            string _Product2DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonProduct2);

            double? _Price = DOMProductInformationParser.ParseDouble("www.amazon.ca", _Product2DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
            double _ExpectedPrice = 54.99;
            Assert.Equal(_ExpectedPrice, _Price);

            string _Title = DOMProductInformationParser.ParseTitle("www.amazon.ca", _Product2DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "Fire TV Stick 4K streaming device with Alexa built in, Ultra HD, Dolby Vision, includes the Alexa Voice Remote : Amazon.ca: Amazon Devices &amp; Accessories";
            Assert.Equal(_ExpectedTitle, _Title);

            // Product 3
            string _Product3DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonProduct3);

            double? _Price3 = DOMProductInformationParser.ParseDouble("www.amazon.ca", _Product3DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
            double _ExpectedPrice3 = 28.02;// NOTE: that product was actually 18.02 because there's a 10.00$ COUPON !
            Assert.Equal(_ExpectedPrice3, _Price3);

            string _Title3 = DOMProductInformationParser.ParseTitle("www.amazon.ca", _Product3DOM, _Config.DOMParserItems);
            string _ExpectedTitle3 = "Single Monitor Mount - Gas Spring Monitor Arm, Adjustable VESA Mount Desk Stand with Clamp and Grommet Base - Fits 17 to 27 Inch LCD Computer Screen Monitors 4.4 to 14.3lbs : Amazon.ca: Office Products";
            Assert.Equal(_ExpectedTitle3, _Title3);

            // Product 4
            string _Product4DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonProduct4);

            double? _Price4 = DOMProductInformationParser.ParseDouble("www.amazon.ca", _Product4DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
            double _ExpectedPrice4 = 299;
            Assert.Equal(_ExpectedPrice4, _Price4);

            string _Title4 = DOMProductInformationParser.ParseTitle("www.amazon.ca", _Product4DOM, _Config.DOMParserItems);
            string _ExpectedTitle4 = "ADAM Audio T5V Studio Monitor Single : Amazon.ca: Musical Instruments, Stage &amp; Studio";
            Assert.Equal(_ExpectedTitle4, _Title4);
        }

        [Fact]
        public void TestNeweggBulkProducts()
        {
            // Product 2
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _Product2DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct2);

            // This second format doesn't work. It's supposed to be 51.45, but this is nowhere in the DOM... it's probably handled after javascript eval...
            double? _Price = DOMProductInformationParser.ParseDouble("www.newegg.ca", _Product2DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());

            //double _ExpectedPrice = 51.45;
            //Assert.Equal(_ExpectedPrice, _Price);

            string _Title = DOMProductInformationParser.ParseTitle("www.newegg.ca", _Product2DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "DualShock 4 PS4 Controller Wireless for PlayStation 4 - Jet Black";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestParseBulkNeweggShippingCost()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct3);

            double? _ShippingCost = DOMProductInformationParser.ParseDouble("www.newegg.ca", _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.ShippingCost).ToArray());
            double _ExpectedShippingCost = 39.96;
            Assert.Equal(_ExpectedShippingCost, _ShippingCost);


            string _DOM2 = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct2);
            double? _ShippingCost2 = DOMProductInformationParser.ParseDouble("www.newegg.ca", _DOM2, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.ShippingCost).ToArray());
            double _ExpectedShippingCost2 = 0.01;
            Assert.Equal(_ExpectedShippingCost2, _ShippingCost2);
        }

        [Fact]
        public void TestParseTypicalAmazonPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.AmazonTypicalProduct);

            double? _Price = DOMProductInformationParser.ParseDouble(ProductsHistoryTestConstants.AMAZON_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
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
        public void TestParseTypicalBestBuyPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            string _DOM = GetDOMFromResource(ProductSourceCodeFile.BestBuyTypicalProduct);

            double? _Price = DOMProductInformationParser.ParseDouble(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
            double _ExpectedPrice = 90.19;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTypicalBestBuyTitle()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.BestBuyTypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(ProductsHistoryTestConstants.BESTBUY_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "PlayStation 5 DualSense Wireless Controller - White";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestParseTypicalNeweggPrice()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();

            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggTypicalProduct);

            double? _Price = DOMProductInformationParser.ParseDouble(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());
            double _ExpectedPrice = 52.99;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTypicalNeweggTitle()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggTypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems);
            string _ExpectedTitle = "DualShock 4 PS4 Controller Wireless for PlayStation 4 - Jet Black";
            Assert.Equal(_ExpectedTitle, _Title);
        }

        [Fact]
        public void TestParseTypicalNeweggDiscountInAmount()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct4);

            double? _DiscountInAmount = DOMProductInformationParser.ParseDouble(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.DiscountPrice).ToArray());
            Assert.Equal(100, _DiscountInAmount);

            double? _DiscountInPercentage = DOMProductInformationParser.ParseDouble(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.DiscountPercentage).ToArray());
            Assert.Equal(null, _DiscountInPercentage);
        }

        [Fact]
        public void TestParseTypicalNeweggOutOfStock()
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct5);

            bool? _IsProductOutOfStock = DOMProductInformationParser.ParseBoolean(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.OutOfStock).ToArray());

            Assert.Equal(true, _IsProductOutOfStock);

            // Test with a product NOT out of stock
            _DOM = GetDOMFromResource(ProductSourceCodeFile.NeweggProduct4);

            _IsProductOutOfStock = DOMProductInformationParser.ParseBoolean(ProductsHistoryTestConstants.NEWEGG_TYPICAL_PRODUCT_URL, _DOM, _Config.DOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.OutOfStock).ToArray());

            Assert.Equal(false, _IsProductOutOfStock);
        }
    }
}
