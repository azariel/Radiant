using System;
using System.IO;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;
using Radiant.Custom.Readers.RadiantReaderCommon.Utils;
using Radiant.WebScraper.RadiantClientWebScraper;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Parsers
{
    internal static class FanfictionFetcher
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************

        private static bool DOMIsValidChapter(string aCurrentDom)
        {
            if (string.IsNullOrWhiteSpace(aCurrentDom))
                return false;

            if (aCurrentDom.ToLowerInvariant().Contains("chapter not found. please check to see you are not using an outdated url"))
                return false;

            if (aCurrentDom.ToLowerInvariant().Contains("Story is unavailable for reading"))
                return false;

            return true;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantReaderBookChapter FetchNextChapterFromBookDefinition(
            RadiantReaderBookDefinitionModel aBookDefinition, out bool shouldStop)
        {
            shouldStop = false;

            if (aBookDefinition == null)
                return null;

            int _ChapterIndex = aBookDefinition.Chapters.Count + 1;// Note that fanfiction.net chapter urls are 1 based

            string _BaseChapterUrl = $"http://www.fanfiction.net{aBookDefinition.Url}";

            // Fetch chapter
            // Set chapter URL
            string _CurrentChapterUrl = _BaseChapterUrl.Replace("/1/", $"/{_ChapterIndex}/");

            // Get DOM
            string _CurrentDOM = ManualWebScraperClient.GetDOMAsync(_CurrentChapterUrl).Result;
            shouldStop = true;

            if (!DOMIsValidChapter(_CurrentDOM))
                return null;

            RadiantReaderBookChapter _NewChapter;
            try
            {
                _NewChapter = FanfictionDOMUtils.ParseBookChapterFromDOM(_CurrentDOM, _ChapterIndex, aBookDefinition.BookDefinitionId);
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("76ad9cdb-891b-4053-86fe-7a8d4d1fdb19", $"Couldn't fetch chapter [{_ChapterIndex}] of [{aBookDefinition.Title}] on Url [{aBookDefinition.Url}]. DOM is invalid.", _Ex);

                // Note: we don't crash. we log it in a specific file since the DOM takes up a ton of space and would pollute the main log file
                string path = Path.Combine("readersErrors", $"{nameof(FanfictionFetcher)}-{DateTime.UtcNow:yyyy.MM.dd HH.mm.ss}.log");
                string directoryPath = Path.GetDirectoryName(path);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                LoggingManager.LogToFile("4ebe3c0b-69e9-4022-9336-d251cd594b49", $"NewChapter couldn't be parsed from DOM [{_CurrentDOM}]. Web crawler will end on chapter [{_ChapterIndex - 1}.]", _Ex, aLogFilePath: Path.Combine("readersErrors", $"{nameof(FanfictionFetcher)}-{DateTime.UtcNow:yyyy.MM.dd HH.mm.ss}.log"));
                return null;
            }

            return _NewChapter;
        }
    }
}
