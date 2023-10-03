using System.Collections.Generic;
using System.Linq;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;

namespace Radiant.WebScraper.RadiantWebScraper.Scrapers.Automatic.HTTPClient
{
    public class HttpClientScraper : IScraper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void GetTargetValueFromUrl(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aHttpClientScraperItems, List<DOMParserItem> aParserItems)
        {
            // Pretty straighforward. HttpClient doesn't do any wrapping magic
            aTarget.Evaluate(aSupportedBrowser, aUrl, false, aHttpClientScraperItems?.OfType<HttpClientParserItemParser>().Select(s=>(IScraperItemParser)s).ToList(), aParserItems);
        }
    }
}
