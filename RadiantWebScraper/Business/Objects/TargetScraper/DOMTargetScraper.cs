using System.Threading;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    /// <summary>
    /// Define how to get DOM from scraper
    /// </summary>
    internal class DOMTargetScraper : BaseTargetScraper, IScraperTarget
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
                    Keycode.CtrlL,
                    Keycode.XK_u
                }
            });
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl)
        {
            base.Evaluate(aSupportedBrowser, aUrl);

            // Get DOM
            ShowDOMInNewTab();
            Thread.Sleep(1000);

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            Thread.Sleep(50);

            // Put in clipboard
            ManualScraperSequenceHelper.CopyAllToClipboard();
            Thread.Sleep(500);

            // Copy clipboard value to var
            this.DOM = ClipboardManager.GetClipboardValue();
            Thread.Sleep(250);

            // Clear clipboard
            ClipboardManager.SetClipboardValue("");
            Thread.Sleep(250);

            // Close the tab we created for the DOM
            ManualScraperSequenceHelper.CloseCurrentTab();
            Thread.Sleep(250);
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
