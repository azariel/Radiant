using System;
using System.IO;
using Radiant.Common.Diagnostics;
using Xunit;

namespace Radiant.Common.Tests.Diagnostics
{
    public class LoggingManagerTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Just a very basic test to try to log to default file.
        /// </summary>
        [Fact]
        public void TestLogToFileDefaultBasic()
        {
            // Delete old file log
            if (File.Exists(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH))
                File.Delete(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);

            string _Message = "test-E80B2ECB-AEDA-46A9-AD05-1BFBF73F4189";
            LoggingManager.LogToFile("A02AFC5D-708B-4E25-ACCC-91F890CEDE79", _Message);

            // Assert file exists
            Assert.True(File.Exists(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH));

            string _LogContent = File.ReadAllText(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);
            Assert.Contains($"Message=[{_Message}]{Environment.NewLine}", _LogContent);

            // Clean
            File.Delete(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);
        }

        /// <summary>
        /// Just a very basic test to try to log to default file.
        /// </summary>
        [Fact]
        public void TestLogToFileDefaultWithExceptionBasic()
        {
            // Delete old file log
            if (File.Exists(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH))
                File.Delete(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);

            string _Message = "test-B5A84F8E-C071-4ABE-B1E3-DED82182ADA6";
            LoggingManager.LogToFile("A8C05C85-7AC4-42D8-BB4A-65F06FE44DA1", _Message, new Exception("Level-1", new Exception("Level 2", new Exception("Level_3"))));

            // Assert file exists
            Assert.True(File.Exists(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH));

            string _LogContent = File.ReadAllText(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);

            const string _HARD_CODED_REFERENCE_VALUE = @"Message=[test-B5A84F8E-C071-4ABE-B1E3-DED82182ADA6]
Exception=[
 [
  Message=[Level-1]
  StackTrace=[]
 ]
 InnerException=
 [
  [
   Message=[Level 2]
   StackTrace=[]
  ]
  InnerException=
  [
   [
    Message=[Level_3]
    StackTrace=[]
   ]
  ]
 ]
]";
            Assert.Contains(_HARD_CODED_REFERENCE_VALUE, _LogContent);

            // Clean
            File.Delete(LoggingManager.DEFAULT_LOG_FILE_RELATIVE_PATH);
        }
    }
}
