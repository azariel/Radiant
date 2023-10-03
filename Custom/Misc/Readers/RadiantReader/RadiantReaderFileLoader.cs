using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReader.Utils;

namespace Radiant.Custom.Readers.RadiantReader
{
    internal class RadiantReaderFileLoader
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static bool LoadFile(string aFilePath, out List<Inline> aInlines)
        {
            aInlines = new List<Inline>();

            if (!File.Exists(aFilePath))
            {
                LoggingManager.LogToFile("ed197844-dd4f-4ea7-b815-33d9cc86c67b", $"Couldn't load file [{aFilePath}]. File was not found.");
                return false;
            }

            string[] _FileContentByRawLines = File.ReadAllLines(aFilePath);
            List<string> _Lines = new();

            foreach (string _RawLine in _FileContentByRawLines)
            {
                // Line could be split by other thing than \r\n, like <p></p> or </p> etc
                if (_RawLine.StartsWith("<?xml", System.StringComparison.InvariantCultureIgnoreCase))
                    continue;

                _Lines.Add(_RawLine);// TEMP
            }

            string _MergedContent = string.Join("", _Lines);
            aInlines.AddRange(StringConvertUtils.GetInlinesFromString(_MergedContent));

            // Convert lines to Inlines
            //foreach (string _Line in _Lines)
            //    aInlines.AddRange(StringConvertUtils.GetInlinesFromString(_Line));

            return true;
        }
    }
}
