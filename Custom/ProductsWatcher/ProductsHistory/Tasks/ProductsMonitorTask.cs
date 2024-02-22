using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Common;
using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Configuration;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Scraper;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions;
using Radiant.Notifier.DataBase;
using Radiant.WebScraper.RadiantWebScraper;
using Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper.Manual;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Tasks
{
    public class ProductsMonitorTask : RadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private RadiantServerProductHistoryModel CreateProductHistoryFromProductTargetScraper(ProductTargetScraper aProductScraper, RadiantServerProductDefinitionModel aProduct, string aProductTitle)
        {
            // Note that is the product is out of stock, we consider the fetched information to be wrong. No point in having an awesome price on a product that isn't available
            if (aProductScraper?.Information?.Price == null || aProductScraper.Information.OutOfStock == true)
                return null;

            // Check that the Title match. Some website (cough couch Amazon) keep the SAME url, but change the product..
            if (!string.IsNullOrWhiteSpace(aProductTitle) && !string.Equals(aProductScraper.Information.Title, aProductTitle, StringComparison.CurrentCultureIgnoreCase))
            {
                LoggingManager.LogToFile("E24BD4BD-0AE7-41B5-BF66-1D703B75905A", $"Product Id [{aProduct.ProductId}] was fetched but Title was mismatching. Title expected: [{aProductTitle}] but found [{aProductScraper.Information.Title?.Trim()}].");

                // Send a notification to admins
                RadiantNotificationModel _NewNotification = new()
                {
                    Content = $"<p>Product {aProduct.Product.Name} may be mismatched</p><p>Please check that the product hasn't changed.</p><p>Expected: {aProductScraper.Information.Title}</p><p>Found: {aProductTitle}</p><p>{aProduct.Url}</p>",
                    Subject = $"Product {aProduct.Product.Name} may be mismatched",
                    EmailFrom = "Radiant Product History",
                    MinimalDateTimetoSend = DateTime.Now
                };

                using ServerProductsDbContext _DbContext = new();
                _DbContext.Users.Load();
                _NewNotification.EmailTo.AddRange(_DbContext.Users.Where(w => w.Type == RadiantUserModel.UserType.Admin).Select(s => s.Email));

                using NotificationsDbContext _NotificationDbContext = new();
                _NotificationDbContext.Notifications.Add(_NewNotification);
                _NotificationDbContext.SaveChanges();
            }

            return new RadiantServerProductHistoryModel
            {
                InsertDateTime = DateTime.Now,
                Price = aProductScraper.Information.Price.Value,
                ShippingCost = aProductScraper.Information.ShippingCost,
                DiscountPrice = aProductScraper.Information.DiscountPrice,
                DiscountPercentage = aProductScraper.Information.DiscountPercentage,
                Title = aProductScraper.Information.Title?.Trim()
            };
        }

        private void EvaluateEmailNotifications(RadiantServerProductDefinitionModel aProductDefinition, RadiantServerProductHistoryModel aNewProductHistory, RadiantServerProductSubscriptionModel[] aSubscriptionModels, double aBestPriceLastYear)
        {
            RadiantServerProductSubscriptionModel[] _EmailSubscriptions = aSubscriptionModels.Where(w => w.SendEmailOnNotification).ToArray();

            if (_EmailSubscriptions.Length <= 0)
                return;

            RadiantNotificationModel _NewNotification = new()
            {
                Content = $"<p>Product {aProductDefinition.Product.Name} is {aNewProductHistory.Price:F}$</p><p>Shipping Cost: {aNewProductHistory.ShippingCost:F}$</p> <p>Shown total discount: {aNewProductHistory.DiscountPrice:F}$ and {aNewProductHistory.DiscountPercentage:F}%</p><p>Best price for last 365 days: {aBestPriceLastYear:F}$</p><p>Url: {aProductDefinition.Url}</p>",
                Subject = $"{aNewProductHistory.Price:F}$ -> {aProductDefinition.Product.Name}",
                EmailFrom = "Radiant - Product History",
                MinimalDateTimetoSend = DateTime.Now
            };

            // Add all subscribed users email to notification EmailTo
            _NewNotification.EmailTo.AddRange(_EmailSubscriptions.Select(s => s.User.Email));

            using NotificationsDbContext _NotificationDbContext = new();
            _NotificationDbContext.Notifications.Add(_NewNotification);
            _NotificationDbContext.SaveChanges();
        }

        private void EvaluateNotifications(RadiantServerProductDefinitionModel aProductDefinition, RadiantServerProductHistoryModel aNewProductHistory)
        {
            // No notification if it's the first fetch for this product
            if (!aProductDefinition.Product.ProductDefinitionCollection.Any(a => a.ProductHistoryCollection.Count > 0))
                return;

            // Notification if it's the first fetch for this Url AND we're the same day
            if (!aProductDefinition.ProductHistoryCollection.Any() && (DateTime.Now - aProductDefinition.InsertDateTime).TotalHours < 24)
                return;

            // If price is the same as the last one, skip
            double? _LastPrice = null;

            foreach (RadiantServerProductDefinitionModel _ProductDefinition in aProductDefinition.Product.ProductDefinitionCollection)
            {
                var _LatestProductHistory = _ProductDefinition.ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).FirstOrDefault();
                double _TempDiscountedPrice = (_LatestProductHistory?.Price ?? 0) - (_LatestProductHistory?.DiscountPrice ?? 0);
                double? _LastPriceOfThisDefinition = _LatestProductHistory?.Price + (_LatestProductHistory?.ShippingCost ?? 0) - (_LatestProductHistory?.DiscountPrice ?? 0) - (_TempDiscountedPrice * _LatestProductHistory?.DiscountPercentage ?? 0);

                if (_LastPriceOfThisDefinition.HasValue && (!_LastPrice.HasValue || _LastPrice > _LastPriceOfThisDefinition))
                    _LastPrice = _LastPriceOfThisDefinition;
            }

            // If we haven't a price in the db for this definition or if it's the last price is the same price we just found, avoid notification
            double _CurrentPrice = Math.Round(aNewProductHistory.Price, 2);

            if (aNewProductHistory.ShippingCost.HasValue)
                _CurrentPrice += Math.Round(aNewProductHistory.ShippingCost.Value, 2);

            if (aNewProductHistory.DiscountPrice.HasValue)
                _CurrentPrice -= Math.Round(aNewProductHistory.DiscountPrice.Value, 2);

            if (aNewProductHistory.DiscountPercentage.HasValue)
                _CurrentPrice -= _CurrentPrice * Math.Round(aNewProductHistory.DiscountPercentage.Value, 2);

            // If current price is the same, ignore
            if (!_LastPrice.HasValue || Math.Abs(Math.Round(Math.Round(_CurrentPrice, 2) - Math.Round(_LastPrice.Value, 2), 2)) < 0.01)
                return;

            // If the current price is higher than the very last one, don't send notification..
            if (_LastPrice.Value < _CurrentPrice)
                return;

            double _BestPriceLastYear = _CurrentPrice;
            RadiantServerProductHistoryModel[] _ProductsHistory = aProductDefinition.Product.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection.Where(w => w.InsertDateTime >= DateTime.Now.AddYears(-1))).ToArray();

            if (_ProductsHistory.Any())
                _BestPriceLastYear = _ProductsHistory.Min(m => m.Price + (m.ShippingCost ?? 0) - (m.DiscountPrice ?? 0) - ((m.Price - (m.DiscountPrice ?? 0)) * m.DiscountPercentage ?? 0));

            // Check if the price is low enough to send notifications. Each user may have a watcher on this product with a different price, so we need to check all subscriptions
            using ServerProductsDbContext _ProductDbContext = new();
            _ProductDbContext.Users.Load();
            _ProductDbContext.Subscriptions.Load();

            RadiantServerProductSubscriptionModel[] _SubscriptionsOnCurrentProduct = _ProductDbContext.Subscriptions.Where(w =>
                w.Product.Equals(aProductDefinition.Product) &&
                w.MaximalPriceForNotification >= aNewProductHistory.Price &&
                _CurrentPrice <= _BestPriceLastYear + (_BestPriceLastYear / 100 * w.BestPricePercentageForNotification)).ToArray();

            // Create email Notification model
            EvaluateEmailNotifications(aProductDefinition, aNewProductHistory, _SubscriptionsOnCurrentProduct, _BestPriceLastYear);

            // TODO: Other means of notifications
        }

        private void EvaluateProductDefinition(RadiantServerProductDefinitionModel aProductDefinition)
        {
            if (aProductDefinition == null)
                return;

            DateTime _Now = DateTime.Now;

            // If product was never fetch, set the next fetch time to right now
            if (!aProductDefinition.NextFetchProductHistory.HasValue)
                UpdateNextFetchDateTime(aProductDefinition, _Now);

            if (_Now > aProductDefinition.NextFetchProductHistory)
                FetchNewProductHistory(aProductDefinition, _Now);
        }

        private void FetchNewProductHistory(RadiantServerProductDefinitionModel aProductDefinition, DateTime aNow)
        {
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            ManualScraper _ManualScraper = new();
            ProductTargetScraper _ProductScraper = new(ManualBaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            List<IScraperItemParser> _ManualScrapers = _Config.ManualScraperSequenceItems.Select(s => (IScraperItemParser)s).ToList();
            List<DOMParserItem> _DomParsers = _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList();

            _ManualScraper.GetTargetValueFromUrl(Browser.Firefox, aProductDefinition.Url, _ProductScraper, _ManualScrapers, _DomParsers);
            fShouldStop = true;// Using the manual scraper takes a long time, we want to avoid processing too many products to avoid going outside a blackout timezone/inactivity trigger incoherence

            RadiantServerProductHistoryModel? _MostRecentProductHistory = aProductDefinition.ProductHistoryCollection.FirstOrDefault(f => f.InsertDateTime == aProductDefinition.ProductHistoryCollection.Max(m => m.InsertDateTime));

            RadiantServerProductHistoryModel _ProductHistory = CreateProductHistoryFromProductTargetScraper(_ProductScraper, aProductDefinition, _MostRecentProductHistory?.Title);

            if (_ProductHistory == null)
            {
                LoggingManager.LogToFile("988B416D-BE97-42A3-BA2F-438FFBFEDAF4", $"Couldn't fetch new product history of product [{aProductDefinition.Product.Name}] Url [{aProductDefinition.Url}]. {(_ProductScraper.Information.OutOfStock == true ? "Product was Out of Stock. Skipping it." : "")}");
                LoggingManager.LogToFile("ADCB5E84-034F-4A7F-BE21-784D5DBC4A77", $"Product [{aProductDefinition.Product.Name}] next update is scheduled on [{aProductDefinition.NextFetchProductHistory}].");

                // Note that when a product fetch fails, the ProductTargetScraper will handle the fail sequence to send notifications to admins, logging relevant informations about failure, etc.

                // To avoid a query loop
                UpdateNextFetchDateTime(aProductDefinition, aNow);

                return;
            }

            // Handle notifications
            if (_ProductScraper.Information.OutOfStock != true)// Never send a notification on an out of stock product
                EvaluateNotifications(aProductDefinition, _ProductHistory);

            aProductDefinition.ProductHistoryCollection.Add(_ProductHistory);

            // Update product in db to set the next trigger time
            UpdateNextFetchDateTime(aProductDefinition, aNow);
        }

        private void UpdateNextFetchDateTime(RadiantServerProductDefinitionModel aProductDefinition, DateTime aNow)
        {
            if (aProductDefinition == null || !aProductDefinition.FetchProductHistoryEnabled)
                return;

            Random _Random = new Random();
            int _Modifier = _Random.Next(-100, 100);
            float _NoisePercValue = aProductDefinition.FetchProductHistoryTimeSpanNoiseInPerc * ((float)_Modifier / 100);
            TimeSpan _NoiseValue = aProductDefinition.FetchProductHistoryEveryX / 100;
            _NoiseValue *= (_NoisePercValue + 100);
            DateTime _NextFetchDateTime = aNow + _NoiseValue;

            aProductDefinition.NextFetchProductHistory = _NextFetchDateTime;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: to implement fetch the next product to update and update it if the time is right
            using var _DataBaseContext = new ServerProductsDbContext();
            _DataBaseContext.Products.Load();
            _DataBaseContext.ProductDefinitions.Load();
            _DataBaseContext.ProductsHistory.Load();

            foreach (RadiantServerProductModel _Product in _DataBaseContext.Products.Where(w => w.FetchProductHistoryEnabled).ToArray())
            {
                foreach (RadiantServerProductDefinitionModel _ProductDefinition in _Product.ProductDefinitionCollection)
                {
                    EvaluateProductDefinition(_ProductDefinition);
                    _DataBaseContext.SaveChanges();

                    if (fShouldStop)
                        return;

                    // Mandatory wait between each products (1st very basic avoid bot tagging failsafe)
                    Thread.Sleep(500);
                }
            }
        }
    }
}
