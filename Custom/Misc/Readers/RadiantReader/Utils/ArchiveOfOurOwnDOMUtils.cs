using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Radiant.Common.Diagnostics;
using RadiantReader.DataBase;

namespace RadiantReader.Utils
{
    public static class ArchiveOfOurOwnDOMUtils
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public class ArchiveOfOurOwnBookDOMDefinition
        {
            public string BookInformationLine { get; set; }
            public string BookNbWordsLine { get; set; }
            public string BookSummaryLine { get; set; }
            public string BookTitleAndUrl { get; set; }
            public string BookRatingLine { get; set; }
            public string[] Pairings { get; set; }
        }

        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const string REGEX_RATING_BOOK_LINE = "<span.*rating\" title=\"(.*?)\"";
        private const string REGEX_NB_WORDS_BOOK_LINE = "<dd class=\"words\">(.*?)</dd>";
        private const string REGEX_SUMMARY_BOOK_LINE = "<blockquote.*?summary.*?\">.*?(<p>.*</p>).*?</blockquote>";
        private const string REGEX_TITLE_AND_URL_BOOK_LINE = "(<a href=\"/works/.*)/a>";
        private const string REGEX_PAIRINGS_BOOK_LINE = "<a class=\"tag\" .*?>(.*?)<";

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static List<RadiantReaderBookDefinitionModel> GenerateReaderBookDefinitionModelsFromBookDOMDefinitions(List<ArchiveOfOurOwnBookDOMDefinition> aDomBookDefinitions)
        {
            List<RadiantReaderBookDefinitionModel> _RadiantReaderBookDefinitionModels = new();
            foreach (ArchiveOfOurOwnBookDOMDefinition _BookDomDefinition in aDomBookDefinitions)
            {
                RadiantReaderBookDefinitionModel _RadiantReaderBookDefinitionModel = new();

                GetUrlAndTitleFromArchiveOfOurOwnTitleAndUrlLine(_BookDomDefinition.BookTitleAndUrl, out string _Url, out string _Title);
                _RadiantReaderBookDefinitionModel.Url = _Url;
                _RadiantReaderBookDefinitionModel.Title = _Title;

                _RadiantReaderBookDefinitionModel.Summary = _BookDomDefinition.BookSummaryLine.Replace("â€™", "'");

                string _NbWords = _BookDomDefinition.BookNbWordsLine.Replace(",", "").Replace(".", "");
                if (int.TryParse(_NbWords, out int _NbWordsValue))
                    _RadiantReaderBookDefinitionModel.SoftNbWords = _NbWordsValue;

                _RadiantReaderBookDefinitionModel.Rating = _BookDomDefinition.BookRatingLine;
                _RadiantReaderBookDefinitionModel.Pairings = string.Join(", ", _BookDomDefinition.Pairings);

                _RadiantReaderBookDefinitionModels.Add(_RadiantReaderBookDefinitionModel);
            }

            return _RadiantReaderBookDefinitionModels;
        }

        private static void GetUrlAndTitleFromArchiveOfOurOwnTitleAndUrlLine(string aBookInformationLine, out string aUrl, out string aTitle)
        {
            aUrl = null;
            aTitle = null;

            // Extract Url
            var _UrlRegexMatch = Regex.Match(aBookInformationLine, "<a href=\"(.*)\"");
            if (_UrlRegexMatch.Success && _UrlRegexMatch.Groups.Count == 2)
                aUrl = _UrlRegexMatch.Groups[groupnum: 1].Value;

            // Extract Title
            var _TitleRegexMatch = Regex.Match(aBookInformationLine, ">(.*)<");
            if (_TitleRegexMatch.Success && _TitleRegexMatch.Groups.Count == 2)
                aTitle = BookStringUtils.FormatTitle(_TitleRegexMatch.Groups[groupnum: 1].Value);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        //public static RadiantReaderBookChapter ParseBookChapterFromDOM(string aDom, int aChapterIndex, long aBookDefinitionId)
        //{
        //    if (string.IsNullOrWhiteSpace(aDom))
        //    {
        //        LoggingManager.LogToFile("48a2aa90-05b7-4b71-aed9-37fe850b16fa", "DOM was empty.");
        //        throw new Exception("26c9069d-2f45-4ac3-b405-6a68b30821ba_DOM was empty.");
        //    }

        //    RadiantReaderBookChapter _Chapter = new();

        //    Regex _ChapterContentRegex = new Regex("id=\"storytext\">(.+?)</div>");// until next div

        //    var _MatchContent = _ChapterContentRegex.Match(aDom);
        //    if (!_MatchContent.Success)
        //    {
        //        _ChapterContentRegex = new Regex("id=\"storytext\">(.+?)$", RegexOptions.Multiline);// Just until end of line

        //        _MatchContent = _ChapterContentRegex.Match(aDom);
        //        if (!_MatchContent.Success)
        //        {
        //            LoggingManager.LogToFile("30a0e9ff-3ad0-40fc-be36-4f2cf2292cc0", "Couldn't match ArchiveOfOurOwn book content");
        //            throw new Exception("203dd19b-d832-4d00-aa28-39fe05364a23_Couldn't match book content.");
        //        }
        //    }

        //    _Chapter.ChapterContent = BookStringUtils.FormatSummary(_MatchContent.Groups.Values.Last().Value);
        //    _Chapter.BookDefinitionId = aBookDefinitionId;
        //    _Chapter.ChapterNumber = aChapterIndex;

        //    _Chapter.ChapterWordsCount = _Chapter.ChapterContent.Split(" ").Length - 1 +
        //                                 _Chapter.ChapterContent.Split("'").Length - 1 +
        //                                 _Chapter.ChapterContent.Split("</p").Length - 1 +
        //                                 1;// 0 based

        //    return _Chapter;
        //}

        public static RadiantReaderBookChapter ParseBookChapterFromDOM(string aDom, int aChapterIndex, long aBookDefinitionId)
        {
            if (string.IsNullOrWhiteSpace(aDom))
            {
                LoggingManager.LogToFile("20c0c24d-8f7b-434a-bec3-fe999e639ad0", "DOM was empty.");
                throw new Exception("da5e2d7d-c69d-4be0-8d47-24280a3dc75a_DOM was empty.");
            }

            RadiantReaderBookChapter _Chapter = new();

            string _DomSingleLine = aDom.Replace("\r\n", "");
            Regex _ChapterContentRegex = new Regex("<!--main content-->.*</h3>(.*)</div>  <!--/main-->");

            var _MatchContent = _ChapterContentRegex.Match(_DomSingleLine);
            if (!_MatchContent.Success)
            {
                LoggingManager.LogToFile("93b7fae2-a168-471c-85d5-d294f7e10c06", "Couldn't match ArchiveOfOurOwn book content");
                    throw new Exception("97214613-e5a4-405e-b696-ea0675a39b04_Couldn't match book content.");
            }

            _Chapter.ChapterContent = BookStringUtils.FormatSummary(_MatchContent.Groups.Values.Last().Value).Trim();
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
            // TODO: ParseBooksFromArchiveOfOurOwnDOM should return it's own model and not RadiantReaderBookDefinitionModel
            List<ArchiveOfOurOwnBookDOMDefinition> _DOMBookDefinitions = new();

            string[] _Lines = aDOM.Split("<div class=\"header module\">");

            // Skip 1st
            for (int i = 1; i < _Lines.Length; i++)
            {
                // Check if current line is a book titleAndUrl line, a book summary line, a book informations line or a line to discard
                // Title and Url
                Match _MatchTitleAndUrlLine = Regex.Match(_Lines[i], REGEX_TITLE_AND_URL_BOOK_LINE, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                if (!_MatchTitleAndUrlLine.Success)
                    continue;// discard book definition

                // Summary
                Match _MatchSummaryLine = Regex.Match(_Lines[i], REGEX_SUMMARY_BOOK_LINE, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (!_MatchSummaryLine.Success)
                {
                    LoggingManager.LogToFile("8dcd1333-331e-4e0b-9525-ab24e198ee0c", $"Couldn't find summary at line [{i}]. Raw line = [{_Lines[i]}].");
                    continue;
                }

                // Soft Nb Words
                Match _MatchSoftNbWordsLine = Regex.Match(_Lines[i], REGEX_NB_WORDS_BOOK_LINE, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (!_MatchSoftNbWordsLine.Success)
                {
                    LoggingManager.LogToFile("89a2a111-0359-4c7f-bb41-aaca65309083", $"Couldn't find nb words at line [{i}]. Raw line = [{_Lines[i]}].");
                    continue;
                }

                // Rating
                Match _MatchRatingLine = Regex.Match(_Lines[i], REGEX_RATING_BOOK_LINE, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (!_MatchRatingLine.Success)
                {
                    LoggingManager.LogToFile("06bcbfa2-15ab-48ca-b4d0-aa3aaa12f16a", $"Couldn't find rating at line [{i}]. Raw line = [{_Lines[i]}].");
                    continue;
                }

                // Rating
                var _MatchPairings = Regex.Matches(_Lines[i], REGEX_PAIRINGS_BOOK_LINE, RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (_MatchPairings.Count <= 0)
                {
                    LoggingManager.LogToFile("529d96c0-9eb6-410a-8ca3-e6a4ab0b545f", $"Couldn't find pairings at line [{i}]. Raw line = [{_Lines[i]}].");
                    continue;
                }

                string[] _PairingElements = _MatchPairings.Where(w => w.Success && w.Groups.Values.Last().Value.Contains("/")).Select(s => s.Groups.Values.Last().Value).ToArray();

                _DOMBookDefinitions.Add(new ArchiveOfOurOwnBookDOMDefinition
                {
                    BookTitleAndUrl = _MatchTitleAndUrlLine.Groups.Values.Last().Value,
                    BookSummaryLine = BookStringUtils.FormatSummary(_MatchSummaryLine.Groups.Values.Last().Value),
                    BookNbWordsLine = _MatchSoftNbWordsLine.Groups.Values.Last().Value,
                    BookRatingLine = _MatchRatingLine.Groups.Values.Last().Value,
                    Pairings = _PairingElements,
                    BookInformationLine = _Lines[i]
                });
            }

            return GenerateReaderBookDefinitionModelsFromBookDOMDefinitions(_DOMBookDefinitions);
        }
    }
}
