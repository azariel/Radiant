﻿using RadiantReader.Business;
using RadiantReader.DataBase;
using RadiantReader.Utils;
using System;

namespace RadiantReader.Parsers
{
    internal class BookFetcher
    {
        internal static RadiantReaderBookChapter FetchNextChapterFromBookDefinition(RadiantReaderBookDefinitionModel aBookDefinition)
        {
            RadiantReaderBookChapter _NewChapter;

            switch (BookProviderUtils.GetProviderFromUrl(aBookDefinition.Host.HostLandingPage))
            {
                case BookProvider.Fanfiction:
                    _NewChapter = FanfictionFetcher.FetchNextChapterFromBookDefinition(aBookDefinition);
                    break;
                case BookProvider.ArchiveOfOurOwn:
                    _NewChapter = ArchiveOfOurOwnFetcher.FetchNextChapterFromBookDefinition(aBookDefinition);
                    break;
                default:
                    throw new Exception($"ec7c365e-ed87-4271-ae24-330728f6cf4f - Book provider [{aBookDefinition.BookHostId}] is unhandled.");
            }

            return _NewChapter;
        }
    }
}