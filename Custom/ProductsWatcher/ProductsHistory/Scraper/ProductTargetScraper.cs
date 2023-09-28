using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Business;
using Radiant.Common.Database.Common;
using Radiant.Common.Diagnostics;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.Common.Serialization;
using Radiant.Common.Utils;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.Custom.ProductsHistoryCommon.DataBase;
using Radiant.Notifier.DataBase;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;
using Radiant.WebScraper.Scrapers.Manual;
using RadiantInputsManager;

namespace Radiant.Custom.ProductsHistory.Scraper
{
    public class ProductTargetScraper : ManualDOMTargetScraper, IScraperTarget
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

                RadiantNotificationModel _NewNotification = new()
                {
                    Content = _Content,
                    Subject = "Error couldn't fetch product information",
                    EmailFrom = "Radiant Product History",
                    MinimalDateTimetoSend = DateTime.Now
                };

                // Attachments
                string _UniqueIdentifier = Guid.NewGuid().ToString();

                // Screenshot attachment
                if (this.Screenshot != null && this.Screenshot.Length > 0)
                {
                    _NewNotification.Attachments.Add(new()
                    {
                        Content = this.Screenshot,
                        NotificationId = _NewNotification.NotificationId,
                        FileName = $"Screenshot_{_UniqueIdentifier}.png",
                        MediaType = "image",
                        MediaSubType = "png"
                    });
                }

                // DOM attachment
                //if (!string.IsNullOrWhiteSpace(this.DOM))
                //{
                //    _NewNotification.Attachments.Add(new()
                //    {
                //        Content = Encoding.ASCII.GetBytes(this.DOM),
                //        NotificationId = _NewNotification.NotificationId,
                //        FileName = $"DOM_{_UniqueIdentifier}.txt",
                //        MediaType = "text",
                //        MediaSubType = ""
                //    });
                //}

                // Add all subscribed users email to notification EmailTo
                using ServerProductsDbContext _ProductDbContext = new();
                _ProductDbContext.Users.Load();

                // Notify all Admins
                IEnumerable<string> _Emails = _ProductDbContext.Users.Where(w => w.Type == RadiantUserModel.UserType.Admin).Select(s => s.Email).ToList().Distinct();
                
                _NewNotification.EmailTo.AddRange(_Emails);

                if (_NewNotification.EmailTo.Count <= 0)
                {
                    LoggingManager.LogToFile("323A13DB-1D77-4693-BBD6-45C63A4A167A", "No admin(s) found to send error notification. Error was : Couldn't fetch product information");
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

        private void FetchDiscounts()
        {
            FetchDiscountsPrice();
            FetchDiscountsPercentage();
        }

        private void FetchDiscountsPercentage()
        {
            // By manual operations
            if (fAllowManualOperations)
                this.Information.DiscountPrice += TryFetchSumOfAllDiscountPriceFromManualOperations(ProductParserItemTarget.DiscountPercentage);

            // By DOM parser
            TryFetchProductDiscountByDOM(ProductParserItemTarget.DiscountPercentage);
        }

        private void FetchDiscountsPrice()
        {
            // By manual operations
            if (fAllowManualOperations)
                this.Information.DiscountPrice += TryFetchSumOfAllDiscountPriceFromManualOperations(ProductParserItemTarget.DiscountPrice);

            // By DOM parser
            TryFetchProductDiscountByDOM(ProductParserItemTarget.DiscountPrice);
        }

        private void FetchOutOfStock()
        {
            // TODO: Handle manual operations

            // By DOM parser
            TryFetchProductOutOfStockByDOM();
        }

        private void FetchProductInformation()
        {
            FetchProductPrice();
            FetchProductName();
            FetchShippingCost();

            // Contrary to others, discounts are cumulable
            FetchDiscounts();

            FetchOutOfStock();
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
                this.Information.Price = TryFetchProductParserTargetByManualOperation(ProductParserItemTarget.Price, true, true);

            if (this.Information.Price.HasValue)
            {
                LoggingManager.LogToFile("87278768-40EA-4B01-AC63-8C1E2331AC4D", $"Product price [{this.Information.Price}] was fetched using manual parser.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            if (fAllowManualOperations)
            {
                LoggingManager.LogToFile("2781E8CF-F080-4B51-A831-7EDC06560E43", $"Manual steps to fetch price of product [{fUrl}] failed.");

                // We consider this an error only if we had at least 1 config available
                ManualScraperProductParser[] _AvailableProductParser = fManualScraperItems.Where(w => w.Target == ProductParserItemTarget.Price).ToArray();
                if (_AvailableProductParser.Any())
                    this.OneOrMoreStepFailedAndRequiredAFallback = true;
            }

            // If it doesn't work, fallback to find the price in the DOM
            TryFetchProductPriceByDOM();

            if (!this.Information.Price.HasValue && fDOMParserItems.Any(w => w.ParserItemTarget == ProductParserItemTarget.Price))
                this.OneOrMoreStepFailedAndRequiredAFallback = true;
        }

        private void FetchShippingCost()
        {
            // First, try to find the price by search

            if (fAllowManualOperations)
                this.Information.ShippingCost = TryFetchProductParserTargetByManualOperation(ProductParserItemTarget.ShippingCost, true, true);

            if (this.Information.ShippingCost.HasValue)
            {
                LoggingManager.LogToFile("E6CD9AF3-E26B-4826-9B61-45EA37C492D0", "Product shipping cost was fetched using manual parser.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            if (fAllowManualOperations)
            {
                LoggingManager.LogToFile("4C0E794F-3F15-4D56-83F0-9084ED439CC8", $"Manual steps to fetch shipping cost of product [{fUrl}] failed.");

                // We consider this an error only if we had at least 1 config available
                ManualScraperProductParser[] _AvailableProductParser = fManualScraperItems.Where(w => w.Target == ProductParserItemTarget.ShippingCost).ToArray();
                if (_AvailableProductParser.Any())
                    this.OneOrMoreStepFailedAndRequiredAFallback = true;
            }

            // If it doesn't work, fallback to find the price in the DOM
            TryFetchProductShippingCostByDOM();

            if (!this.Information.ShippingCost.HasValue && fDOMParserItems.Any(w => w.ParserItemTarget == ProductParserItemTarget.ShippingCost))
                this.OneOrMoreStepFailedAndRequiredAFallback = true;
        }

        private void HandleFailureProcess()
        {
            string _ErrorMessage = $"Couldn't fetch price on Url [{fUrl}].";
            LoggingManager.LogToFile("9C0FE4DD-408C-4F5A-A716-A9D7CF23D729", _ErrorMessage);
            WriteProductInformationToErrorFolder(_ErrorMessage);

            CreateErrorNotificationForAdministration("<p>Product price couldn't be fetched.</p><p>Check DOM and screenshot saved on Server disk for more info.</p>");
        }

        private void TryFetchProductDiscountByDOM(ProductParserItemTarget aProductParserItemTarget)
        {
            if (aProductParserItemTarget != ProductParserItemTarget.DiscountPercentage && aProductParserItemTarget != ProductParserItemTarget.DiscountPrice)
            {
                LoggingManager.LogToFile("F82099F8-B7DE-46E6-A2C2-FEC4EF8E05FC", $"Fetching discount of type [{aProductParserItemTarget}] is unhandled.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.DOM))
            {
                LoggingManager.LogToFile("EA41C271-CBAC-4001-A9B0-19AC565E4076", $"Trying to fetch {aProductParserItemTarget} of [{fUrl}], but no DOM was found on this Url.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            ProductDOMParserItem[] _DiscountParsers = fDOMParserItems.Where(w => w.ParserItemTarget == aProductParserItemTarget).ToArray();

            if (_DiscountParsers.Length <= 0)
                return;

            LoggingManager.LogToFile("58B2F76D-30F5-4FDF-BE48-752A1DBFB779", $"Trying to fetch {aProductParserItemTarget} of [{fUrl}] using [{_DiscountParsers.Length}] DOM parsers for this domain.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            double? _DiscountValue = DOMProductInformationParser.ParseDouble(fUrl, this.DOM, _DiscountParsers);

            switch (aProductParserItemTarget)
            {
                case ProductParserItemTarget.DiscountPrice:
                    if (_DiscountValue.HasValue)
                        this.Information.DiscountPrice = _DiscountValue.Value;
                    break;
                case ProductParserItemTarget.DiscountPercentage:
                    if (_DiscountValue.HasValue)
                        this.Information.DiscountPercentage = _DiscountValue.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aProductParserItemTarget), aProductParserItemTarget, null);
            }

            if (_DiscountParsers.Length <= 0)
                return;

            if (!_DiscountValue.HasValue)
                LoggingManager.LogToFile("E8FF8EAC-80AE-478A-AFF7-66654348162B", $"DOM parser step to fetch {aProductParserItemTarget} of product [{fUrl}] failed.");
            else
                LoggingManager.LogToFile("E58A5E3D-C351-48D2-96DB-09636027C3B7", $"Product {aProductParserItemTarget} was fetched using DOM parser", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        private void TryFetchProductNameFromDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
                return;

            this.Information.Title = DOMProductInformationParser.ParseTitle(fUrl, this.DOM, fDOMParserItems);
        }

        private void TryFetchProductOutOfStockByDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
                return;

            this.Information.OutOfStock = DOMProductInformationParser.ParseBoolean(fUrl, this.DOM, fDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.OutOfStock).ToArray());
        }

        private double? TryFetchProductParserTargetByManualOperation(ProductParserItemTarget aProductParserItemTarget, bool aTruncateEmptyLinesInValueFound, bool aApplyPriceTransformationToTargetValue)
        {
            try
            {
                ManualScraperProductParser[] _AvailableProductParser = fManualScraperItems.Where(w => w.Target == aProductParserItemTarget).ToArray();

                if (!_AvailableProductParser.Any())
                {
                    LoggingManager.LogToFile("0B22380F-DA8D-482C-B37D-EF0517EF6229", $"Product [{fUrl}] had no manual parser of target type [{aProductParserItemTarget}].");
                    return null;
                }

                LoggingManager.LogToFile("D11C642D-DABC-4625-B004-41E8C39D582A", $"Trying to fetch {aProductParserItemTarget} of [{fUrl}] using [{_AvailableProductParser.Length}] Manual Product Parsers.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                foreach (ManualScraperProductParser _ManualScraperItemParser in _AvailableProductParser)
                {
                    double? _Price = TryFetchProductParserTargetByManualOperationBySpecificParser(_ManualScraperItemParser, aTruncateEmptyLinesInValueFound, aApplyPriceTransformationToTargetValue);

                    if (_Price.HasValue)
                        return _Price;
                }
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("FA210BC6-9321-422A-9378-4874AB53F241", $"Couldn't reproduce steps for manual operation in [{nameof(ProductTargetScraper)}].", _Ex);
                throw;
            }

            return null;
        }

        private double? TryFetchProductParserTargetByManualOperationBySpecificParser(ManualScraperProductParser aManualScraperProductParser, bool aTruncateEmptyLinesInValueFound, bool aApplyPriceTransformationToTargetValue)
        {
            // Check conditions (Certain manual scraper requires something to work. ex: if "Deal Price:" is found on the webpage, we can use a certain manual scraper
            if (aManualScraperProductParser.Condition?.Evaluate(fBrowser) == false)
                return null;

            foreach (ManualScraperSequenceItem _ManualScraperSequenceItem in aManualScraperProductParser.ManualScraperSequenceItems)
            {
                switch (_ManualScraperSequenceItem)
                {
                    case ManualScraperSequenceItemByClipboard _ManualScraperSequenceItemByClipboard:
                        if (_ManualScraperSequenceItemByClipboard.Operation == ManualScraperSequenceItemByClipboard.ClipboardOperation.Get)
                        {
                            string _RawTargetValue = ClipboardManager.GetClipboardValue();

                            if (aTruncateEmptyLinesInValueFound)
                                _RawTargetValue = _RawTargetValue.Replace("\r", "").Replace("\n", "");

                            if (_ManualScraperSequenceItemByClipboard.WaitMsOnEnd > 0)
                                WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItemByClipboard.WaitMsOnEnd);

                            // Override clipboard value
                            ClipboardManager.SetClipboardValue("");

                            if (_ManualScraperSequenceItemByClipboard.WaitMsOnEnd > 0)
                                WaitForBrowserInputsReadyOrMax(_ManualScraperSequenceItemByClipboard.WaitMsOnEnd / 2);

                            // If target doesn't contains decimal separator, add it
                            string _TargetValue = _RawTargetValue;
                            if (aManualScraperProductParser.ValueParser?.RegexPattern != null)
                            {
                                // Parse target value
                                Regex _PriceRegex = new Regex(aManualScraperProductParser.ValueParser?.RegexPattern, RegexOptions.CultureInvariant);
                                MatchCollection _Matches = _PriceRegex.Matches(_RawTargetValue);

                                if (_Matches.Count <= 0)
                                    return null;

                                Match _SelectedMatch = aManualScraperProductParser.ValueParser.RegexMatch switch
                                {
                                    RegexItemResultMatch.First => _Matches.First(),
                                    RegexItemResultMatch.Last => _Matches.Last(),
                                    _ => throw new Exception($"{nameof(RegexItemResultMatch)} value [{aManualScraperProductParser.ValueParser.RegexMatch}] is unhandled.")
                                };

                                switch (aManualScraperProductParser.ValueParser.Target)
                                {
                                    case RegexItemResultTarget.Value:
                                        _TargetValue = _SelectedMatch.Value;
                                        break;
                                    case RegexItemResultTarget.Group0Value when _SelectedMatch.Groups.Count < 1:
                                        return null;
                                    case RegexItemResultTarget.Group0Value:
                                        _TargetValue = _SelectedMatch.Groups[0].Value;
                                        break;
                                    case RegexItemResultTarget.Group1Value when _SelectedMatch.Groups.Count < 2:
                                        return null;
                                    case RegexItemResultTarget.Group1Value:
                                        _TargetValue = _SelectedMatch.Groups[1].Value;
                                        break;
                                    case RegexItemResultTarget.LastGroupValue when _SelectedMatch.Groups.Count < 1:
                                        return null;
                                    case RegexItemResultTarget.LastGroupValue:
                                        _TargetValue = _SelectedMatch.Groups[^1].Value;
                                        break;
                                }
                            }

                            // If price doesn't contains decimal separator, add it
                            if (aApplyPriceTransformationToTargetValue && !_TargetValue.Contains('.') && !_TargetValue.Contains(',') && _TargetValue.Length > 2)
                                _TargetValue = _TargetValue.Insert(_TargetValue.Length - 2, ".");

                            if (!string.IsNullOrWhiteSpace(_TargetValue) && double.TryParse(_TargetValue, out double _PriceAsDouble))
                                return _PriceAsDouble;
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

            return null;
        }

        private void TryFetchProductPriceByDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
            {
                LoggingManager.LogToFile("1A1AF5EE-BDFA-4F71-A698-935341E133AD", $"Trying to fetch shipping cost of [{fUrl}], but no DOM was found on this Url.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            ProductDOMParserItem[] _PriceParsers = fDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray();

            LoggingManager.LogToFile("D07C90BF-8570-4BE6-A98A-2EFB20322A4A", $"Trying to fetch price of [{fUrl}] using [{_PriceParsers.Length}] DOM parsers / {fDOMParserItems.Count} for this domain.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            double? _Price = DOMProductInformationParser.ParseDouble(fUrl, this.DOM, _PriceParsers);

            if (_Price.HasValue)
            {
                this.Information.Price = _Price;
                LoggingManager.LogToFile("2F91F50B-C73E-454D-A2EC-5705377890D8", $"DOM parser step to fetch price of product [{fUrl}] succeeded.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
            }

            if (!this.Information.Price.HasValue)
                LoggingManager.LogToFile("2CCDD325-3050-4FA5-A9F5-F9331A155C4F", $"DOM parser step to fetch price of product [{fUrl}] failed.");
        }

        private void TryFetchProductShippingCostByDOM()
        {
            if (string.IsNullOrWhiteSpace(this.DOM))
            {
                LoggingManager.LogToFile("D1E84B62-052B-4AB4-BF9A-3075145ED459", $"Trying to fetch shipping cost of [{fUrl}], but no DOM was found on this Url.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
                return;
            }

            ProductDOMParserItem[] _ShippingCostParsers = fDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.ShippingCost).ToArray();

            if (_ShippingCostParsers.Length <= 0)
                return;

            LoggingManager.LogToFile("B56BFC7A-0E62-4B85-87A9-7C5F007D0B35", $"Trying to fetch shipping cost of [{fUrl}] using [{_ShippingCostParsers.Length}] DOM parsers / {fDOMParserItems.Count} for this domain.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

            double? _ShippingCost = DOMProductInformationParser.ParseDouble(fUrl, this.DOM, _ShippingCostParsers);

            if (_ShippingCost.HasValue)
                this.Information.ShippingCost = _ShippingCost;

            if (!this.Information.ShippingCost.HasValue)
                LoggingManager.LogToFile("8D020C36-2542-4944-B41B-17D9931E6915", $"DOM parser step to fetch price of product [{fUrl}] failed.");
            else
                LoggingManager.LogToFile("CEB74E01-5C23-4547-99AC-008E75116D35", "Product shipping cost was fetched using DOM parser", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);
        }

        private double TryFetchSumOfAllDiscountPriceFromManualOperations(ProductParserItemTarget aProductParserItemTarget)
        {
            double _TotalAmount = 0;
            try
            {
                ManualScraperProductParser[] _AvailableProductParser = fManualScraperItems.Where(w => w.Target == aProductParserItemTarget).ToArray();
                LoggingManager.LogToFile("B0618052-EBA5-4BB1-ABF5-B0738AA52E2B", $"Trying to fetch {aProductParserItemTarget} of [{fUrl}] using [{_AvailableProductParser.Length}] Manual Product Parsers.", aLogVerbosity: LoggingManager.LogVerbosity.Verbose);

                foreach (ManualScraperProductParser _ManualScraperItemParser in _AvailableProductParser)
                {
                    double? _Amount = TryFetchProductParserTargetByManualOperationBySpecificParser(_ManualScraperItemParser, true, true);

                    if (_Amount.HasValue)
                        _TotalAmount += _Amount.Value;
                }
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("D17DCA12-0872-4F45-AB00-120259233C8F", $"Couldn't reproduce steps for manual operation in [{nameof(ProductTargetScraper)}].", _Ex);
                throw;
            }

            return _TotalAmount;
        }

        private void WriteProductInformationToErrorFolder(string aErrorMessage)
        {
            try
            {
                DateTime _Now = DateTime.Now;

                // Save Screenshot and DOM in error folder
                string _RootFolder = "Errors";

                // Add website domain to rootFolder
                if (!string.IsNullOrWhiteSpace(fUrl))
                    _RootFolder = Path.Combine(_RootFolder, RegexUtils.GetWebSiteDomain(fUrl));

                // Add current date to root folder
                _RootFolder = Path.Combine(_RootFolder, $"{_Now:yyyy-MM-dd}");

                if (!Directory.Exists(_RootFolder))
                    Directory.CreateDirectory(_RootFolder);

                string _FileFormat = "HH.mm.ss";

                if (!string.IsNullOrWhiteSpace(this.DOM))
                    File.WriteAllText(Path.Combine(_RootFolder, $"{_Now:_FileFormat}-DOM.txt"), this.DOM);

                if (this.Screenshot != null && this.Screenshot.Length > 0)
                    File.WriteAllBytes(Path.Combine(_RootFolder, $"{_Now:_FileFormat}.png"), this.Screenshot);

                // Log other relevant information to a single file
                File.WriteAllText(Path.Combine(_RootFolder, $"{_Now:_FileFormat}-INFO.txt"),
                    @$"Url: {fUrl}{Environment.NewLine}
OneOrMoreStepFailedAndRequiredAFallback: {this.OneOrMoreStepFailedAndRequiredAFallback}{Environment.NewLine}
this.Information: {Environment.NewLine}{JsonCommonSerializer.SerializeToString(this.Information)}{Environment.NewLine}{aErrorMessage}{Environment.NewLine}
");
            } catch (Exception _Ex)
            {
                LoggingManager.LogToFile("6C69E0C6-6C77-4C91-B4D8-FF9EFDA88129", "Couldn't write fail files on disk.", _Ex);
            }
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public override void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems)
        {
            fDOMParserItems = aDOMParserItems?.OfType<ProductDOMParserItem>().ToList() ?? new List<ProductDOMParserItem>();
            fManualScraperItems = aManualScraperItems?.OfType<ManualScraperProductParser>().Where(w => aUrl.ToLowerInvariant().Contains(w.IfUrlContains.ToLowerInvariant())).ToList() ?? new List<ManualScraperProductParser>();

            base.Evaluate(aSupportedBrowser, aUrl, aAllowManualOperations, aManualScraperItems, aDOMParserItems);

            // Refine DOM Parsers with the information we got from parents
            fDOMParserItems = fDOMParserItems.Where(w => fUrl.Contains(w.IfUrlContains, StringComparison.InvariantCultureIgnoreCase)).ToList();

            WaitForBrowserInputsReadyOrMax(1000);

            // Fetch product information
            FetchProductInformation();

            if (this.Information.OutOfStock == true)
            {
                LoggingManager.LogToFile("A7E20E42-265C-44B0-98B8-EC2A90C47BE4", "Product is out of stock. Ignoring failure handling process.");
                return;
            }

            if (!this.Information.Price.HasValue)
            {
                HandleFailureProcess();
                return;
            }

            // Validate fetched information with DOM parser to check if we should inform Admins that a configuration may be incorrect
            double? _Price = DOMProductInformationParser.ParseDouble(fUrl, this.DOM, fDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price).ToArray());

            if (!_Price.HasValue)
            {
                LoggingManager.LogToFile("5CFCB97E-5DD2-467D-A555-6967F2ADD23A", $"Price couldn't be fetched from DOM. [{fUrl}]");
            }

            if (this.OneOrMoreStepFailedAndRequiredAFallback || (_Price.HasValue && Math.Abs(this.Information.Price.Value - _Price.Value) >= 0.01))
            {
                string _ErrorMessage = $"Error. Price fetched from scrapper [{this.Information.Price}] is different from price fetched from DOM parser [{_Price}]. Price [{_Price}] will be considered as the right one. Abs Diff was [{Math.Abs(this.Information.Price.Value - _Price ?? 0)}].";
                LoggingManager.LogToFile("3D62E30F-4D4D-4A64-8EC7-09C060D7D4AF", _ErrorMessage);

                CreateErrorNotificationForAdministration($"<p>The price fetched was different from DOM parser price fetched.</p><p>this.OneOrMoreStepFailedAndRequiredAFallback = {this.OneOrMoreStepFailedAndRequiredAFallback}</p><p>_Price.HasValue={_Price.HasValue}</p><p>this.Information.Price.Value={this.Information.Price}</p><p>_Price.Value(by DOM only)={_Price}</p><p>Shipping Cost: {this.Information.ShippingCost}</p><p>Assuming product price is {_Price}. If it's not, please update database accordingly.</p>");

                // We consider DOM parser better than manual steps that can fail more easily. Ex: Amazon price could be $299.99 without "Price: " and we mismatch the next "Price: 314.14" we found. True case = https://www.amazon.ca/ADAM-Audio-Two-Way-Nearfield-Monitor/dp/B07B6JXBZH
                this.Information.Price = _Price;

                WriteProductInformationToErrorFolder(_ErrorMessage);
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
