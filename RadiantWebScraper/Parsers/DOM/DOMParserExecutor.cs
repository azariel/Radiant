using System;
using System.Linq;
using System.Text.RegularExpressions;
using Radiant.Common.Business;
using Radiant.Common.Diagnostics;

namespace Radiant.WebScraper.Parsers.DOM
{
    public static class DOMParserExecutor
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string Execute(string aUrl, string aDOM, DOMParserItem aParserItem)
        {
            if (!aUrl.ToLowerInvariant().Contains(aParserItem.IfUrlContains.ToLowerInvariant()))
                return null;

            var _Regex = new Regex(aParserItem.RegexPattern);

            MatchCollection _Matches = _Regex.Matches(aDOM);
            if (_Matches.Count <= 0)
            {
                LoggingManager.LogToFile("95276771-4C9B-47FD-A2ED-1F3F2E707F9D", $"Couldn't match content. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                return null;
            }

            Match _SelectedMatch = aParserItem.RegexMatch switch
            {
                RegexItemResultMatch.First => _Matches.First(),
                RegexItemResultMatch.Last => _Matches.Last(),
                _ => throw new Exception($"{nameof(RegexItemResultMatch)} value [{aParserItem.RegexMatch}] is unhandled.")
            };

            string _Value;
            switch (aParserItem.Target)
            {
                case RegexItemResultTarget.Value:
                    _Value = _SelectedMatch.Value;
                    break;
                case RegexItemResultTarget.Group0Value:

                    if (_SelectedMatch.Groups.Count < 1)
                    {
                        LoggingManager.LogToFile("3107FB22-8575-496C-9DB5-313E0E583CB5", $"Couldn't match content. Was expecting at least 1 group, but only [{_SelectedMatch.Groups.Count}] found. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                        return null;
                    }

                    _Value = _SelectedMatch.Groups[0].Value;
                    break;
                case RegexItemResultTarget.Group1Value:

                    if (_SelectedMatch.Groups.Count < 2)
                    {
                        LoggingManager.LogToFile("50C3D9BA-EE2A-4861-A7DB-BE724C4D99A4", $"Couldn't match content. Was expecting at least 2 groups, but only [{_SelectedMatch.Groups.Count}] found. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                        return null;
                    }

                    _Value = _SelectedMatch.Groups[1].Value;
                    break;
                case RegexItemResultTarget.LastGroupValue:

                    if (_SelectedMatch.Groups.Count < 1)
                    {
                        LoggingManager.LogToFile("78A366DE-8BC3-42D1-97D8-76788128160C", $"Couldn't match content. Was expecting at least 1 group, but only [{_SelectedMatch.Groups.Count}] found. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                        return null;
                    }

                    _Value = _SelectedMatch.Groups[^1].Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{aParserItem.Target} is unhandled.");
            }

            if (aParserItem.SubParserToExecuteOnTargetValue != null)
                _Value = Execute(aUrl, _Value, aParserItem.SubParserToExecuteOnTargetValue);

            return _Value;
        }
    }
}
