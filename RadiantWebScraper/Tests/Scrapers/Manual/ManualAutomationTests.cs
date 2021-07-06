using System.Drawing;
using RadiantInputsManager;
using Xunit;

namespace Radiant.WebScraper.Tests.Scrapers.Manual
{
    public class ManualAutomationTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void TestMoveCursorToLocation()
        {
            // Assert that it doesn't throw
            ManualAutomation.MoveCursorToLocation(new Point(274,255));
        }
    }
}
