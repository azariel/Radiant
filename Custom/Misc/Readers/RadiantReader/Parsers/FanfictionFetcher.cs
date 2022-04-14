﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

            string _CurrentChapterURL = $"http://www.fanfiction.net{aBookDefinition.Url}";
            string _CurrentDOM = null;

            // Fetch chapters
            List<RadiantReaderBookChapter> _ChaptersToAdd = new();
            do
            {
                // Set chapter URL
                _CurrentChapterURL = _CurrentChapterURL.Replace($"/{_ChapterIndex - 1}/", $"/{_ChapterIndex}/");

                // Get DOM
                string _DOM = ManualWebScraperClient.GetDOMAsync(_CurrentChapterURL).Result;

                var _NewChapter = FanfictionDOMUtils.ParseBookChapterFromFanfictionDOM(_DOM, _ChapterIndex, aBookDefinition.BookDefinitionId);
                _ChaptersToAdd.Add(_NewChapter);

                _ChapterIndex++;

                // Add a little sleep to avoid being tagged as a bot too easily
                Thread.Sleep(new Random().Next(5765,9457));

            } while (DOMIsValidChapter(_CurrentDOM));

            aBookDefinition.Chapters.AddRange(_ChaptersToAdd);
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
