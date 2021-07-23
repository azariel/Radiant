using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.Custom.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;
using Radiant.Notifier.DataBase;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.Custom.ProductsHistory.Tasks
{
    public class ProductsMonitorTask : RadiantTask
    {
        private RadiantServerProductHistoryModel CreateProductHistoryFromProductTargetScraper(ProductTargetScraper aProductScraper, RadiantServerProductDefinitionModel aProduct, string aProductTitle)
        {
            if (aProductScraper?.Information?.Price == null)
                return null;

            // Check that the Title match. Some website (cough couch Amazon) keep the SAME url, but change the product..
            if (!string.IsNullOrWhiteSpace(aProductTitle) && !string.Equals(aProductScraper.Information.Title, aProductTitle, StringComparison.CurrentCultureIgnoreCase))
            {
                LoggingManager.LogToFile("E24BD4BD-0AE7-41B5-BF66-1D703B75905A", $"Product Id [{aProduct.ProductId}] was fetched but Title was mismatching. Title expected: [{aProductTitle}] but found [{aProductScraper.Information.Title?.Trim()}].");

                return null;
            }

            return new RadiantServerProductHistoryModel
            {
                InsertDateTime = DateTime.Now,
                Price = aProductScraper.Information.Price.Value,
                Title = aProductScraper.Information.Title.Trim()
            };
        }

        private void EvaluateEmailNotifications(RadiantServerProductDefinitionModel aProductDefinition, RadiantServerProductHistoryModel aNewProductHistory, RadiantServerProductSubscriptionModel[] aSubscriptionModels)
        {
            RadiantServerProductSubscriptionModel[] _EmailSubscriptions = aSubscriptionModels.Where(w => w.SendEmailOnNotification).ToArray();

            if (_EmailSubscriptions.Length <= 0)
                return;

            RadiantNotificationModel _NewNotification = new()
            {
                Content = $"<p>Product {aProductDefinition.Product.Name} is {aNewProductHistory.Price}$</p> <p>Url: {aProductDefinition.Url}</p>",
                Subject = $"Deal on {aProductDefinition.Product.Name} {aNewProductHistory.Price}$",
                EmailFrom = "Radiant Product History",
                MinimalDateTimetoSend = DateTime.Now
            };

            // Add all subscribed users email to notification EmailTo
            _NewNotification.EmailTo.AddRange(_EmailSubscriptions.Select(s => s.User.Email));

            using NotificationsDbContext _NotificationDbContext = new();
            _NotificationDbContext.Notifications.Add(_NewNotification);
            _NotificationDbContext.SaveChanges();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
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
                double? _LastPriceOfThisDefinition = _ProductDefinition.ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).FirstOrDefault()?.Price;

                if (_LastPriceOfThisDefinition.HasValue && (!_LastPrice.HasValue || _LastPrice > _LastPriceOfThisDefinition))
                    _LastPrice = _LastPriceOfThisDefinition;
            }

            // If we haven't a price in the db for this definition or if it's the last price is the same price we just found, avoid notification
            if (!_LastPrice.HasValue || Math.Abs(Math.Round(Math.Round(aNewProductHistory.Price, 2) - Math.Round(_LastPrice.Value, 2), 2)) < 0.01)
                return;

            if (aNewProductHistory.Price >= _LastPrice.Value)
                return;

            double _BestPriceLastYear = aNewProductHistory.Price;
            RadiantServerProductHistoryModel[] _ProductsHistory = aProductDefinition.Product.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection.Where(w => w.InsertDateTime >= DateTime.Now.AddYears(-1))).ToArray();

            if (_ProductsHistory.Any())
                _BestPriceLastYear = _ProductsHistory.Min(m => m.Price);

            // Check if the price is low enough to send notifications. Each user may have a watcher on this product with a different price, so we need to check all subscriptions
            using ServerProductsDbContext _ProductDbContext = new();
            _ProductDbContext.Users.Load();
            _ProductDbContext.Subscriptions.Load();

            RadiantServerProductSubscriptionModel[] _SubscriptionsOnCurrentProduct = _ProductDbContext.Subscriptions.Where(w =>
                w.Product.Equals(aProductDefinition.Product) &&
                w.MaximalPriceForNotification >= aNewProductHistory.Price &&
                aNewProductHistory.Price <= _BestPriceLastYear + _BestPriceLastYear / 100 * w.BestPricePercentageForNotification).ToArray();

            // Create email Notification model
            EvaluateEmailNotifications(aProductDefinition, aNewProductHistory, _SubscriptionsOnCurrentProduct);

            // TODO: Other means of notifications
        }

        private void EvaluateProductDefinition(RadiantServerProductDefinitionModel aProductDefinition)
        {
            DateTime _Now = DateTime.Now;
            //if (aProductDefinition.ProductHistoryCollection == null || aProductDefinition.ProductHistoryCollection.Count <= 0)
            //{
            //    FetchNewProductHistory(aProductDefinition, _Now);
            //    return;
            //}

            if (!aProductDefinition.NextFetchProductHistory.HasValue)
                UpdateNextFetchDateTime(aProductDefinition, _Now);

            if (_Now > aProductDefinition.NextFetchProductHistory.Value)
                FetchNewProductHistory(aProductDefinition, _Now);
        }

        private void FetchNewProductHistory(RadiantServerProductDefinitionModel aProductDefinition, DateTime aNow)
        {
            ManualScraper _ManualScraper = new();
            ProductTargetScraper _ProductScraper = new(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            List<ManualScraperItemParser> _ManualScrapers = _Config.ManualScraperSequenceItems.Select(s => (ManualScraperItemParser)s).ToList();
            List<DOMParserItem> _DomParsers = _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList();

            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, aProductDefinition.Url, _ProductScraper, _ManualScrapers, _DomParsers);

            RadiantServerProductHistoryModel? _MostRecentProductHistory = aProductDefinition.ProductHistoryCollection.FirstOrDefault(f => f.InsertDateTime == aProductDefinition.ProductHistoryCollection.Max(m => m.InsertDateTime));

            RadiantServerProductHistoryModel _ProductHistory = CreateProductHistoryFromProductTargetScraper(_ProductScraper, aProductDefinition, _MostRecentProductHistory?.Title);

            if (_ProductHistory == null)
            {
                LoggingManager.LogToFile("988B416D-BE97-42A3-BA2F-438FFBFEDAF4", $"Couldn't fetch new product history of product [{aProductDefinition.Product.Name}] Url [{aProductDefinition.Url}].");

                // Note that when a product fetch fails, the ProductTargetScraper will handle the fail sequence to send notifications to admins, logging relevant informations about failure, etc.

                // To avoid a query loop
                UpdateNextFetchDateTime(aProductDefinition, aNow);

                return;
            }

            // Handle notifications
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
            TimeSpan _NoiseValue = aProductDefinition.FetchProductHistoryEveryX / 100 * (_NoisePercValue + 100);
            DateTime _NextFetchDateTime = aNow + _NoiseValue;

            aProductDefinition.NextFetchProductHistory = _NextFetchDateTime;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: to implement fetch the next product to update and update it if the time is right
            Thread.Sleep(1000);
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

                    // Mandatory wait between each products (bot tagging)
                    Thread.Sleep(5000);
                }
            }

            _DataBaseContext.SaveChanges();
        }

        //public void Register()
        //{
        //    RadiantConfig _test = new();
        //    _test.Tasks.Tasks.Add(new ProductsMonitorTask
        //    {
        //        Triggers = new List<ITrigger>
        //        {
        //            new ScheduleTrigger
        //            {
        //                TriggerEverySeconds = 10,
        //            }
        //        },
        //        IsEnabled = true
        //    });
        //    CommonConfigurationManager.SetConfigInMemory(_test);
        //    CommonConfigurationManager.SaveConfigInMemoryToDisk();
        //}
    }
}
