namespace Radiant.WebScraper.RadiantWebScraper.Scrapers.Conditions
{
    public interface IScraperCondition
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public bool Evaluate(Browser? aBrowser);
    }
}
