using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Business;
using Radiant.Common.Database.Common;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.Custom.ProductsHistory.DataBase;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.Notifier.DataBase;
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

            this.OneOrMoreStepFailedAndRequiredAFallback = true;

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

            if(!this.Information.Price.HasValue)
                LoggingManager.LogToFile("2CCDD325-3050-4FA5-A9F5-F9331A155C4F", $"DOM parser step to fetch price of product [{fUrl}] failed.");
        }

        private void TryFetchProductPriceByManualOperation()
        {
            try
            {
                foreach (ManualScraperProductParser _ManualScraperItemParser in fManualScraperItems.Where(w => w.Target == ProductParserItemTarget.Price))
                {
                    // Check conditions (Certain manual scraper requires something to work. ex: if "Deal Price:" is found on the webpage, we can use a certain manual scraper
                    if (_ManualScraperItemParser.Condition?.Evaluate(fBrowser) == false)
                        continue;

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

                                    // If price doesn't contains decimal separator, add it
                                    string _Price = _RawPrice;
                                    if (_ManualScraperItemParser.ValueParser?.RegexPattern != null)
                                    {
                                        // Parse price
                                        Regex _PriceRegex = new Regex(_ManualScraperItemParser.ValueParser?.RegexPattern, RegexOptions.CultureInvariant);
                                        Match _Match = _PriceRegex.Match(_RawPrice);

                                        if (!_Match.Success)
                                            return;

                                        if (_ManualScraperItemParser.ValueParser.Target == RegexItemResultTarget.Value)
                                            _Price = _Match.Value;
                                        else if (_ManualScraperItemParser.ValueParser.Target == RegexItemResultTarget.Group0Value)
                                        {
                                            if (_Match.Groups.Count < 1)
                                                return;

                                            _Price = _Match.Groups[0].Value;
                                        } else if (_ManualScraperItemParser.ValueParser.Target == RegexItemResultTarget.Group1Value)
                                        {
                                            if (_Match.Groups.Count < 2)
                                                return;

                                            _Price = _Match.Groups[1].Value;
                                        }
                                    }

                                    // If price doesn't contains decimal separator, add it
                                    if (!_Price.Contains('.') && !_Price.Contains(',') && _Price.Length > 2)
                                        _Price = _Price.Insert(_Price.Length - 2, ".");

                                    if (!string.IsNullOrWhiteSpace(_Price) && double.TryParse(_Price, out double _PriceAsDouble))
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
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("2AE999BA-5D76-4CF0-AC97-EB51D3EF2CC2", $"Couldn't reproduce steps for manual operation in [{nameof(ProductTargetScraper)}].", _Ex);
                throw;
            }

            if(!this.Information.Price.HasValue)
                LoggingManager.LogToFile("158B5041-37B1-476F-8DC2-C96430E2B0F9", $"Manual steps to fetch price of product [{fUrl}] failed.");
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            fDOMParserItems = aDOMParserItems?.OfType<ProductDOMParserItem>().ToList();
            fManualScraperItems = aManualScraperItems?.OfType<ManualScraperProductParser>().Where(w => aUrl.ToLowerInvariant().Contains(w.IfUrlContains.ToLowerInvariant())).ToList();

            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aManualScraperItems, aDOMParserItems);

            WaitForBrowserInputsReadyOrMax(500);

            // Fetch product information
            FetchProductInformation();

            if (!this.Information.Price.HasValue)
                HandleFailureProcess();
        }

        private void HandleFailureProcess()
        {
            LoggingManager.LogToFile("9C0FE4DD-408C-4F5A-A716-A9D7CF23D729", $"Couldn't fetch price on Url [{fUrl}].");

            try
            {
                // Save Screenshot and DOM in error folder
                string _RootFolder = "Errors";

                if (!Directory.Exists(_RootFolder))
                    Directory.CreateDirectory(_RootFolder);

                if (!string.IsNullOrWhiteSpace(this.DOM))
                    File.WriteAllText(Path.Combine(_RootFolder, $"{DateTime.Now:HH24.mm.ss}-DOM.txt"), this.DOM);

                if (this.Screenshot != null && this.Screenshot.Length > 0)
                    File.WriteAllBytes(Path.Combine(_RootFolder, $"{DateTime.Now:HH24.mm.ss}.png"), this.Screenshot);

            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("6C69E0C6-6C77-4C91-B4D8-FF9EFDA88129", "Couldn't write fail files on disk.", _Ex);
            }

            try
            {
                // TODO: send notification ? with screenshot and DOM
                RadiantNotificationModel _NewNotification = new()
                {
                    Content = $"<p>Url: {fUrl}</p>",
                    Subject = "Error couldn't fetch product information",
                    EmailFrom = "Radiant Product History",
                    MinimalDateTimetoSend = DateTime.Now

                    // TODO: attachments = Screenshot + DOM
                };

                // Add all subscribed users email to notification EmailTo
                using ProductsDbContext _ProductDbContext = new();
                _ProductDbContext.Users.Load();

                // Notify all Admins
                _NewNotification.EmailTo.AddRange(_ProductDbContext.Users.Where(w => w.Type == RadiantUserModel.UserType.Admin).Select(s => s.Email));

                using NotificationsDbContext _NotificationDbContext = new();
                _NotificationDbContext.Notifications.Add(_NewNotification);
                _NotificationDbContext.SaveChanges();
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("A1273815-7729-41E3-B4C6-94979F9908E9", $"Couldn't create notification on {nameof(ProductTargetScraper)} fetch failure.", _Ex);
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductFetchedInformation Information { get; set; } = new();

        /// <summary>
        /// If a step failed, for example, a manual step and we had to fallback to the DOM parser (or other fallback), this will be true
        /// </summary>
        public bool OneOrMoreStepFailedAndRequiredAFallback { get; set; } = false;
    }
}
