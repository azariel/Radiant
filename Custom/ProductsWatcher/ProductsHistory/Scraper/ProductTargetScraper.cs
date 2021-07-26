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
using Radiant.Common.Serialization;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.Custom.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;
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
        private List<ProductDOMParserItem> fDOMParserItems = new();
        private List<ManualScraperProductParser> fManualScraperItems = new();

        private void CreateErrorNotificationForAdministration(string aCustomContent)
        {
            try
            {
                string _Content = $"<p>Url: {fUrl}</p>{Environment.NewLine}{aCustomContent}";

                // TODO: send notification ? with screenshot and DOM
                RadiantNotificationModel _NewNotification = new()
                {
                    Content = _Content,
                    Subject = "Error couldn't fetch product information",
                    EmailFrom = "Radiant Product History",
                    MinimalDateTimetoSend = DateTime.Now

                    // TODO: attachments = Screenshot + DOM
                };

                // Add all subscribed users email to notification EmailTo
                using ServerProductsDbContext _ProductDbContext = new();
                _ProductDbContext.Users.Load();

                // Notify all Admins
                _NewNotification.EmailTo.AddRange(_ProductDbContext.Users.Where(w => w.Type == RadiantUserModel.UserType.Admin).Select(s => s.Email));

                if (_NewNotification.EmailTo.Count <= 0)
                {
                    LoggingManager.LogToFile("323A13DB-1D77-4693-BBD6-45C63A4A167A", $"No admin(s) found to send error notification. Error was : Couldn't fetch product information");
                    return;
                }

                using NotificationsDbContext _NotificationDbContext = new();
                _NotificationDbContext.Notifications.Add(_NewNotification);
                _NotificationDbContext.SaveChanges();
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("A1273815-7729-41E3-B4C6-94979F9908E9", $"Couldn't create notification on {nameof(ProductTargetScraper)} fetch failure.", _Ex);
            }
        }

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

        private void HandleFailureProcess()
        {
            LoggingManager.LogToFile("9C0FE4DD-408C-4F5A-A716-A9D7CF23D729", $"Couldn't fetch price on Url [{fUrl}].");

            WriteProductInformationToErrorFolder();

            CreateErrorNotificationForAdministration("<p>Product price couldn't be fetched. Will retry later.</p><p>Check DOM and screenshot saved on Server disk for more info.</p>");
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
            {
                LoggingManager.LogToFile("1A1AF5EE-BDFA-4F71-A698-935341E133AD", $"Trying to fetch price of [{fUrl}], but no DOM parsers were configured matching this URL domain.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            LoggingManager.LogToFile("D07C90BF-8570-4BE6-A98A-2EFB20322A4A", $"Trying to fetch price of [{fUrl}] using [{fDOMParserItems.Count}] DOM parsers", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            double? _Price = DOMProductInformationParser.ParsePrice(fUrl, this.DOM, fDOMParserItems);

            if (_Price.HasValue)
                this.Information.Price = _Price;

            if (!this.Information.Price.HasValue)
                LoggingManager.LogToFile("2CCDD325-3050-4FA5-A9F5-F9331A155C4F", $"DOM parser step to fetch price of product [{fUrl}] failed.");
        }

        private void TryFetchProductPriceByManualOperation()
        {
            try
            {
                ManualScraperProductParser[] _AvailableProductParser = fManualScraperItems.Where(w => w.Target == ProductParserItemTarget.Price).ToArray();
                LoggingManager.LogToFile("013C0C5E-5149-465B-9D7E-1138169C5869", $"Trying to fetch price of [{fUrl}] using [{_AvailableProductParser.Length}] Manual Product Parsers.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                foreach (ManualScraperProductParser _ManualScraperItemParser in _AvailableProductParser)
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
                                    string _RawPrice = ClipboardManager.GetClipboardValue().Replace("\r", "").Replace("\n", "");

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
                                        MatchCollection _Matches = _PriceRegex.Matches(_RawPrice);

                                        if (_Matches.Count <= 0)
                                            return;

                                        Match _SelectedMatch = _ManualScraperItemParser.ValueParser.RegexMatch switch
                                        {
                                            RegexItemResultMatch.First => _Matches.First(),
                                            RegexItemResultMatch.Last => _Matches.Last(),
                                            _ => throw new Exception($"{nameof(RegexItemResultMatch)} value [{_ManualScraperItemParser.ValueParser.RegexMatch}] is unhandled.")
                                        };

                                        switch (_ManualScraperItemParser.ValueParser.Target)
                                        {
                                            case RegexItemResultTarget.Value:
                                                _Price = _SelectedMatch.Value;
                                                break;
                                            case RegexItemResultTarget.Group0Value when _SelectedMatch.Groups.Count < 1:
                                                return;
                                            case RegexItemResultTarget.Group0Value:
                                                _Price = _SelectedMatch.Groups[0].Value;
                                                break;
                                            case RegexItemResultTarget.Group1Value when _SelectedMatch.Groups.Count < 2:
                                                return;
                                            case RegexItemResultTarget.Group1Value:
                                                _Price = _SelectedMatch.Groups[1].Value;
                                                break;
                                            case RegexItemResultTarget.LastGroupValue when _SelectedMatch.Groups.Count < 1:
                                                return;
                                            case RegexItemResultTarget.LastGroupValue:
                                                _Price = _SelectedMatch.Groups[^1].Value;
                                                break;
                                        }
                                    }

                                    // If price doesn't contains decimal separator, add it
                                    if (!_Price.Contains('.') && !_Price.Contains(',') && _Price.Length > 2)
                                        _Price = _Price.Insert(_Price.Length - 2, ".");

                                    if (!string.IsNullOrWhiteSpace(_Price) && double.TryParse(_Price, out double _PriceAsDouble))
                                    {
                                        this.Information.Price = _PriceAsDouble;
                                        return;
                                    }
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

            if (!this.Information.Price.HasValue)
                LoggingManager.LogToFile("158B5041-37B1-476F-8DC2-C96430E2B0F9", $"Manual steps to fetch price of product [{fUrl}] failed.");
            else
                LoggingManager.LogToFile("161DD11D-5DE3-4120-83F7-9A6061173878", "Product price was fetched using manual parser.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        private void WriteProductInformationToErrorFolder()
        {
            try
            {
                // Save Screenshot and DOM in error folder
                string _RootFolder = "Errors";

                if (!Directory.Exists(_RootFolder))
                    Directory.CreateDirectory(_RootFolder);

                DateTime _Now = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(this.DOM))
                    File.WriteAllText(Path.Combine(_RootFolder, $"{_Now:HH.mm.ss}-DOM.txt"), this.DOM);

                if (this.Screenshot != null && this.Screenshot.Length > 0)
                    File.WriteAllBytes(Path.Combine(_RootFolder, $"{_Now:HH.mm.ss}.png"), this.Screenshot);

                // Log other relevant information to a single file
                File.WriteAllText(Path.Combine(_RootFolder, $"{_Now:HH.mm.ss}-INFO.txt"),
                    @$"Url: {fUrl}{Environment.NewLine}
                              OneOrMoreStepFailedAndRequiredAFallback: {this.OneOrMoreStepFailedAndRequiredAFallback}{Environment.NewLine}
                              this.Information: {JsonCommonSerializer.SerializeToString(this.Information)}{Environment.NewLine}
");

            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("6C69E0C6-6C77-4C91-B4D8-FF9EFDA88129", "Couldn't write fail files on disk.", _Ex);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            fDOMParserItems = aDOMParserItems?.OfType<ProductDOMParserItem>().ToList() ?? new List<ProductDOMParserItem>();
            fManualScraperItems = aManualScraperItems?.OfType<ManualScraperProductParser>().Where(w => aUrl.ToLowerInvariant().Contains(w.IfUrlContains.ToLowerInvariant())).ToList() ?? new List<ManualScraperProductParser>();

            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aManualScraperItems, aDOMParserItems);

            WaitForBrowserInputsReadyOrMax(500);

            // Fetch product information
            FetchProductInformation();

            if (!this.Information.Price.HasValue)
            {
                HandleFailureProcess();
                return;
            }

            // Validate fetched information with DOM parser to check if we should inform Admins that a configuration may be incorrect
            double? _Price = DOMProductInformationParser.ParsePrice(fUrl, this.DOM, fDOMParserItems);

            if (this.OneOrMoreStepFailedAndRequiredAFallback || !_Price.HasValue || Math.Abs(this.Information.Price.Value - _Price.Value) >= 0.01)
            {
                WriteProductInformationToErrorFolder();
                CreateErrorNotificationForAdministration($"<p>The price fetched was different from DOM parser price fetched.</p><p>this.OneOrMoreStepFailedAndRequiredAFallback = {this.OneOrMoreStepFailedAndRequiredAFallback}</p><p>_Price.HasValue={_Price.HasValue}</p><p>this.Information.Price.Value={this.Information.Price.Value}</p><p>_Price.Value(by DOM only)={_Price}</p>");
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductFetchedInformation Information { get; set; } = new();

        /// <summary>
        /// If a step failed, for example, a manual step and we had to fallback to the DOM parser (or other fallback), this will be
        /// true
        /// </summary>
        public bool OneOrMoreStepFailedAndRequiredAFallback { get; set; }
    }
}
