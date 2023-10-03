using System.Collections.Generic;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;

namespace Radiant.WebScraper.RadiantWebScraper.Scrapers
{
    internal interface IScraper
    {
        void GetTargetValueFromUrl(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aParserItems);
    }
}
