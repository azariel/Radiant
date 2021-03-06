using System;
using System.IO;
using Radiant.Common.Utils.MiscUtils;

namespace Radiant.Common.Diagnostics
{
    public static class LoggingManager
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum LogVerbosity
        {
            Minimal,
            Verbose
        }

        // ********************************************************************
        //                            Constants
        // ********************************************************************
        public const string DEFAULT_LOG_FILE_RELATIVE_PATH = "radiant.log";

        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Format message and then log it to specified or default file
        /// </summary>
        public static void LogToFile(string aLogUID, string aLogContent, Exception aException = null, LogVerbosity aLogVerbosity = LogVerbosity.Minimal, string aLogFilePath = DEFAULT_LOG_FILE_RELATIVE_PATH)
        {
            string _Message = $"Message=[{aLogContent}]{Environment.NewLine}";

            if (aException != null)
                _Message += $"Exception=[{Environment.NewLine}{ExceptionUtils.BuildExceptionAndInnerExceptionsMessage(aException)}]{Environment.NewLine}";

            // Format message to add useful information
            _Message = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss.fff} - [{aLogUID}] {_Message}";

            File.AppendAllText(aLogFilePath, _Message);
        }
    }
}
