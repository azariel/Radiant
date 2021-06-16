using System.Collections.Generic;
using System.Linq;
using Radiant.Common.Diagnostics;

namespace Radiant.WebScraper.Configuration
{
    public class WebScrapersConfiguration
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public SupportedBrowserConfiguration GetBrowserConfigurationBySupportedBrowser(SupportedBrowser aSupportedBrowser)
        {
            SupportedBrowserConfiguration[] _MatchingConfigs = this.SupportedBrowsers?.Where(w => w.SupportedBrowser.Equals(aSupportedBrowser)).ToArray();

            if (_MatchingConfigs == null || _MatchingConfigs.Length <= 0)
            {
                LoggingManager.LogToFile($"No configuration found for {nameof(SupportedBrowser)} [{aSupportedBrowser}].");
                return null;
            }

            if (_MatchingConfigs.Length > 1)
            {
                LoggingManager.LogToFile($"Multiple- configurations found for {nameof(SupportedBrowser)} [{aSupportedBrowser}].");
                return null;
            }

            return _MatchingConfigs.Single();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<SupportedBrowserConfiguration> SupportedBrowsers { get; set; } = new();
    }
}
