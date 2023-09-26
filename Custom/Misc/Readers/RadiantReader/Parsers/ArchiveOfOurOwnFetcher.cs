using System;
using System.Linq;
using Radiant.Common.Diagnostics;
using RadiantClientWebScraper;
using RadiantReader.DataBase;
using RadiantReader.Utils;
using System.Text.RegularExpressions;

namespace RadiantReader.Parsers
{
    internal static class ArchiveOfOurOwnFetcher
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
        public static RadiantReaderBookChapter FetchNextChapterFromBookDefinition(RadiantReaderBookDefinitionModel aBookDefinition, out bool shouldStop)
        {
            shouldStop = false;

            if (aBookDefinition == null)
                return null;

            int _ChapterIndex = aBookDefinition.Chapters.Count + 1;// Note that archiveofourown.org chapter urls are 1 based

            string _BaseChapterUrl = $"https://archiveofourown.org{aBookDefinition.Url}";

            // Fetch chapter
            // Set chapter URL
            string _CurrentChapterUrl = $"{_BaseChapterUrl}";

            RadiantReaderBookChapter _MostRecentChapter = null;
            if (_ChapterIndex > 1)
            {
                _MostRecentChapter = aBookDefinition.Chapters.LastOrDefault();
                if (_MostRecentChapter == null)
                {
                    throw new Exception($"Book [{aBookDefinition.BookDefinitionId}] has [{aBookDefinition.Chapters.Count}] chapter, but couldn't get most recent one.");
                }

                if (_MostRecentChapter.NextChapterPartialUrl == null)
                {
                    // Fetch this chapter DOM, find the next chapter URL, update that chapter and then continue
                    string _MostRecentChapterDOM = AutomaticWebScraperClient.GetDOMAsync(_CurrentChapterUrl).Result;
                    _MostRecentChapter.NextChapterPartialUrl = GetNextchapterRelativeUrlFromDom(_MostRecentChapterDOM);
                }

                _CurrentChapterUrl += $"/chapters/{_MostRecentChapter.NextChapterPartialUrl}";
            }

            // Get DOM
            string _CurrentDOM = AutomaticWebScraperClient.GetDOMAsync(_CurrentChapterUrl).Result;

            if (!DOMIsValidChapter(_CurrentDOM))
                return null;

            RadiantReaderBookChapter _NewChapter;
            try
            {
                _NewChapter = ArchiveOfOurOwnDOMUtils.ParseBookChapterFromDOM(_CurrentDOM, _ChapterIndex, aBookDefinition.BookDefinitionId);

                if (_MostRecentChapter != null)
                    _NewChapter.ChapterPartialUrl = _MostRecentChapter?.NextChapterPartialUrl;

                _NewChapter.NextChapterPartialUrl = GetNextchapterRelativeUrlFromDom(_CurrentDOM);
            }
            catch (Exception _Ex)
            {
                // Note: we don't crash. we log it
                LoggingManager.LogToFile("cb817d47-7f81-4376-bca0-6c3d68d40e22", $"NewChapter couldn't be parsed from DOM [{_CurrentDOM}]. Web crawler will end on chapter [{_ChapterIndex - 1}.]", _Ex);
                return null;
            }

            return _NewChapter;
        }

        private static string GetNextchapterRelativeUrlFromDom(string aMostRecentChapterDOM)
        {
            Regex _NextChapterFinderRegex = new Regex("href=\".*chapters/(.*)#.*\">");

            var _Match = _NextChapterFinderRegex.Match(aMostRecentChapterDOM);
            if (!_Match.Success || _Match.Groups.Count <= 1)
            {
                throw new Exception($"Next chapter not found.");
            }

            return _Match.Groups[1].Value;
        }
    }
}
