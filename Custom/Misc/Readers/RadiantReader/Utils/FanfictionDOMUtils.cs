using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReader.DataBase;

namespace Radiant.Custom.Readers.RadiantReader.Utils
{
    public static class FanfictionDOMUtils
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public class FanfictionBookDOMDefinition
        {
            public string BookInformationLine { get; set; }
            public string BookSummaryLine { get; set; }
        }

        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string REGEX_INFORMATION_BOOK_LINE = "(class=\"stitle\")";
        private const string REGEX_SUMMARY_BOOK_LINE = "(class=\"z-indent z-padtop\")";

        private static RadiantReaderBookDefinitionModel GenerateReaderBookDefinitionFromSummaryLine(string aBookSummaryLine)
        {
            RadiantReaderBookDefinitionModel _RadiantReaderBookDefinitionModel = new();

            // Extract Summary
            Match _UrlRegexMatch = Regex.Match(aBookSummaryLine, ">(.+?)<div");
            if (_UrlRegexMatch.Success && _UrlRegexMatch.Groups.Count == 2)
                _RadiantReaderBookDefinitionModel.Summary = _UrlRegexMatch.Groups[1].Value;

            Match _MatchOfRemainingInfos = Regex.Match(aBookSummaryLine, "xgray\">(.*)");
            if (!_MatchOfRemainingInfos.Success && _MatchOfRemainingInfos.Groups.Count == 2)
                return _RadiantReaderBookDefinitionModel;

            string _LineContainingRemainingInfo = _MatchOfRemainingInfos.Groups[1].Value;

            Match _RemainingInfosMatch = Regex.Match(_LineContainingRemainingInfo, "(.+?)-");

            if (_RemainingInfosMatch.Groups.Count != 2)
                return _RadiantReaderBookDefinitionModel;

            string _CurrentMatchValue = _RemainingInfosMatch.Groups[1].Value;
            while (!string.IsNullOrWhiteSpace(_CurrentMatchValue))
            {
                if (_CurrentMatchValue.ToLowerInvariant().Contains("rated: "))
                    _RadiantReaderBookDefinitionModel.Rating = _CurrentMatchValue.Substring(_CurrentMatchValue.Length - 3, 2).Trim();
                else if (_CurrentMatchValue.ToLowerInvariant().Contains("words: "))
                {
                    string _Words = _CurrentMatchValue.Substring(7, _CurrentMatchValue.Length - 8).Trim();
                    _Words = _Words.Replace(",", "").Replace(".", "");

                    _RadiantReaderBookDefinitionModel.SoftNbWords = int.Parse(_Words);
                }

                //else if (_CurrentMatchValue.Contains("/"))
                //    _RadiantReaderBookDefinitionModel.Genres = _CurrentMatchValue;

                _RemainingInfosMatch = _RemainingInfosMatch.NextMatch();
                if (!_RemainingInfosMatch.Success || _RemainingInfosMatch.Groups.Count != 2)
                    break;

                _CurrentMatchValue = _RemainingInfosMatch.Groups[1].Value;
            }

            // Main characters
            Match _MainCharactersRegexMatch = Regex.Match(aBookSummaryLine, "</span> - (.+?)</div>");
            if (_MainCharactersRegexMatch.Success && _MainCharactersRegexMatch.Groups.Count == 2)
            {
                string _SplittedValue = _MainCharactersRegexMatch.Groups[1].Value.Split("-").Last();
                _SplittedValue = _SplittedValue.Split("]").Last().Trim();

                string _LowerInvariantSplittedValue = _SplittedValue.ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(_SplittedValue) && !_LowerInvariantSplittedValue.Contains("xutime=") && _LowerInvariantSplittedValue != "complete")
                    _RadiantReaderBookDefinitionModel.MainCharacters = _SplittedValue;
            }

            // finally, the pairing
            Match _PairingRegexMatch = Regex.Match(aBookSummaryLine, "\\[(.+?)\\]");

            if (_PairingRegexMatch.Success && _PairingRegexMatch.Groups.Count == 2)
            {
                int _IndexOfFirstPairing = -1;
                string _Pairings = "";
                do
                {
                    if (!_PairingRegexMatch.Success || _PairingRegexMatch.Groups.Count != 2)
                        break;

                    if (_IndexOfFirstPairing < 0)
                        _IndexOfFirstPairing = aBookSummaryLine.IndexOf(_PairingRegexMatch.Groups[1].Value, StringComparison.InvariantCulture);

                    _Pairings += $"[{_PairingRegexMatch.Groups[1].Value}]";
                    _PairingRegexMatch = _PairingRegexMatch.NextMatch();

                } while (!string.IsNullOrWhiteSpace(_Pairings));

                // If pairings was found in the summary text, but was before XUTime, it was in book summary. Very dependent on fanfiction format, but this whole class is.. : /
                int _IndexOfXUTime = aBookSummaryLine.IndexOf("xutime=", StringComparison.InvariantCultureIgnoreCase);

                // Note that if we don't find xutime or xutime AND pairings, the condition will return true and we'll keep the pairings as it is
                if (_IndexOfFirstPairing < 0 || _IndexOfFirstPairing >= _IndexOfXUTime)
                    _RadiantReaderBookDefinitionModel.Pairings = _Pairings;
            }

            return _RadiantReaderBookDefinitionModel;
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static List<RadiantReaderBookDefinitionModel> GenerateReaderBookDefinitionModelsFromBookDOMDefinitions(List<FanfictionBookDOMDefinition> aDomBookDefinitions)
        {
            List<RadiantReaderBookDefinitionModel> _RadiantReaderBookDefinitionModels = new();
            foreach (FanfictionBookDOMDefinition _BookDomDefinition in aDomBookDefinitions)
            {
                RadiantReaderBookDefinitionModel _RadiantReaderBookDefinitionModel = GenerateReaderBookDefinitionFromSummaryLine(_BookDomDefinition.BookSummaryLine);

                if (_RadiantReaderBookDefinitionModel == null)
                    continue;

                GetUrlAndTitleFromFanfictionInformationLine(_BookDomDefinition.BookInformationLine, out string _Url, out string _Title);
                _RadiantReaderBookDefinitionModel.Url = _Url;
                _RadiantReaderBookDefinitionModel.Title = _Title;

                _RadiantReaderBookDefinitionModels.Add(_RadiantReaderBookDefinitionModel);
            }

            return _RadiantReaderBookDefinitionModels;
        }

        private static void GetUrlAndTitleFromFanfictionInformationLine(string aBookInformationLine, out string aUrl, out string aTitle)
        {
            aUrl = null;
            aTitle = null;

            // Extract Url
            var _UrlRegexMatch = Regex.Match(aBookInformationLine, "class=\"stitle\" href=\"(.+?)(?=\">)");
            if (_UrlRegexMatch.Success && _UrlRegexMatch.Groups.Count == 2)
                aUrl = _UrlRegexMatch.Groups[1].Value;

            // Extract Title
            var _TitleRegexMatch = Regex.Match(aBookInformationLine, "\"66\">(.+?)<\\/a>");// That's a very weak regex... find a better way
            if (_TitleRegexMatch.Success && _TitleRegexMatch.Groups.Count == 2)
                aTitle = BookStringUtils.FormatTitle(_TitleRegexMatch.Groups[1].Value);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static RadiantReaderBookChapter ParseBookChapterFromDOM(string aDom, int aChapterIndex, long aBookDefinitionId)
        {
            if (string.IsNullOrWhiteSpace(aDom))
            {
                LoggingManager.LogToFile("48a2aa90-05b7-4b71-aed9-37fe850b16fa", "DOM was empty.");
                throw new Exception("26c9069d-2f45-4ac3-b405-6a68b30821ba_DOM was empty.");
            }

            RadiantReaderBookChapter _Chapter = new();

            Regex _ChapterContentRegex = new Regex("id=\"storytext\">(.+?)</div>");// until next div

            var _MatchContent = _ChapterContentRegex.Match(aDom);
            if (!_MatchContent.Success)
            {
                _ChapterContentRegex = new Regex("id=\"storytext\">(.+?)$", RegexOptions.Multiline);// Just until end of line

                _MatchContent = _ChapterContentRegex.Match(aDom);
                if (!_MatchContent.Success)
                {
                    LoggingManager.LogToFile("30a0e9ff-3ad0-40fc-be36-4f2cf2292cc0", "Couldn't match fanfiction book content");
                    throw new Exception("203dd19b-d832-4d00-aa28-39fe05364a23_Couldn't match book content.");
                }
            }

            _Chapter.ChapterContent = BookStringUtils.FormatSummary(_MatchContent.Groups.Values.Last().Value);
            _Chapter.BookDefinitionId = aBookDefinitionId;
            _Chapter.ChapterNumber = aChapterIndex;

            _Chapter.ChapterWordsCount = _Chapter.ChapterContent.Split(" ").Length - 1 +
                                         _Chapter.ChapterContent.Split("'").Length - 1 +
                                         _Chapter.ChapterContent.Split("</p").Length - 1 +
                                         1;// 0 based

            return _Chapter;
        }

        public static List<RadiantReaderBookDefinitionModel> ParseBooksFromDOM(string aDOM)
        {
            // TODO: ParseBooksFromFanfictionDOM should return it's own model and not RadiantReaderBookDefinitionModel
            List<FanfictionBookDOMDefinition> _DOMBookDefinitions = new();

            string[] _Lines = aDOM.Split(Environment.NewLine);

            for (int i = 0; i < _Lines.Length; i++)
            {
                // Check if current line is a book information line, a book summary line or a line to discard
                Match _MatchInformationLine = Regex.Match(_Lines[i], REGEX_INFORMATION_BOOK_LINE);
                if (!_MatchInformationLine.Success)
                    continue;// discard line

                if (_Lines.Length <= i + 1)
                {
                    LoggingManager.LogToFile("05b6d6f3-3500-492e-8715-a468c0cabb83", $"Couldn't find summary at line [{i + 1}] for line [{_Lines[i]}]. End of file met before summary.");
                    continue;// skip this book information and summary
                }

                Match _MatchSummaryLine = Regex.Match(_Lines[i + 1], REGEX_SUMMARY_BOOK_LINE);

                if (!_MatchSummaryLine.Success)
                {
                    LoggingManager.LogToFile("a88fe850-a336-4556-849c-005a1bd830b9", $"Couldn't find summary at line [{i + 1}] for line [{_Lines[i]}]. Summary was [{_Lines[i + 1]}].");
                    continue;// skip this book information and summary
                }

                _DOMBookDefinitions.Add(new FanfictionBookDOMDefinition
                {
                    BookInformationLine = _Lines[i],
                    BookSummaryLine = _Lines[i + 1]
                });
            }

            return GenerateReaderBookDefinitionModelsFromBookDOMDefinitions(_DOMBookDefinitions);
        }
    }
}
