﻿using Radiant.Custom.Readers.RadiantReaderCommon.Business;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;
using Radiant.Custom.Readers.RadiantReaderCommon.Utils;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Parsers
{
    internal class BookFetcher
    {
        internal static RadiantReaderBookChapter FetchNextChapterFromBookDefinition(
            RadiantReaderBookDefinitionModel aBookDefinition, out bool shouldStop)
        {
            RadiantReaderBookChapter _NewChapter;

            switch (BookProviderUtils.GetProviderFromUrl(aBookDefinition.Host.HostLandingPage))
            {
                case BookProvider.Fanfiction:
                    _NewChapter = FanfictionFetcher.FetchNextChapterFromBookDefinition(aBookDefinition, out shouldStop);
                    break;
                case BookProvider.ArchiveOfOurOwn:
                    _NewChapter = ArchiveOfOurOwnFetcher.FetchNextChapterFromBookDefinition(aBookDefinition, out shouldStop);
                    break;
                default:
                    throw new Exception($"ec7c365e-ed87-4271-ae24-330728f6cf4f - Book provider [{aBookDefinition.BookHostId}] is unhandled.");
            }

            return _NewChapter;
        }
    }
}
