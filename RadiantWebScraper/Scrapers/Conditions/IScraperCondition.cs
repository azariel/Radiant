namespace Radiant.WebScraper.Scrapers.Conditions
{
    public interface IScraperCondition
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate(SupportedBrowser? aBrowser);
    }
}
