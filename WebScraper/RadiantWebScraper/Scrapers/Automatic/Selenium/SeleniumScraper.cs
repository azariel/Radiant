using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;

namespace Radiant.WebScraper.Scrapers.Automatic.Selenium
{
    public class SeleniumScraper : IScraper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void GetTargetValueFromUrl(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aParserItems)
        {
            // Pretty straightforward. Selenium doesn't do any wrapping magic
            aTarget.Evaluate(aSupportedBrowser, aUrl, false, aParserItems?.OfType<SeleniumParserItemParser>().Select(s => (IScraperItemParser)s).ToList(), aParserItems);
        }

        public async Task GetTargetValueFromUrlAsync(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aParserItems)
        {
            await Task.Run(() =>
            {
                GetTargetValueFromUrl(aSupportedBrowser, aUrl, aTarget, aScraperItemsParser, aParserItems);
                return aTarget;
            });
        }
    }
}
