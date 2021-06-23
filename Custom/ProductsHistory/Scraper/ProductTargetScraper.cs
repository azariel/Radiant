using System;
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
        private List<ManualScraperProductParser> fManualScraperItems;

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
                TryFetchProductPriceByManualOperation();

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

        private void TryFetchProductPriceByManualOperation()
        {
            foreach (ManualScraperProductParser _ManualScraperItemParser in fManualScraperItems.Where(w => w.Target == ProductParserItemTarget.Price))
            {
                foreach (ManualScraperSequenceItem _ManualScraperSequenceItem in _ManualScraperItemParser.ManualScraperSequenceItems)
                {
                    switch (_ManualScraperSequenceItem)
                    {
                        case ManualScraperSequenceItemByClipboard _ManualScraperSequenceItemByClipboard:
                            if (_ManualScraperSequenceItemByClipboard.Operation == ManualScraperSequenceItemByClipboard.ClipboardOperation.Get)
                            {
                                string _RawPrice = ClipboardManager.GetClipboardValue();

                                if (_ManualScraperSequenceItemByClipboard.WaitMsOnEnd > 0)
                                    WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItemByClipboard.WaitMsOnEnd);

                                // Override clipboard value
                                ClipboardManager.SetClipboardValue("");

                                if (_ManualScraperSequenceItemByClipboard.WaitMsOnEnd > 0)
                                    WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItemByClipboard.WaitMsOnEnd / 2);

                                string _Price = _RawPrice;
                                if (_ManualScraperItemParser.ValueParser?.RegexPattern != null)
                                {
                                    // Parse price
                                    Regex _PriceRegex = new Regex(_ManualScraperItemParser.ValueParser?.RegexPattern, RegexOptions.CultureInvariant);
                                    Match _Match = _PriceRegex.Match(_RawPrice);

                                    if (!_Match.Success)
                                        return;

                                    if (_ManualScraperItemParser.ValueParser.Target == DOMParserItem.DOMParserItemResultTarget.Value)
                                        _Price = _Match.Value;
                                    else if (_ManualScraperItemParser.ValueParser.Target == DOMParserItem.DOMParserItemResultTarget.Group1Value)
                                    {
                                        if (_Match.Groups.Count < 1)
                                            return;

                                        _Price = _Match.Groups[0].Value;
                                    }
                                }

                                if (double.TryParse(_Price, out double _PriceAsDouble))
                                    this.Information.Price = _PriceAsDouble;
                            }

                            break;
                        case ManualScraperSequenceItemByInput _ManualScraperSequenceItemByInput:
                            InputsManager.ExecuteConcurrentInputWithOverrideOfExclusivity(_ManualScraperSequenceItemByInput.InputType, _ManualScraperSequenceItemByInput.InputParam);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_ManualScraperSequenceItem));
                    }

                    if (_ManualScraperSequenceItem.WaitMsOnEnd > 0)
                        WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItem.WaitMsOnEnd);
                }
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            fDOMParserItems = aDOMParserItems.OfType<ProductDOMParserItem>().ToList();
            fManualScraperItems = aManualScraperItems.OfType<ManualScraperProductParser>().Where(w => aUrl.ToLowerInvariant().Contains(w.IfUrlContains.ToLowerInvariant())).ToList();

            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aManualScraperItems, aDOMParserItems);

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
