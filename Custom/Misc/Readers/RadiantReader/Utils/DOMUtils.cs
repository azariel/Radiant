using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Radiant.Common.Diagnostics;
using RadiantReader.DataBase;

namespace RadiantReader.Utils
{
    public static class DOMUtils
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

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static List<RadiantReaderBookDefinitionModel> GenerateReaderBookDefinitionModelsFromFanfictionBookDOMDefinitions(List<FanfictionBookDOMDefinition> aDomBookDefinitions)
        {
            List<RadiantReaderBookDefinitionModel> _RadiantReaderBookDefinitionModels = new();
            foreach (FanfictionBookDOMDefinition _BookDomDefinition in aDomBookDefinitions)
            {
                RadiantReaderBookDefinitionModel _RadiantReaderBookDefinitionModel = GenerateReaderBookDefinitionFromFanfictionSummaryLine(_BookDomDefinition.BookSummaryLine);

                if (_RadiantReaderBookDefinitionModel == null)
                    continue;

                GetUrlAndTitleFromFanfictionInformationLine(_BookDomDefinition.BookInformationLine, out string _Url, out string _Title);
                _RadiantReaderBookDefinitionModel.Url = _Url;
                _RadiantReaderBookDefinitionModel.Title = _Title;

                _RadiantReaderBookDefinitionModels.Add(_RadiantReaderBookDefinitionModel);
            }

            return _RadiantReaderBookDefinitionModels;
        }

        private static RadiantReaderBookDefinitionModel GenerateReaderBookDefinitionFromFanfictionSummaryLine(string aBookSummaryLine)
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
                _SplittedValue = _SplittedValue.Split("]").Last(); 

                if (!_SplittedValue.ToLowerInvariant().Contains("xutime="))
                    _RadiantReaderBookDefinitionModel.MainCharacters = _SplittedValue.Trim();
            }

            // finally, the pairing
            Match _PairingRegexMatch = Regex.Match(aBookSummaryLine, "\\[(.+?)\\]");

            if (_PairingRegexMatch.Success && _PairingRegexMatch.Groups.Count == 2)
            {
                string _Pairings = $"[{_PairingRegexMatch.Groups[1].Value}]";
                while (!string.IsNullOrWhiteSpace(_Pairings))
                {
                    _PairingRegexMatch = _PairingRegexMatch.NextMatch();

                    if (!_PairingRegexMatch.Success || _PairingRegexMatch.Groups.Count != 2)
                        break;

                    _Pairings += $"[{_PairingRegexMatch.Groups[1].Value}]";
                }

                _RadiantReaderBookDefinitionModel.Pairings = _Pairings;
            }

            return _RadiantReaderBookDefinitionModel;
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
                aTitle = _TitleRegexMatch.Groups[1].Value;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<RadiantReaderBookDefinitionModel> ParseBooksFromFanfictionDOM(string aDOM)
        {
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

            return GenerateReaderBookDefinitionModelsFromFanfictionBookDOMDefinitions(_DOMBookDefinitions);
        }
    }
}
