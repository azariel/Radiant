using Radiant.Common.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace RadiantReader
{
    internal class RadiantReaderFileLoader
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static bool LoadFile(string aFilePath, out List<Inline> aInlines)
        {
            aInlines = new List<Inline>();

            List<Inline> _Inlines = new();

            if (!File.Exists(aFilePath))
            {
                LoggingManager.LogToFile("ed197844-dd4f-4ea7-b815-33d9cc86c67b", $"Couldn't load file [{aFilePath}]. File was not found.");
                return false;
            }

            string[] _FileContentByRawLines = File.ReadAllLines(aFilePath);
            List<string> _Lines = new();

            foreach (string _RawLine in _FileContentByRawLines)
            {
                // Line could be splitted by other thing than \r\n, like <p></p> or </p> etc

            }

            foreach (string _Line in _Lines)
            {
                // Convert that line to an Inline

            }

            return true;
        }
    }
}
