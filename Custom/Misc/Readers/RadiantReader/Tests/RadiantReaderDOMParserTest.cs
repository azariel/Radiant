using RadiantReader.Utils;
using Xunit;

namespace RadiantReader.Tests
{
    public class RadiantReaderDOMParserTest
    {
        [Fact]
        public void TestFanfictionDOM()
        {
            string _DOM = Resources.ResourceDom.Fanfiction_2022_04_08;

            var _Books = FanfictionDOMUtils.ParseBooksFromFanfictionDOM(_DOM);
            Assert.Equal(25, _Books.Count);
        }
    }
}
