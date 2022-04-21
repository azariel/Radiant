using System.Collections.Generic;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Business.Objects.TargetScraper.Manual
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
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_F12
                }
            });
            WaitForBrowserInputsReadyOrMax(2303);
        }

        private void ExtractDOM()
        {
            // Get DOM
            ShowAndFocusDOMInInspector();

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(451);

            // Put in clipboard
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 460,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_c
                }
            });

            WaitForBrowserInputsReadyOrMax(3026);

            // Copy clipboard value to var
            this.DOM = ClipboardManager.GetClipboardValue();
            WaitForBrowserInputsReadyOrMax(821);

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(387);

            // Close the inspector window
            CloseInspector();
        }

        private void ShowAndFocusDOMInInspector()
        {
            // Note that it's the same in every supported browser (atm)
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_F12
                }
            });
            WaitForBrowserInputsReadyOrMax(15154);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(526);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 180,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(326);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 220,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(376);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 160,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(399);

            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 320,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_TAB
                }
            });
            WaitForBrowserInputsReadyOrMax(926);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aScraperItemsParser, aDOMParserItems);

            if (aAllowManualOperations)
            {
                WaitForBrowserInputsReadyOrMax(500);
                ExtractDOM();

                if (string.IsNullOrWhiteSpace(this.DOM))
                {
                    ShowAndFocusDOMInInspector();

                    // Try to close split console
                    InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
                    {
                        Delay = 320,
                        KeyStrokeCodes = new[]
                        {
                            Keycode.XK_Escape
                        }
                    });
                    WaitForBrowserInputsReadyOrMax(352);

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
