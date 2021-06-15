namespace Radiant.WebScraper.Scrapers
{
    internal interface IScraper
    {
        string GetDOMFromUrl(string aUrl);
    }
}
