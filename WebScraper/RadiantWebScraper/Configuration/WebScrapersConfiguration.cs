using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent;

namespace Radiant.WebScraper.RadiantWebScraper.Configuration
{
    public class WebScrapersConfiguration
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public SupportedBrowserConfiguration GetBrowserConfigurationBySupportedBrowser(Browser aSupportedBrowser)
        {
            SupportedBrowserConfiguration[] _MatchingConfigs = this.SupportedBrowsers?.Where(w => w.SupportedBrowser.Equals(aSupportedBrowser)).ToArray();

            if (_MatchingConfigs == null || _MatchingConfigs.Length <= 0)
            {
                LoggingManager.LogToFile("DAB57A35-D99F-417D-BE85-044B9B144792", $"No configuration found for {nameof(Browser)} [{aSupportedBrowser}].");
                return null;
            }

            SupportedOperatingSystem _CurrentOS = OperatingSystemHelper.GetCurrentOperatingSystem();
            switch(_CurrentOS)
            {
                case SupportedOperatingSystem.Linux:
                    return _MatchingConfigs.First();
                case SupportedOperatingSystem.Windows:
                    // Return first one that exists on disk
                    foreach (SupportedBrowserConfiguration _MatchingConfig in _MatchingConfigs)
                    {
                        if (File.Exists(_MatchingConfig.ExecutablePath))
                            return _MatchingConfig;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<SupportedBrowserConfiguration> SupportedBrowsers { get; set; } = new();
    }
}
