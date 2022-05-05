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
        //                            Private
        // ********************************************************************

        private static bool DOMIsValidChapter(string aCurrentDom)
        {
            if (string.IsNullOrWhiteSpace(aCurrentDom))
                return false;

            if (aCurrentDom.ToLowerInvariant().Contains("chapter not found. please check to see you are not using an outdated url"))
                return false;

            return true;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantReaderBookChapter FetchNextChapterFromBookDefinition(RadiantReaderBookDefinitionModel aBookDefinition)
        {
            if (aBookDefinition == null)
                return null;

            int _ChapterIndex = aBookDefinition.Chapters.Count + 1;// Note that fanfiction.net chapter urls are 1 based

            string _BaseChapterUrl = $"http://www.fanfiction.net{aBookDefinition.Url}";

            // Fetch chapter
            // Set chapter URL
            string _CurrentChapterUrl = _BaseChapterUrl.Replace("/1/", $"/{_ChapterIndex}/");

            // Get DOM
            string _CurrentDOM = ManualWebScraperClient.GetDOMAsync(_CurrentChapterUrl).Result;

            if (!DOMIsValidChapter(_CurrentDOM))
                return null;

            RadiantReaderBookChapter _NewChapter;
            try
            {
                _NewChapter = FanfictionDOMUtils.ParseBookChapterFromFanfictionDOM(_CurrentDOM, _ChapterIndex, aBookDefinition.BookDefinitionId);
            } catch (Exception _Ex)
            {
                // Note: we don't crash. we log it
                LoggingManager.LogToFile("4ebe3c0b-69e9-4022-9336-d251cd594b49", $"NewChapter couldn't be parsed from DOM [{_CurrentDOM}]. Web crawler will end on chapter [{_ChapterIndex - 1}.]", _Ex);
                return null;
            }

            return _NewChapter;
        }
    }
}
