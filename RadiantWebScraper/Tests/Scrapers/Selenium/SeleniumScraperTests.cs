using Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.Selenium
{
    public class SeleniumScraperTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestBasicUrl()
        {
            SeleniumScraper _SeleniumScraper = new SeleniumScraper();

            var _DomScraper = new SeleniumDOMTargetScraper();
            _SeleniumScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            Assert.NotNull(_DomScraper.DOM);
            Assert.NotEqual("", _DomScraper.DOM);
        }
    }
}
