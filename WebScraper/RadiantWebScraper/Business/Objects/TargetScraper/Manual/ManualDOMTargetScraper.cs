using System;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;
using System.Collections.Generic;
using System.IO;
using Radiant.Common.Utils;
using Radiant.WebScraper.RadiantWebScraper.Configuration;

namespace Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Manual
{
    /// <summary>
    /// Define how to get DOM from scraper
    /// </summary>
    public class ManualDOMTargetScraper : ManualBaseTargetScraper, IScraperTarget
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ManualDOMTargetScraper() { }

        public ManualDOMTargetScraper(TargetScraperCoreOptions aOptions) : base(aOptions) { }

        // ********************************************************************
        //                            Private
        // ********************************************************************

        private void CloseInspector()
        {
            LoggingManager.LogToFile("ea494753-8d38-4ae3-b0d4-d635e5a8fc52", "Closing DOM in Inspector.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_F12
                }
            });
            WaitForBrowserInputsReadyOrMax(1303);
        }

        private void ExtractDOM()
        {
            LoggingManager.LogToFile("99522242-b5c2-4a5b-886c-feda421c22a8", "Extracting DOM...", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // Get DOM
            ShowAndFocusDOMInInspector();

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(251);

            // Put in clipboard
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 460,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_c
                }
            });

            WaitForBrowserInputsReadyOrMax(1526);

            // Copy clipboard value to var
            this.DOM = ClipboardManager.GetClipboardValue();
            WaitForBrowserInputsReadyOrMax(421);

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(147);

            // Save DOM file
            var _Config = WebScraperConfigurationManager.ReloadConfig();

            if (_Config.TakeDOMExtractionScreenshot)
            {
                string _RootFolder = "Screenshots";

                if (!string.IsNullOrWhiteSpace(fUrl))
                    _RootFolder = Path.Combine(_RootFolder, RegexUtils.GetWebSiteDomain(fUrl));

                // Add current date to root folder
                _RootFolder = Path.Combine(_RootFolder, $"{DateTime.Now:yyyy-MM-dd}");

                DateTime _Now = DateTime.Now;
                ImageUtils.TakeScreenshot(_RootFolder, out string _);

                // Add the DOM file beside
                File.WriteAllText(Path.Combine(_RootFolder, $"{_Now:yyyy-MM-dd HH.mm.ss.fff}-DOM.txt"), this.DOM);
            }

            // Close the inspector window
            CloseInspector();

            LoggingManager.LogToFile("0050e2c2-91a8-45c3-bf5e-9340c5ef00cf", "Extracting DOM done.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        private void ShowAndFocusDOMInInspector()
        {
            LoggingManager.LogToFile("7ba463af-9db9-4aeb-90ae-3841bef12d59", "Focusing DOM in Inspector...", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            // Note that it's the same in every supported browser (atm)
            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_F12
                }
            });
            WaitForBrowserInputsReadyOrMax(2054);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(266);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 180,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(186);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 220,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(156);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 320,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(526);

            LoggingManager.LogToFile("4bf7c499-b17e-4215-b89c-e98edd6877e9", "Focusing DOM in Inspector done.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aScraperItemsParser, aDOMParserItems);

            if (aAllowManualOperations)
            {
                WaitForBrowserInputsReadyOrMax(250);
                ExtractDOM();

                if (string.IsNullOrWhiteSpace(this.DOM))
                {
                    ShowAndFocusDOMInInspector();

                    // Try to close split console
                    InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
                    {
                        Delay = 320,
                        KeyStrokeCodes = new[]
                        {
                            Keycode.XK_Escape
                        }
                    });
                    WaitForBrowserInputsReadyOrMax(172);

                    CloseInspector();

                    // Try again, but with split console closed as it messed with TAB focus
                    ExtractDOM();
                }
            }

            ShrinkDOM();
        }

        /// <summary>
        /// DOM is often filled with Scripts and other things we don't need. executing a Regex on 1 million lines take a very long time that can be largely reduced
        /// </summary>
        private void ShrinkDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
                return;

            // TODO implement a DOM shrinker

            //Regex _RemScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            //this.DOM = _RemScript.Replace(this.DOM, "");

            //Regex _RemStyle = new Regex(@"<style[^>]*>[\s\S]*?</style>");
            //this.DOM = _RemStyle.Replace(this.DOM, "");
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
