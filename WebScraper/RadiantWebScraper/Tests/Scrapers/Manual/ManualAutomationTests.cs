using System.Drawing;
using System.Threading;
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
            ManualAutomation.MoveCursorToLocation(new Point(274, 255));
        }

        [Fact]
        public void TestExtendedKeys1()
        {
            Thread.Sleep(5000);

            // Assert that it doesn't throw
            ManualAutomation.KeyboardExecute(new[]
            {
                Keycode.XK_Shift_L,
                Keycode.XK_Right
            });

            ManualAutomation.KeyboardExecute(new[]
            {
                Keycode.XK_Control_L,
                Keycode.XK_c
            });
        }

        [Fact]
        public void TestExtendedKeys2()
        {
            Thread.Sleep(5000);
            // Assert that it doesn't throw
            ManualAutomation.KeyboardExecute(new[]
            {
                Keycode.XK_Shift_L,
                Keycode.XK_Home
            });

            ManualAutomation.KeyboardExecute(new[]
            {
                Keycode.XK_Control_L,
                Keycode.XK_c
            });
        }
    }
}
