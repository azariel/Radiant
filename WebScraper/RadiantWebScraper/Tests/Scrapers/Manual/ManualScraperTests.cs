using Radiant.Common.Configuration;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.WebScraper.RadiantWebScraper.Tests.Scrapers.Manual
{
    public class ManualScraperTests
    {
        public ManualScraperTests()
        {
            // Override RadiantCommonConfig.json file so we avoid loading dependencies
            CommonConfigurationManager.SetConfigInMemory(new RadiantCommonConfig());
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Get the DOM of the specified url
        /// </summary>
        [Fact]
        public void TestManualGetDomFromBasicUrl()
        {
            ManualScraper _ManualScraper = new ManualScraper();

            var _DomScraper = new ManualDOMTargetScraper();
            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            Assert.NotNull(_DomScraper.DOM);
            Assert.NotEqual("", _DomScraper.DOM);
            Assert.Null(_DomScraper.Screenshot);
        }

        /// <summary>
        /// Take a screenshot of the default url and then, execute the dom scrap
        /// </summary>
        [Fact]
        public void TestManualGetDomAndScreenshotFromBasicUrl()
        {
            ManualScraper _ManualScraper = new ManualScraper();

            var _DomScraper = new ManualDOMTargetScraper(ManualBaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            Assert.NotNull(_DomScraper.DOM);
            Assert.NotEqual("", _DomScraper.DOM);
            Assert.NotEmpty(_DomScraper.Screenshot);
        }
    }
}
