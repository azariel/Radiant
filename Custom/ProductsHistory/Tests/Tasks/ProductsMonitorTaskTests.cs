using System;
using System.Linq;
using Radiant.Custom.ProductsHistory.DataBase;
using Radiant.Custom.ProductsHistory.Tasks;
using Radiant.Notifier;
using Radiant.Notifier.DataBase;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.Tasks
{
    public class ProductsMonitorTaskTests
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void AddProduct1()
        {
            using var _DataBaseContext = new ProductsDbContext();

            var _Product2 = new ProductModel
            {
                Name = "TestProductName",
                InsertDateTime = DateTime.Now.AddMinutes(-11),
                FetchProductHistoryEnabled = true,
                FetchProductHistoryEveryX = new TimeSpan(0, 10, 0),
                FetchProductHistoryTimeSpanNoiseInPerc = 2.5f,
                Url = "https://www.amazon.ca/PlayStation-DualSense-Wireless-Controller-Midnight/dp/B0951JZDWT"
            };

            _DataBaseContext.Products.Add(_Product2);
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
            using var _DataBaseContext = new ProductsDbContext();
            _DataBaseContext.Products.RemoveRange(_DataBaseContext.Products);
            _DataBaseContext.SaveChanges();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void EvaluateBasicAmazonProduct()
        {
            using var _DataBaseContext = new ProductsDbContext();
            RemoveAllProducts();
            RemoveAllNotifications();

            AddProduct1();
            Assert.Equal(1, _DataBaseContext.Products.Count());

            ProductsMonitorTask _Task = new ProductsMonitorTask
            {
                IsEnabled = true
            };
            _Task.ForceTriggerNow();

            _DataBaseContext.Entry(_DataBaseContext.Products.Single()).Collection(c => c.ProductHistoryCollection).Load();
            Assert.Equal(1, _DataBaseContext.Products.Single().ProductHistoryCollection.Count);

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
