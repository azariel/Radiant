using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Radiant.Common.Diagnostics;
using Radiant.Common.Serialization;
using Sylvan.Data.Csv;
using Sylvan.Data.Excel;

namespace Radiant.Common.Utils
{
    public static class CsvUtils
    {
        public static string[] LoadFile(string filePath, int skipNbHeadersLines = 0)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            // Skip header lines if required
            string[] fileLines = File.ReadAllLines(filePath).Skip(skipNbHeadersLines).ToArray();

            return fileLines;
        }

        public static string ConvertExcelFileToCsv(string excelFilePath, bool deleteOriginalFile = true)
        {
            if (string.IsNullOrWhiteSpace(excelFilePath))
                return null;

            string outputFilePath = $"{Path.Combine(Path.GetDirectoryName(excelFilePath), Path.GetFileNameWithoutExtension(excelFilePath))}.csv";

            using (var excelReader = ExcelDataReader.Create(excelFilePath))
            {
                using (var csvWriter = CsvDataWriter.Create(outputFilePath, new CsvDataWriterOptions
                {
                    Delimiter = ';'
                }))
                {
                    csvWriter.Write(excelReader);
                }
            }

            if (!File.Exists(outputFilePath))
            {
                LoggingManager.LogToFile("48d50cb8-216a-423f-a37a-7f35a41d3765", $"Couldn't convert excel file [{excelFilePath}] to csv file.");
                return null;
            }

            if (deleteOriginalFile)
                File.Delete(excelFilePath);

            return outputFilePath;
        }

        public static LineTypeObject[] LoadFile<LineTypeObject>(string filePath, Func<string, LineTypeObject> rowParserFunc, int skipNbHeadersLines = 0) where LineTypeObject : new()
        {
            string[] fileLines = LoadFile(filePath, skipNbHeadersLines);

            if (fileLines == null || fileLines.Length <= 0)
            {
                return null;
            }

            List<LineTypeObject> typedObjectsCollection = new();
            foreach (string line in fileLines)
            {
                LineTypeObject typedObject;

                try
                {
                    typedObject = rowParserFunc.Invoke(line);
                } catch (Exception _Ex)
                {
                    LoggingManager.LogToFile("EDB55407-59F4-41AD-87A9-D5B251EA0FF6", $"Couldn't parse line [{line}] from CSV file [{filePath}] using provided func.", _Ex);
                    return null;
                }

                typedObjectsCollection.Add(typedObject);
            }

            return typedObjectsCollection.ToArray();
        }

        public static LineTypeObject[] LoadFileEmbeddingJsonObjects<LineTypeObject>(string filePath, int skipNbHeadersLines = 0)
        {
            string[] fileLines = LoadFile(filePath, skipNbHeadersLines);

            if (fileLines == null || fileLines.Length <= 0)
            {
                return null;
            }

            List<LineTypeObject> typedObjectsCollection = new();
            foreach (string line in fileLines)
            {
                LineTypeObject typedObject;

                try
                {
                    typedObject = JsonCommonSerializer.DeserializeFromString<LineTypeObject>(line);
                } catch (Exception)
                {
                    // error was already logged by JsonCommonSerializer throw
                    return null;
                }

                typedObjectsCollection.Add(typedObject);
            }

            return typedObjectsCollection.ToArray();
        }
    }
}
