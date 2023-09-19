using RadiantReader.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RadiantReader.Utils
{
    public static class AlternativeBookContentHelper
    {
        private static Regex fDigitsRegex = new("\\d+");

        public static string FindAlternativeChapterWithOffset(int aOffset)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            string _FilePath = _Config.State.SelectedBook.AlternativeBookPathOnDisk;

            string _BaseValue = GetAlternativeChapterIndex(0);
            string _Value = GetAlternativeChapterIndex(aOffset);

            if (_Value == null)
                return null;

            int _IndexOfLastMatch = _FilePath.LastIndexOf(_BaseValue, StringComparison.InvariantCultureIgnoreCase);

            if (_IndexOfLastMatch == -1)
                return null;

            var _Result = _FilePath.Remove(_IndexOfLastMatch, _BaseValue.Length).Insert(_IndexOfLastMatch, _Value);

            if (!File.Exists(_Result))
                return null;

            return _Result;
        }

        public static string GetAlternativeChapterIndex(int aOffset)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            string _FilePath = _Config.State.SelectedBook.AlternativeBookPathOnDisk;

            var _DigitsMatches = fDigitsRegex.Matches(_FilePath);

            if (!_DigitsMatches.Any())
                return null;

            var _DigitsMatch = _DigitsMatches.LastOrDefault();

            if (_DigitsMatch == null)
                return null;

            string _Value = _DigitsMatch.Captures.LastOrDefault()?.Value;
            if (!int.TryParse(_Value, out int _IntValue))
                return null;

            string _ChapterOffsettedIndexValue = $"{_IntValue + aOffset}";

            // Keep leading "0" in filename if there was any
            var _NbLeadingZerosRaw = _Value.TakeWhile(t => t == '0').Count();
            if (_NbLeadingZerosRaw > 0)
                _NbLeadingZerosRaw = _Value.Length;

            //int _NbLeadingZerosToRemove = _ChapterIndexValue.Length - _IntValue.ToString().Length;//If we change units (unit vs deca vs centa)
            string _ValueWithLeadingZeros = _ChapterOffsettedIndexValue.PadLeft(_NbLeadingZerosRaw, '0');

            return _ValueWithLeadingZeros;
        }
    }
}
