using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Scrapers.Manual;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.Manual
{
    public class ManualScraperTests
    {
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
