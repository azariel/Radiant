using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;
using RadiantInputsManager.InputsParam;

namespace Radiant.Custom.ProductsHistory.Scraper
{
    public class ProductTargetScraper : DOMTargetScraper, IScraperTarget
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ProductTargetScraper() { }

        public ProductTargetScraper(TargetScraperCoreOptions aOptions) : base(aOptions) { }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private List<ProductDOMParserItem> fDOMParserItems;

        private void FetchProductInformation()
        {
            FetchProductPrice();
            FetchProductName();

            // TODO: fetch product shipping cost, etc
        }

        /// <summary>
        /// Fetching the name is mainly to know if the URL content has changed. Amazon is a big fan of this. The product
        /// shown for a specific url will change from time to time.
        /// </summary>
        private void FetchProductName()
        {
            // Fetch product name from DOM
            TryFetchProductNameFromDOM();
        }

        private void FetchProductPrice()
        {
            // First, try to find the price by search
            if (fAllowManualOperations)
                TryFetchProductPriceBySearch();

            if (this.Information.Price.HasValue)
                return;

            // If it doesn't work, fallback to find the price in the DOM
            TryFetchProductPriceByDOM();
        }

        private void TryFetchProductNameFromDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
                return;

            this.Information.Title = DOMProductInformationParser.ParseTitle(fUrl, this.DOM, fDOMParserItems);
        }

        private void TryFetchProductPriceByDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
                return;

            double? _Price = DOMProductInformationParser.ParsePrice(fUrl, this.DOM, fDOMParserItems);

            if (_Price.HasValue)
                this.Information.Price = _Price;
        }

        private void TryFetchProductPriceBySearch()
        {
            ManualScraperSequenceHelper.Search("price:");

            WaitForBrowserInputsReadyOrMax(1151);

            // Press escape
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 290,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Escape
                }
            });

            WaitForBrowserInputsReadyOrMax(625);

            // Shift+Right
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 321,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Shift_L,
                    Keycode.XK_Right
                }
            });

            WaitForBrowserInputsReadyOrMax(422);

            // Shift+End
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 159,
                KeyStrokeCodes = new[]
                {
                    Keycode.XK_Shift_L,
                    Keycode.XK_End
                }
            });

            WaitForBrowserInputsReadyOrMax(522);

            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(625);

            // Copy to clipboard
            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(InputsManager.InputType.Keyboard, new KeyboardKeyStrokeActionInputParam
            {
                Delay = 168,
                KeyStrokeCodes = new[]
                {
                    Keycode.CtrlL,
                    Keycode.XK_c
                }
            });

            WaitForBrowserInputsReadyOrMax(1314);

            string _RawPrice = ClipboardManager.GetClipboardValue();

            WaitForBrowserInputsReadyOrMax(1254);

            // Override clipboard value
            ClipboardManager.SetClipboardValue("");
            WaitForBrowserInputsReadyOrMax(625);

            // Parse price
            Regex _PriceRegex = new Regex(@"[\d,]+\.\d+", RegexOptions.CultureInvariant);
            Match _Match = _PriceRegex.Match(_RawPrice);

            if (!_Match.Success || _Match.Groups.Count < 1)
                return;

            string _Price = _Match.Groups[0].Value;

            if (double.TryParse(_Price, out double _PriceAsDouble))
                this.Information.Price = _PriceAsDouble;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<DOMParserItem> aDOMParserItems)
        {
            fDOMParserItems = aDOMParserItems.OfType<ProductDOMParserItem>().ToList();

            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aDOMParserItems);

            WaitForBrowserInputsReadyOrMax(500);

            // Fetch product information
            FetchProductInformation();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductFetchedInformation Information { get; set; } = new();
    }
}
