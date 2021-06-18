using Radiant.WebScraper.Business.Objects.TargetScraper;
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

            var _DomScraper = new DOMTargetScraper();
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, "http://www.perdu.com", _DomScraper);

            Assert.NotEqual("", _DomScraper.DOM);
        }

        [Fact]
        public void TestManualGetDomAndScreenshotFromBasicUrl()
        {
            ManualScraper _ManualScraper = new ManualScraper();

            var _DomScraper = new DOMTargetScraper(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, "http://www.perdu.com", _DomScraper);

            Assert.NotEqual("", _DomScraper.DOM);
            Assert.NotEmpty(_DomScraper.Screenshot);
        }
    }
}
