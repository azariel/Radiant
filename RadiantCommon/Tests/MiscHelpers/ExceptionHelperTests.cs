using System;
using Radiant.Common.Utils.MiscUtils;
using Xunit;

namespace Radiant.Common.Tests.MiscHelpers
{
    public class ExceptionHelperTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestBuildExceptionAndInnerExceptionsMessage()
        {
            Exception _TestException = new("Level-1", new Exception("Level 2", new Exception("Level_3")));

            string _GeneratedMessage = ExceptionUtils.BuildExceptionAndInnerExceptionsMessage(_TestException);
            const string _HARD_CODED_REFERENCE_VALUE =
              @" [
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
";

            Assert.Equal(_HARD_CODED_REFERENCE_VALUE, _GeneratedMessage);
        }
    }
}
