using Radiant.WebScraper.Parsers.DOM;

namespace Radiant.Custom.ProductsHistory.Parsers
{
    public class ProductDOMParserItem : DOMParserItem
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum ProductDOMParserItemTarget
        {
            Price,
            Title
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductDOMParserItemTarget ParserItemTarget { get; set; }
    }
}
