using System;
using System.Diagnostics;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent;

namespace Radiant.WebScraper.RadiantWebScraper.Helpers
{
    public static class BrowserHelper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static void WaitForBrowserInputsReadyOrMax(int aMinMsToWait, Browser? aBrowser, int aMaxMsToWait = 60000)
        {
            Stopwatch _Stopwatch = new Stopwatch();
            _Stopwatch.Start();

            if (aBrowser.HasValue)
            {
                WaitForWebPageToFinishLoadingByBrowser(aBrowser.Value, (int)(aMaxMsToWait - _Stopwatch.ElapsedMilliseconds));

                if (_Stopwatch.ElapsedMilliseconds > aMaxMsToWait)
                    return;
            }

            int _MinMsToWait = (int)(aMinMsToWait - _Stopwatch.ElapsedMilliseconds);

            if (_MinMsToWait > 0)
                Thread.Sleep(_MinMsToWait);
        }

        public static bool WaitForWebPageToFinishLoadingByBrowser(Browser aSupportedBrowser, int aMaxMsToWait)
        {
            SupportedOperatingSystem _CurrentOS = OperatingSystemHelper.GetCurrentOperatingSystem();

            // NET5 doesn't support process under Linux atm
            if (_CurrentOS == SupportedOperatingSystem.Linux)
            {
                Thread.Sleep(aMaxMsToWait);
                return true;
            }

            LoggingManager.LogToFile("18e74850-afa9-4536-a1d4-121c818278df", $"Waiting for browser [{aSupportedBrowser}] to be ready...", aLogVerbosity:LoggingManager.LogVerbosity.Verbose);

            Stopwatch _Stopwatch = new();
            _Stopwatch.Start();
            try
            {
                Process[] _Processes = ScraperProcessHelper.GetProcessesAssociatedWithBrowser(aSupportedBrowser);

                // We want to wait until ONE instance of the browser is ready for input.
                // Modern browser uses multiples processes for multiples tabs, all the unfocused tabs are stuck until they are focused
                while (true)
                {
                    if (_Stopwatch.ElapsedMilliseconds > aMaxMsToWait)
                    {
                        LoggingManager.LogToFile("3e5c0255-feb5-4435-b629-1347235cce51", $"Browser [{aSupportedBrowser}] is still not ready after [{_Stopwatch.ElapsedMilliseconds}] ms. aborting.", aLogVerbosity:LoggingManager.LogVerbosity.Verbose);
                        return false;
                    }

                    foreach (Process _Process in _Processes)
                    {
                        // Force refresh the state before we query it 
                        _Process.Refresh();
                        
                        // Wait for this instance of the browser to be responsive to user input
                        if (!_Process.HasExited && !_Process.WaitForInputIdle(35))
                        {
                            Thread.Sleep(50);
                            continue;
                        }

                        LoggingManager.LogToFile("18e74850-afa9-4536-a1d4-121c818278df", $"Browser [{aSupportedBrowser}] is ready.", aLogVerbosity:LoggingManager.LogVerbosity.Verbose);

                        return true;
                    }
                }
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("0DB98514-B525-4012-ACC4-A3202FE4965C", "Something went wrong when waiting for webpage loading.", _Ex);

                int _NbMsToWait = aMaxMsToWait - (int)_Stopwatch.ElapsedMilliseconds;

                if (_NbMsToWait > 0)
                    Thread.Sleep(_NbMsToWait);
            }

            return true;
        }
    }
}
