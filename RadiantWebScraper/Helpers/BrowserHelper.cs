using System.Diagnostics;

namespace Radiant.WebScraper.Helpers
{
    public static class BrowserHelper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static bool WaitForWebPageToFinishLoadingByBrowser(SupportedBrowser aSupportedBrowser, int aMaxMsToWait)
        {
            Process[] _Processes = ScraperProcessHelper.GetProcessesAssociatedWithBrowser(aSupportedBrowser);
            Stopwatch _Stopwatch = new Stopwatch();
            _Stopwatch.Start();

            // We want to wait until ONE instance of the browser is ready for input.
            // Modern browser uses multiples processes for multiples tabs, all the unfocused tabs are stuck until they are focused
            while (true)
            {
                if (_Stopwatch.ElapsedMilliseconds > aMaxMsToWait)
                    return false;

                foreach (Process _Process in _Processes)
                {
                    // Force refresh the state before we query it 
                    _Process.Refresh();

                    // Wait for this instance of the browser to be responsive to user input
                    if (!_Process.WaitForInputIdle(35))
                        continue;

                    return true;
                }
            }
        }
    }
}
