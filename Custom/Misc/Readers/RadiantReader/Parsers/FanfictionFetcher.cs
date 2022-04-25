using System;
using System.Threading;
using Radiant.Common.Diagnostics;
using RadiantClientWebScraper;
using RadiantReader.DataBase;
using RadiantReader.Utils;

namespace RadiantReader.Parsers
{
    internal static class FanfictionFetcher
    {
        // ********************************************************************
        //                            Internal
        // ********************************************************************
        public static void FetchNewChaptersFromBookDefinition(RadiantReaderBookDefinitionModel aBookDefinition)
        {
            if (aBookDefinition == null)
                return;

            int _ChapterIndex = aBookDefinition.Chapters.Count + 1;// Note that fanfiction.net chapter urls are 1 based

            string _CurrentChapterUrl = $"http://www.fanfiction.net{aBookDefinition.Url}";

            // Fetch chapters
            while (true)
            {
                // Set chapter URL
                _CurrentChapterUrl = _CurrentChapterUrl.Replace($"/{_ChapterIndex - 1}/", $"/{_ChapterIndex}/");

                // Get DOM
                string _CurrentDOM = ManualWebScraperClient.GetDOMAsync(_CurrentChapterUrl).Result;

                if (!DOMIsValidChapter(_CurrentDOM))
                    break;

                RadiantReaderBookChapter _NewChapter;
                try
                {
                    _NewChapter = FanfictionDOMUtils.ParseBookChapterFromFanfictionDOM(_CurrentDOM, _ChapterIndex, aBookDefinition.BookDefinitionId);
                }
                catch (Exception _Ex)
                {
                    // Note: we don't crash. we log it, but we 
                    LoggingManager.LogToFile("4ebe3c0b-69e9-4022-9336-d251cd594b49", $"NewChapter couldn't be parsed from DOM [{_CurrentDOM}]. Web crawler will end on chapter [{_ChapterIndex - 1}.]", _Ex);
                    break;
                }

                aBookDefinition.Chapters.Add(_NewChapter);

                _ChapterIndex++;

                // Add a little sleep to avoid being tagged as a bot too easily
                Thread.Sleep(new Random().Next(5765, 9457));
            }
        }

        private static bool DOMIsValidChapter(string aCurrentDom)
        {
            if (string.IsNullOrWhiteSpace(aCurrentDom))
                return false;

            if (aCurrentDom.ToLowerInvariant().Contains("chapter not found. please check to see you are not using an outdated url"))
                return false;

            return true;
        }
    }
}
