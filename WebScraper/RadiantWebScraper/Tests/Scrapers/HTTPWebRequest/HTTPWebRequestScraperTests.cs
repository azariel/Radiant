using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Automatic.HttpClient;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Automatic.HTTPClient;
using Xunit;

namespace Radiant.WebScraper.RadiantWebScraper.Tests.Scrapers.HTTPWebRequest
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
