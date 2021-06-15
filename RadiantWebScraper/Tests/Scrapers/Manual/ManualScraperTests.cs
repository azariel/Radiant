using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.Manual
{
    public class ManualScraperTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestManualGetDomFromBasicUrl()
        {
            ManualScraper _ManualScraper = new ManualScraper();
            string _DOM = _ManualScraper.GetDOMFromUrl("http://www.perdu.com");
        }
    }
}
