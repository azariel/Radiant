using System.Collections.Generic;
using RadiantReader.Business;
using RadiantReader.DataBase;
using RadiantReader.Managers;
using RadiantReader.Tests.Resources;
using Xunit;

namespace RadiantReader.Tests
{
    public class RadiantReaderDOMParserTest
    {
        [Fact]
        public void TestArchiveOfOurOwnDOM()
        {
            string _DOM = ResourceDom.ArchiveOfOurOwn_2022_05_09;

            List<RadiantReaderBookDefinitionModel> _Books = BookBuilderManager.ParseBooksFromDOM(BookProvider.ArchiveOfOurOwn, _DOM);
            Assert.Equal(expected: 20, _Books.Count);
        }

        [Fact]
        public void TestFanfictionDOM()
        {
            string _DOM = ResourceDom.Fanfiction_2022_04_08;

            List<RadiantReaderBookDefinitionModel> _Books = BookBuilderManager.ParseBooksFromDOM(BookProvider.Fanfiction, _DOM);
            Assert.Equal(expected: 25, _Books.Count);
        }
    }
}
