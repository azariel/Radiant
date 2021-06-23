using System;
using System.IO;
using Radiant.Common.Helpers.MiscHelpers;

namespace Radiant.Common.Diagnostics
{
    public static class LoggingManager
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        public const string DEFAULT_LOG_FILE_RELATIVE_PATH = "C:/temp/radiant.default.log";

        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Format message and then log it to specified or default file
        /// </summary>
        public static void LogToFile(string aLogContent, Exception aException = null, string aLogFilePath = DEFAULT_LOG_FILE_RELATIVE_PATH)
        {
            string _Message = $"Message=[{aLogContent}]{Environment.NewLine}";

            if (aException != null)
                _Message += $"Exception=[{Environment.NewLine}{ExceptionHelper.BuildExceptionAndInnerExceptionsMessage(aException)}]{Environment.NewLine}";

            // Format message to add useful information
            _Message = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss.fff} - {_Message}";

            // IO operations
            if (!Directory.Exists(Path.GetDirectoryName(aLogFilePath)))
                Directory.CreateDirectory(aLogFilePath);

            File.WriteAllText(aLogFilePath, _Message);
        }
    }
}
