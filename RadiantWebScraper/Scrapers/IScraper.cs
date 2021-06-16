namespace Radiant.WebScraper.Scrapers
{
    internal interface IScraper
    {
        string GetDOMFromUrl(SupportedBrowser aSupportedBrowser, string aUrl);
    }
}
