namespace Radiant.WebScraper.RadiantWebScraper.Scrapers.Conditions
{
    public class StringValueCondition : IScraperCondition
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum MatchCondition
        {
            Match,
            DontMatch
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************

        public bool Evaluate(Browser? aBrowser)
        {

            return false;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string Value { get; set; }
        public bool InvariantCase { get; set; } = false;
        public MatchCondition Match { get; set; } = MatchCondition.Match;
    }
}