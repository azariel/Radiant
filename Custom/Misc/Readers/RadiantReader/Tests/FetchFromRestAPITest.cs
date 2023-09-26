using RadiantClientWebScraper;
using Xunit;

namespace RadiantReader.Tests
{
    public class FetchFromRestAPITest
    {
        [Fact]
        public void TestMicroServiceGetDOMFromRestAPI()
        {
            string _DOM = AutomaticWebScraperClient.GetDOMAsync("http://www.perdu.com").Result;

            Assert.NotNull(_DOM);
            Assert.NotEmpty(_DOM);
        }
    }
}
