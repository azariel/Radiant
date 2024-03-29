﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Database.Common;
using Radiant.Common.Tests;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Tasks;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions;
using Radiant.Notifier;
using Radiant.Notifier.DataBase;
using Xunit;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Tests.Tasks
{
    public class ProductsMonitorTaskTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void AddProduct1User1AndSubscriptionToProduct1()
        {
            using var _DataBaseContext = new ServerProductsDbContext();

            var _Product1 = new RadiantServerProductModel
            {
                Name = "TestProductName",
                FetchProductHistoryEnabled = true,
                ProductDefinitionCollection = new List<RadiantServerProductDefinitionModel>
                {
                    new()
                    {
                        InsertDateTime = DateTime.UtcNow.AddMinutes(-1),
                        FetchProductHistoryEnabled = true,
                        FetchProductHistoryEveryX = new TimeSpan(0, 0, 1),
                        FetchProductHistoryTimeSpanNoiseInPerc = 2.5f,
                        NextFetchProductHistory = DateTime.UtcNow,
                        Url = "https://www.amazon.ca/PlayStation-DualSense-Wireless-Controller-Midnight/dp/B0951JZDWT"
                    }
                }
            };

            _DataBaseContext.Products.Add(_Product1);
            _DataBaseContext.SaveChanges();

            var _User = new RadiantServerUserProductsHistoryModel
            {
                Email = RadiantCommonUnitTestsConstants.EMAIL,
                Password = "passwordEC7FAB53-8D76-4561-8452-F6D0AA013045",
                Type = RadiantUserModel.UserType.Admin,
                UserName = "username31770FB5-981F-469A-B453-11AF855D5A2A"
            };

            _DataBaseContext.Users.Add(_User);
            _DataBaseContext.SaveChanges();

            var _Sub = new RadiantServerProductSubscriptionModel()
            {
                Product = _Product1,
                User = _User,
                BestPricePercentageForNotification = 10,
                MaximalPriceForNotification = 1000,
                SendEmailOnNotification = true,
            };

            _DataBaseContext.Subscriptions.Add(_Sub);
            _DataBaseContext.SaveChanges();
        }

        private void RemoveAllNotifications()
        {
            using NotificationsDbContext _DbContext = new();

            foreach (RadiantNotificationModel _Notification in _DbContext.Notifications)
                _DbContext.Notifications.Remove(_Notification);

            _DbContext.SaveChanges();
        }

        private void RemoveAllProducts()
        {
            using var _DataBaseContext = new ServerProductsDbContext();
            _DataBaseContext.Products.RemoveRange(_DataBaseContext.Products);
            _DataBaseContext.SaveChanges();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void EvaluateBasicAmazonProduct()
        {
            using var _DataBaseContext = new ServerProductsDbContext();
            RemoveAllProducts();
            RemoveAllNotifications();

            AddProduct1User1AndSubscriptionToProduct1();
            Assert.Equal(1, _DataBaseContext.Products.Count());

            ProductsMonitorTask _Task = new ProductsMonitorTask
            {
                IsEnabled = true
            };
            _Task.ForceTriggerNow(null, null);

            _DataBaseContext.ProductsHistory.Load();
            _DataBaseContext.ProductDefinitions.Load();

            //_DataBaseContext.Entry(_DataBaseContext.Products.Single()).Collection(c => c.ProductDefinitionCollection).Load();
            Assert.Equal(1, _DataBaseContext.Products.Single().ProductDefinitionCollection.Count);
            Assert.Equal(1, _DataBaseContext.Products.Single().ProductDefinitionCollection.Single().ProductHistoryCollection.Count);

            // The very first time a product is fetched, no notification should occur
            using (NotificationsDbContext _DbContext = new())
            {
                Assert.Equal(0, _DbContext.Notifications.Count());
            }

            // Bump the price. We will trig again and we want a notification to be created. If the price remains the same, no notification are created
            _DataBaseContext.ProductsHistory.First().Price = 256;
            _DataBaseContext.SaveChanges();

            // Trig again to create a notification
            _Task.ForceTriggerNow(null, null);

            // Check that notification was created
            using (NotificationsDbContext _DbContext = new())
            {
                Assert.Equal(1, _DbContext.Notifications.Count());
                Assert.False(_DbContext.Notifications.Single().Sent);
            }

            // Evaluate notifications
            RadiantNotificationsManager.EvaluateNotificationsInStorage();

            using (NotificationsDbContext _DbContext = new())
            {
                Assert.Equal(1, _DbContext.Notifications.Count());
                Assert.True(_DbContext.Notifications.Single().Sent);
            }

            // Clean up
            RemoveAllProducts();
            RemoveAllNotifications();
            Assert.Equal(0, _DataBaseContext.Products.Count());
        }
    }
}
