using Radiant.WebScraper.RadiantClientWebScraper;
using Xunit;

namespace Radiant.Custom.Readers.RadiantReader.Tests
{
    public class FetchFromRestAPITest
    {
        [Fact]
        public async void TestMicroServiceGetDOMFromRestAPI()
        {
            string _DOM = await AutomaticWebScraperClient.GetDOMAsync("http://www.perdu.com");

            Assert.NotNull(_DOM);
            Assert.NotEmpty(_DOM);
        }
    }
}
