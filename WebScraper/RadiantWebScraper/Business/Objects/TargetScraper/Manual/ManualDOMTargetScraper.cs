using System;
using System.Collections.Generic;
using System.IO;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.InputsManager;
using Radiant.InputsManager.InputsParam;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;

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
        private const string DEBUG_FILENAME = "DOMScraperOps.txt";

        private void CloseInspector()
        {
            File.AppendAllText(DEBUG_FILENAME, $"Close Inspector.{Environment.NewLine}");
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
            File.AppendAllText(DEBUG_FILENAME, $"ExtractDOM start.{Environment.NewLine}");
            // Get DOM
            ShowAndFocusDOMInInspector();
            File.AppendAllText(DEBUG_FILENAME, $"ExtractDOM done.{Environment.NewLine}");

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

            // Close the inspector window
            CloseInspector();
        }

        private void ShowAndFocusDOMInInspector()
        {
            File.AppendAllText(DEBUG_FILENAME, $"Focus DOM in Inspector.{Environment.NewLine}");

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
                Delay = 160,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(201);

            InputsManager.InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 320,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(526);
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
