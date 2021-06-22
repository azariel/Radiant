using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Radiant.Custom.ProductsHistory.Scraper;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Scraper
{
    public class DOMInformationParserTests
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        private enum ProductSourceCodeFile
        {
            TypicalProduct
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private string GetDOMFromResource(ProductSourceCodeFile aProductSourceCodeFile)
        {
            switch (aProductSourceCodeFile)
            {
                case ProductSourceCodeFile.TypicalProduct:
                    return Resources.UnitTestsResources.ResourceManager.GetObject("Product1_SourceCode") as string;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aProductSourceCodeFile), aProductSourceCodeFile, null);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestParsePrice()
        {
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.TypicalProduct);

            double? _Price = DOMProductInformationParser.ParsePrice(_DOM);
            double _ExpectedPrice = 89.96;
            Assert.Equal(_ExpectedPrice, _Price);
        }

        [Fact]
        public void TestParseTitle()
        {
            string _DOM = GetDOMFromResource(ProductSourceCodeFile.TypicalProduct);

            string _Title = DOMProductInformationParser.ParseTitle(_DOM);
            string _ExpectedTitle = "PlayStation DualSense Wireless Controller – Midnight Black - Midnight Black Edition: PlayStation: Computer and Video Games - Amazon.ca";
            Assert.Equal(_ExpectedTitle, _Title);
        }
    }
}
