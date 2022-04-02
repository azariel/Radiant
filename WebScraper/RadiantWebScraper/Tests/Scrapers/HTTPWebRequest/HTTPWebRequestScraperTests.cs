using Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.HttpClient;
using Radiant.WebScraper.Scrapers.Automatic.HTTPClient;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.HTTPWebRequest
{
    public class HTTPWebRequestScraperTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestBasicUrl()
        {
            HttpClientScraper _HttpClientScraper = new HttpClientScraper();

            var _DomScraper = new HttpClientDOMTargetScraper();
            _HttpClientScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            Assert.NotNull(_DomScraper.DOM);
            Assert.NotEqual("", _DomScraper.DOM);
        }
    }
}
