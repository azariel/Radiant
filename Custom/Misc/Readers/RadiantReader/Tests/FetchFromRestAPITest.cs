using RadiantClientWebScraper;
using Xunit;

namespace RadiantReader.Tests
{
    public class FetchFromRestAPITest
    {
        [Fact]
        public void TestMicroServiceGetDOMFromRestAPI()
        {
            string _DOM = AutomaticWebScraperClient.GetDOM("http://www.perdu.com");

            Assert.NotNull(_DOM);
            Assert.NotEmpty(_DOM);
        }
    }
}
