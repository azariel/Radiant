namespace Radiant.WebScraper.Business.Objects.ScraperTargetValue
{
    public class ScraperTargetValueString : IScraperTargetValue
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public object Value { get; set; }

        public string ValueAsString
        {
            get => this.Value as string;
        }
    }
}
