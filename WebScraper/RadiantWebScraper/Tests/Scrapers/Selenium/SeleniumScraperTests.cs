using System.Threading;
using Radiant.Common.Configuration;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Automatic.Selenium;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Automatic.Selenium;
using Xunit;

namespace Radiant.WebScraper.RadiantWebScraper.Tests.Scrapers.Selenium
{
    public class SeleniumScraperTests
    {
        public SeleniumScraperTests()
        {
           // Override RadiantCommonConfig.json file so we avoid loading dependencies
            CommonConfigurationManager.SetConfigInMemory(new RadiantCommonConfig()); 
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestBasicUrl()
        {
            SeleniumScraper _SeleniumScraper = new SeleniumScraper();

            // Firefox
            var _DomScraper = new SeleniumDOMTargetScraper();
            _SeleniumScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            Assert.NotNull(_DomScraper.DOM);
            Assert.NotEqual("", _DomScraper.DOM);

            Thread.Sleep(1000);

            // Chrome
            var _DomScraper2 = new SeleniumDOMTargetScraper();
            _SeleniumScraper.GetTargetValueFromUrl(Browser.Chrome, "http://www.perdu.com", _DomScraper2, null, null);

            Assert.NotNull(_DomScraper2.DOM);
            Assert.NotEqual("", _DomScraper2.DOM);
        }
    }
}
