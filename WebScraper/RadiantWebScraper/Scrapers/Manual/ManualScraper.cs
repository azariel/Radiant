using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Configuration;
using Radiant.WebScraper.Helpers;
using Radiant.WebScraper.Parsers.DOM;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Scrapers.Manual
{
    /// <summary>
    /// Manual scraper is a very little special tool. It reproduce user inputs to physically go to the website and scrap
    /// manually the source or the data it wants. It's often a good way to avoid bot detection, but it's painfully slow,
    /// among other things.. So we often consider it to be "the last stand" to fetch the data and should only be used on
    /// a dedicated server..
    /// </summary>
    public class ManualScraper : IScraper
    {
        // ********************************************************************
        //                            Constants
        // ********************************************************************
        private const int NB_MS_WAIT_FOR_INPUT_HANG = 60000;

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private string fBrowserProcessName;

        private void CloseCurrentTab()
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 290,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_w
                }
            });
        }

        private void ExecuteFullScreenF11()
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                KeyStrokeCodes = new[] { Keycode.XK_F11 }
            });
        }

        private void KillBrowserProcess(Browser aBrowserToKill)
        {
            Process[] _ProcessesToKill = ScraperProcessHelper.GetProcessesAssociatedWithBrowser(aBrowserToKill);

            foreach (Process _Process in _ProcessesToKill)
                _Process.Kill();
        }

        private bool StartBrowser(Browser aSupportedBrowser, string aDefaultUrl, bool aUseDefaultBrowserAsFallback)
        {
            var _WebScraperConfiguration = WebScraperConfigurationManager.ReloadConfig();

            SupportedBrowserConfiguration _SupportedBrowserConfiguration = _WebScraperConfiguration.GetBrowserConfigurationBySupportedBrowser(aSupportedBrowser);

            if (_SupportedBrowserConfiguration == null)
            {
                LoggingManager.LogToFile("B8C37FE6-707E-4BCA-8644-EDAB3A589DF7", $"Browser [{aSupportedBrowser}] has no valid configuration.");

                if (!aUseDefaultBrowserAsFallback)
                    return false;

                fBrowserProcessName = null;

            } else
                fBrowserProcessName = _SupportedBrowserConfiguration.ExecutablePath;

            if (_SupportedBrowserConfiguration != null)
            {
                SupportedOperatingSystem _CurrentOS = OperatingSystemHelper.GetCurrentOperatingSystem();

                switch (_CurrentOS)
                {
                    case SupportedOperatingSystem.Linux:
                        fBrowserProcessName = _SupportedBrowserConfiguration.LinuxCommand;
                        break;
                    case SupportedOperatingSystem.Windows:
                        fBrowserProcessName = _SupportedBrowserConfiguration.ExecutablePath;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var _StartInfo = new ProcessStartInfo
            {
                Arguments = $"{aDefaultUrl}",
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Maximized // not really working for browser ?...meh
            };

            if (!string.IsNullOrWhiteSpace(fBrowserProcessName))
                _StartInfo.FileName = fBrowserProcessName;

            var _Process = new Process { StartInfo = _StartInfo };
            _Process.Start();

            return true;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void GetTargetValueFromUrl(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            Thread.Sleep(500);

            // First thing is to get to the webpage
            // For the duration of the function, we're taking exclusivity of inputs to avoid conflicts
            InputsManager.ExecuteInputsWithExclusivity(() =>
            {
                // Close all tabs to avoid "browser ready" state issue
                CloseAllTabs(aSupportedBrowser, aTarget);

                // Re-open browser
                if (!StartBrowser(aSupportedBrowser, aUrl, false))
                {
                    LoggingManager.LogToFile("311264B0-5EEC-41CA-9F67-1B085ECFB366", $"Couldn't start browser [{aSupportedBrowser}]. Aborting {nameof(GetTargetValueFromUrl)} from URL [{aUrl}].");
                    return;
                }
                if (!BrowserHelper.WaitForWebPageToFinishLoadingByBrowser(aSupportedBrowser, NB_MS_WAIT_FOR_INPUT_HANG))
                {
                    LoggingManager.LogToFile("3A4B102B-E437-4C1B-90AA-EC1FCF3669B4", $"Couldn't wait for browser [{aSupportedBrowser}]. It may be stuck. Aborting [{nameof(GetTargetValueFromUrl)}] Target was [{aTarget}].");
                    return;
                }
                
                // Wait a little longer just in case the system is a little slow (like a raspberry pi for instance)
                var _WebScraperConfiguration = WebScraperConfigurationManager.ReloadConfig();
                SupportedBrowserConfiguration _SupportedBrowserConfiguration = _WebScraperConfiguration.GetBrowserConfigurationBySupportedBrowser(aSupportedBrowser);
                Thread.Sleep(_SupportedBrowserConfiguration?.NbMsToWaitOnBrowserStart ?? 30000);

                Thread.Sleep(500);

                // Fullscreen
                ExecuteFullScreenF11();
                Thread.Sleep(500);

                // Evaluate the target and get the value
                aTarget.Evaluate(aSupportedBrowser, aUrl, true, aManualScraperItems?.OfType<ManualScraperItemParser>().Select(s=>(IScraperItemParser)s).ToList(), aDOMParserItems);
                Thread.Sleep(500);

                // "Closing" sequence
                // Exit F11
                ExecuteFullScreenF11();
                Thread.Sleep(500);

                // Close the tab we created on the first step
                CloseCurrentTab();
                Thread.Sleep(500);

                // Note: actually, we already closed our tab.. so.. no, don't kill the browser..
            });
        }

        private void CloseAllTabs(Browser aSupportedBrowser, IScraperTarget aTarget)
        {
            string _Url = "www.google.com";
            // Re-open browser
            if (!StartBrowser(aSupportedBrowser, _Url, false))
            {
                LoggingManager.LogToFile("D5288A56-B37B-4F6C-B691-A2B738F92A1E", $"Couldn't start browser [{aSupportedBrowser}]. Aborting {nameof(GetTargetValueFromUrl)} from URL [{_Url}].");
                return;
            }
            if (!BrowserHelper.WaitForWebPageToFinishLoadingByBrowser(aSupportedBrowser, NB_MS_WAIT_FOR_INPUT_HANG))
            {
                LoggingManager.LogToFile("238B040E-7FC0-456D-ADCD-0EF3B6C2D98C", $"Couldn't wait for browser [{aSupportedBrowser}]. It may be stuck. Aborting [{nameof(GetTargetValueFromUrl)}] Target was [{aTarget}].");
                return;
            }

            // Wait a little longer just in case the system is a little slow (like a raspberry pi for instance)
            var _WebScraperConfiguration = WebScraperConfigurationManager.ReloadConfig();
            SupportedBrowserConfiguration _SupportedBrowserConfiguration = _WebScraperConfiguration.GetBrowserConfigurationBySupportedBrowser(aSupportedBrowser);
            Thread.Sleep(_SupportedBrowserConfiguration?.NbMsToWaitOnBrowserStart ?? 30000);

            var _Stopwatch = new Stopwatch();
            _Stopwatch.Start();
            while (true)
            {
                // Check if process still running
                Process[] _ProcessesToKill = ScraperProcessHelper.GetProcessesAssociatedWithBrowser(aSupportedBrowser);

                if (!_ProcessesToKill.Any())
                    break;

                Thread.Sleep(1000);
                CloseCurrentTab();
                Thread.Sleep(5000);

                if (_Stopwatch.Elapsed.TotalHours > 1)
                {
                    LoggingManager.LogToFile("5F3A2CE4-C4A8-42B9-828E-86F2BC7FDA45", "Process to kill browser tabs was stuck.");
                    throw new Exception("Process to kill browser tabs was stuck.");
                }
            }
        }
    }
}
