using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.Manual
{
    /// <summary>
    /// Manual scraper is a very little special tool. It reproduce user inputs to physically go to the website and scrap
    /// manually the source or the data it wants. It's often a good way to avoid bot detection, but it's painfully slow,
    /// among other things.. So we often consider it to be "the last stand" to fetch the data and should only be used on
    /// a dedicated server..
    /// </summary>
    public class ManualScraperTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestManualGetDomFromBasicUrl()
        {
            ManualScraper _ManualScraper = new ManualScraper();
            string _DOM = _ManualScraper.GetDOMFromUrl(SupportedBrowser.Firefox, "http://www.perdu.com");
        }
    }
}
