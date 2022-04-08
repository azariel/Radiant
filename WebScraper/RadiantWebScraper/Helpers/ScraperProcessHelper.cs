using System;
using System.Diagnostics;
using System.Linq;

namespace Radiant.WebScraper.Helpers
{
    public static class ScraperProcessHelper
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static Process[] GetProcessesAssociatedWithBrowser(Browser aBrowserToKill)
        {
            var _Processes = Process.GetProcesses().Where(w => string.Equals(w.ProcessName, aBrowserToKill.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToArray();

            return _Processes;
        }
    }
}
