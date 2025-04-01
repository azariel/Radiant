using System;
using System.Collections.Generic;
using Radiant.Custom.Readers.RadiantReaderCommon.Business;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;
using Radiant.Custom.Readers.RadiantReaderCommon.Utils;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Managers
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
                case BookProvider.Fanfiction:
                    return FanfictionDOMUtils.ParseBooksFromDOM(aDOM);
                default:
                    throw new ArgumentOutOfRangeException(nameof(aBookProvider), aBookProvider, $"Unhandled [{aBookProvider}].");
            }
        }
    }
}
