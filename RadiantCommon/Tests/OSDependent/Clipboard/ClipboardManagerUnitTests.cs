using Radiant.Common.OSDependent.Clipboard;
using Xunit;

namespace Radiant.Common.Tests.OSDependent.Clipboard
{
    public class ClipboardManagerUnitTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicClipboardInteractions()
        {
            string _ClipboardValue = "UnitTest-1B7E470D-28BA-403F-B8FF-001A045959A6";
            ClipboardManager.SetClipboardValue(_ClipboardValue);

            string _ClipboardValueFromManaged = ClipboardManager.GetClipboardValue();

            Assert.Equal(_ClipboardValue, _ClipboardValueFromManaged);
        }
    }
}
