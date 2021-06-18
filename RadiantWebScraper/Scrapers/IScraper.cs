using Radiant.WebScraper.Business.Objects.TargetScraper;

namespace Radiant.WebScraper.Scrapers
{
    internal interface IScraper
    {
        void GetTargetValueFromUrl(SupportedBrowser aSupportedBrowser, string aUrl, IScraperTarget aTarget);
    }
}
