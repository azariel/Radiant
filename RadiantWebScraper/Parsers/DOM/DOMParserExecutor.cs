using System;
using System.Text.RegularExpressions;
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

            Regex _Regex = new Regex(aParserItem.RegexPattern);

            Match _Match = _Regex.Match(aDOM);
            if (!_Match.Success)
            {
                LoggingManager.LogToFile($"Couldn't match content. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                return null;
            }

            string _Value;
            switch (aParserItem.Target)
            {
                case DOMParserItem.DOMParserItemResultTarget.Value:
                    _Value = _Match.Value;
                    break;
                case DOMParserItem.DOMParserItemResultTarget.Group1Value:

                    if (_Match.Groups.Count < 2)
                    {
                        LoggingManager.LogToFile($"Couldn't match content. Was expecting at least 2 groups, but only [{_Match.Groups.Count}] found. Check ParserItem config - UID [{aParserItem.UID}]. Regex = [{aParserItem.RegexPattern}].");
                        return null;
                    }

                    _Value = _Match.Groups[1].Value;
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
