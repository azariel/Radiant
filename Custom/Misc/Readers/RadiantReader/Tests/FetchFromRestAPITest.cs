using System.Net.Http;
using Xunit;

namespace RadiantReader.Tests
{
    public class FetchFromRestAPITest
    {
        [Fact]
        public void TestMicroServiceGetDOMFromRestAPI() 
        {
            //SeleniumScraper _SeleniumScraper = new SeleniumScraper();

            //// Firefox
            //var _DomScraper = new SeleniumDOMTargetScraper();
            //_SeleniumScraper.GetTargetValueFromUrl(Browser.Firefox, "http://www.perdu.com", _DomScraper, null, null);

            //Assert.NotNull(_DomScraper.DOM);
            //Assert.NotEqual("", _DomScraper.DOM);

            //Thread.Sleep(1000);

            HttpClient client = new HttpClient();

            string _DOM;
            HttpResponseMessage _Response = client.GetAsync("127.0.0.1/AutomaticWebScraper").Result;
            if (_Response.IsSuccessStatusCode)
            {
                _DOM = _Response.Content.ReadAsStringAsync().Result;
            }


        }
    }
}
