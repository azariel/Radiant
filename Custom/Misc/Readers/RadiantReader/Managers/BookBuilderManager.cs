using System;
using System.Collections.Generic;
using RadiantReader.Business;
using RadiantReader.DataBase;
using RadiantReader.Utils;

namespace RadiantReader.Managers
{
    public static class BookBuilderManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************

        public static List<RadiantReaderBookDefinitionModel> ParseBooksFromDOM(BookProvider aBookProvider, string aDOM)
        {
            switch (aBookProvider)
            {
                case BookProvider.ArchiveOfOurOwn:
                    return ArchiveOfOurOwnDOMUtils.ParseBooksFromDOM(aDOM);
                    break;
                case BookProvider.Fanfiction:
                    return FanfictionDOMUtils.ParseBooksFromDOM(aDOM);
                default:
                    throw new ArgumentOutOfRangeException(nameof(aBookProvider), aBookProvider, $"Unhandled [{aBookProvider}].");
            }
        }
    }
}
