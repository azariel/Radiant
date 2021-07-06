using System.Collections.Generic;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    /// <summary>
    /// Define how to get DOM from scraper
    /// </summary>
    public class DOMTargetScraper : BaseTargetScraper, IScraperTarget
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public DOMTargetScraper() { }

        public DOMTargetScraper(TargetScraperCoreOptions aOptions) : base(aOptions) { }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void ShowDOMInNewTab()
        {
            // Note that it's the same in every supported browser (atm)
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 120,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Control_L,
                    Keycode.XK_u
                }
            });
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aManualScraperItems, aDOMParserItems);

            if (aAllowManualOperations)
            {
                // Get DOM
                ShowDOMInNewTab();
                WaitForBrowserInputsReadyOrMax(5124);

                // Clear clipboard
                ClipboardManager.SetClipboardValue("");
                WaitForBrowserInputsReadyOrMax(351);

                // Put in clipboard
                ManualScraperSequenceHelper.CopyAllToClipboard();
                WaitForBrowserInputsReadyOrMax(1026);

                // Copy clipboard value to var
                this.DOM = ClipboardManager.GetClipboardValue();
                WaitForBrowserInputsReadyOrMax(521);

                // Clear clipboard
                ClipboardManager.SetClipboardValue("");
                WaitForBrowserInputsReadyOrMax(487);

                // Close the tab we created for the DOM
                ManualScraperSequenceHelper.CloseCurrentTab();
                WaitForBrowserInputsReadyOrMax(1303);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
