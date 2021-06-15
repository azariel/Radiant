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
            ManualAutomation.MoveCursorToLocation(new Point(274,255));
        }
    }
}
