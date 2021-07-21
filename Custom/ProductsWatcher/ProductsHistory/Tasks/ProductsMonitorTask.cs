﻿using System;
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
        private RadiantProductHistoryModel CreateProductHistoryFromProductTargetScraper(ProductTargetScraper aProductScraper, RadiantProductModel aProduct, string aProductTitle)
        {
            if (aProductScraper?.Information?.Price == null)
                return null;

            // Check that the Title match. Some website (cough couch Amazon) keep the SAME url, but change the product..
            if (!string.IsNullOrWhiteSpace(aProductTitle) && !string.Equals(aProductScraper.Information.Title, aProductTitle, StringComparison.CurrentCultureIgnoreCase))
            {
                LoggingManager.LogToFile("E24BD4BD-0AE7-41B5-BF66-1D703B75905A", $"Product Id [{aProduct.ProductId}] was fetched but Title was mismatching. Title expected: [{aProductTitle}] but found [{aProductScraper.Information.Title?.Trim()}].");

                return null;
            }

            return new RadiantProductHistoryModel
            {
                InsertDateTime = DateTime.Now,
                Price = aProductScraper.Information.Price.Value,
                Title = aProductScraper.Information.Title.Trim()
            };
        }

        private void EvaluateEmailNotifications(RadiantProductModel aProduct, RadiantProductHistoryModel aNewProductHistory, RadiantProductSubscriptionModel[] aSubscriptionModels)
        {
            RadiantProductSubscriptionModel[] _EmailSubscriptions = aSubscriptionModels.Where(w => w.SendEmailOnNotification).ToArray();

            if (_EmailSubscriptions.Length <= 0)
                return;

            RadiantNotificationModel _NewNotification = new()
            {
                Content = $"<p>Product {aProduct.Name} is {aNewProductHistory.Price}$</p> <p>Url: {aProduct.Url}</p>",
                Subject = $"Deal on {aProduct.Name} {aNewProductHistory.Price}$",
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
        private void EvaluateNotifications(RadiantProductModel aProduct, RadiantProductHistoryModel aNewProductHistory)
        {
            // If price is the same as the last one, skip
            double _LastPrice = aProduct.ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).FirstOrDefault()?.Price ?? -1;

            if (Math.Abs(aNewProductHistory.Price - _LastPrice) < 0.01)
                return;

            // No notification if it's the first fetch
            if (!aProduct.ProductHistoryCollection.Any())
                return;

            double _BestPriceLastYear = aNewProductHistory.Price;
            RadiantProductHistoryModel[] _ProductsHistory = aProduct.ProductHistoryCollection.Where(w => w.InsertDateTime >= DateTime.Now.AddYears(-1)).ToArray();

            if (_ProductsHistory.Any())
                _BestPriceLastYear = _ProductsHistory.Min(m => m.Price);

            // Check if the price is low enough to send notifications. Each user may have a watcher on this product with a different price, so we need to check all subscriptions
            using ProductsDbContext _ProductDbContext = new();
            _ProductDbContext.Users.Load();
            RadiantProductSubscriptionModel[] _SubscriptionsOnCurrentProduct = _ProductDbContext.Subscriptions.Where(w =>
                w.Product.Equals(aProduct) &&
                w.MaximalPriceForNotification >= aNewProductHistory.Price &&
                aNewProductHistory.Price <= _BestPriceLastYear + _BestPriceLastYear / 100 * w.BestPricePercentageForNotification).ToArray();

            // Create email Notification model
            EvaluateEmailNotifications(aProduct, aNewProductHistory, _SubscriptionsOnCurrentProduct);

            // TODO: Other means of notifications
        }

        private void EvaluateProduct(RadiantProductModel aProduct)
        {
            if (aProduct == null)
                return;

            DateTime _Now = DateTime.Now;
            if (aProduct.ProductHistoryCollection == null || aProduct.ProductHistoryCollection.Count <= 0)
            {
                FetchNewProductHistory(aProduct, _Now);
                return;
            }

            if (!aProduct.NextFetchProductHistory.HasValue)
                UpdateNextFetchDateTime(aProduct, _Now);

            if (_Now > aProduct.NextFetchProductHistory.Value)
                FetchNewProductHistory(aProduct, _Now);
        }

        private void FetchNewProductHistory(RadiantProductModel aProduct, DateTime aNow)
        {
            ManualScraper _ManualScraper = new();
            ProductTargetScraper _ProductScraper = new(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            List<ManualScraperItemParser> _ManualScrapers = _Config.ManualScraperSequenceItems.Select(s => (ManualScraperItemParser)s).ToList();
            List<DOMParserItem> _DomParsers = _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList();

            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, aProduct.Url, _ProductScraper, _ManualScrapers, _DomParsers);

            RadiantProductHistoryModel? _MostRecentProductHistory = aProduct.ProductHistoryCollection.FirstOrDefault(f => f.InsertDateTime == aProduct.ProductHistoryCollection.Max(m => m.InsertDateTime));

            RadiantProductHistoryModel _ProductHistory = CreateProductHistoryFromProductTargetScraper(_ProductScraper, aProduct, _MostRecentProductHistory?.Title);

            if (_ProductHistory == null)
            {
                LoggingManager.LogToFile("988B416D-BE97-42A3-BA2F-438FFBFEDAF4", $"Couldn't fetch new product history of product [{aProduct.Name}] Url [{aProduct.Url}].");

                // Note that when a product fetch fails, the ProductTargetScraper will handle the fail sequence to send notifications to admins, logging relevant informations about failure, etc.

                // To avoid a query loop
                UpdateNextFetchDateTime(aProduct, aNow);

                return;
            }

            // Handle notifications
            EvaluateNotifications(aProduct, _ProductHistory);

            aProduct.ProductHistoryCollection.Add(_ProductHistory);

            // Update product in db to set the next trigger time
            UpdateNextFetchDateTime(aProduct, aNow);
        }

        private void UpdateNextFetchDateTime(RadiantProductModel aProduct, DateTime aNow)
        {
            if (aProduct == null || !aProduct.FetchProductHistoryEnabled)
                return;

            Random _Random = new Random();
            int _Modifier = _Random.Next(-100, 100);
            float _NoisePercValue = aProduct.FetchProductHistoryTimeSpanNoiseInPerc * ((float)_Modifier / 100);
            TimeSpan _NoiseValue = aProduct.FetchProductHistoryEveryX / 100 * (_NoisePercValue + 100);
            DateTime _NextFetchDateTime = aNow + _NoiseValue;

            aProduct.NextFetchProductHistory = _NextFetchDateTime;
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: to implement fetch the next product to update and update it if the time is right
            Thread.Sleep(1000);
            using var _DataBaseContext = new ProductsDbContext();

            RadiantProductModel? _ProductToUpdate = _DataBaseContext.Products.OrderByDescending(o => o.ProductHistoryCollection.Min(m => m.InsertDateTime)).FirstOrDefault();

            if (_ProductToUpdate == null)
                return;

            foreach (RadiantProductModel _Product in _DataBaseContext.Products.Where(w => w.FetchProductHistoryEnabled).ToArray())
            {
                // Load specific product histories
                _DataBaseContext.Entry(_Product).Collection(c => c.ProductHistoryCollection).Load();
                EvaluateProduct(_Product);
                _DataBaseContext.SaveChanges();

                // Mandatory wait between each products (bot tagging)
                Thread.Sleep(5000);
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