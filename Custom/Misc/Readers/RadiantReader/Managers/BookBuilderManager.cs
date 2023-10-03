using System;
using System.Collections.Generic;
using Radiant.Custom.Readers.RadiantReader.Business;
using Radiant.Custom.Readers.RadiantReader.DataBase;
using Radiant.Custom.Readers.RadiantReader.Utils;

namespace Radiant.Custom.Readers.RadiantReader.Managers
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
