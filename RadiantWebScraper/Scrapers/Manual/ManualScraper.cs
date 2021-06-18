using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.WebScraper.Configuration;
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
        private const int NB_MS_WAIT_FOR_INPUT_HANG = 120000;

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
                    Keycode.CtrlL,
                    Keycode.XK_w
                }
            });
        }

        private void CopyAllToClipboard(int aDelayBetweenSelectAllAndCopyToClipboardInMs = 500)
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 160,
                KeyStrokeCodes = new[]
                {
                    Keycode.CtrlL,
                    Keycode.XK_a
                }
            });

            Thread.Sleep(aDelayBetweenSelectAllAndCopyToClipboardInMs);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 260,
                KeyStrokeCodes = new[]
                {
                    Keycode.CtrlL,
                    Keycode.XK_c
                }
            });

            Thread.Sleep(100);
        }

        private void ExecuteFullScreenF11()
        {
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                KeyStrokeCodes = new[] { Keycode.XK_F11 }
            });
        }

        private Process[] GetProcessesAssociatedWithBrowser(SupportedBrowser aBrowserToKill)
        {
            var _Processes = Process.GetProcesses().Where(w => string.Equals(w.ProcessName, aBrowserToKill.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToArray();

            return _Processes;
        }

        private void KillBrowserProcess(SupportedBrowser aBrowserToKill)
        {
            Process[] _ProcessesToKill = GetProcessesAssociatedWithBrowser(aBrowserToKill);

            foreach (Process _Process in _ProcessesToKill)
                _Process.Kill();
        }

        private void ShowDOMInNewTab()
        {
            // Note that it's the same in every supported browser (atm)
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.CtrlL,
                    Keycode.XK_u
                }
            });
        }

        private void StartBrowser(SupportedBrowser aSupportedBrowser, string aDefaultUrl)
        {
            var _WebScraperConfiguration = WebScraperConfigurationManager.ReloadConfig();

            SupportedBrowserConfiguration _SupportedBrowserConfiguration = _WebScraperConfiguration.GetBrowserConfigurationBySupportedBrowser(aSupportedBrowser);

            if (_SupportedBrowserConfiguration == null)
                return;

            fBrowserProcessName = _SupportedBrowserConfiguration.ExecutablePath;
            var _Process = new Process
            {
                StartInfo =
                {
                    Arguments = $"{aDefaultUrl}",
                    FileName = fBrowserProcessName,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Maximized// not really working for browser ?...meh
                }
            };

            _Process.Start();
        }

        private bool WaitForWebPageToFinishLoadingByBrowser(SupportedBrowser aSupportedBrowser)
        {
            Process[] _Processes = GetProcessesAssociatedWithBrowser(aSupportedBrowser);

            foreach (Process _Process in _Processes)
            {
                // Wait for this instance of the browser to be responsive to user input (max 120 sec to avoid deadloop)
                if (!_Process.WaitForInputIdle(NB_MS_WAIT_FOR_INPUT_HANG))
                    return false;
            }

            return true;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public string GetDOMFromUrl(SupportedBrowser aSupportedBrowser, string aUrl)
        {
            string _DOM = "";
            // For the duration of the function, we're taking exclusivity of inputs to avoid conflicts
            InputsManager.ExecuteInputsWithExclusivity(() =>
            {
                StartBrowser(aSupportedBrowser, aUrl);

                if (!WaitForWebPageToFinishLoadingByBrowser(aSupportedBrowser))
                {
                    LoggingManager.LogToFile($"Couldn't wait for browser [{aSupportedBrowser}]. It may be stuck. Aborting [{nameof(GetDOMFromUrl)}].");
                    return;
                }

                // Wait a little longer just in case the system is a little slow (like a raspberry pi for instance)
                Thread.Sleep(5000);

                // Fullscreen
                ExecuteFullScreenF11();
                Thread.Sleep(500);

                // Get DOM
                ShowDOMInNewTab();
                Thread.Sleep(1000);

                // Clear clipboard
                ClipboardManager.SetClipboardValue("");
                Thread.Sleep(50);

                // Put in clipboard
                CopyAllToClipboard();
                Thread.Sleep(500);

                // Exit F11
                ExecuteFullScreenF11();
                Thread.Sleep(500);

                // Close the tab we created for the DOM
                CloseCurrentTab();
                Thread.Sleep(250);

                // Close the tab we created on the first step
                CloseCurrentTab();
                Thread.Sleep(250);

                // Copy clipboard value to var
                _DOM = ClipboardManager.GetClipboardValue();
                Thread.Sleep(50);

                // Clear clipboard
                ClipboardManager.SetClipboardValue("");
                Thread.Sleep(50);

                // Note: actually, we already closed our tab.. so.. no, don't kill the browser..
                // Close browser
                //KillBrowserProcess(aSupportedBrowser);
            });

            return _DOM;
        }
    }
}
