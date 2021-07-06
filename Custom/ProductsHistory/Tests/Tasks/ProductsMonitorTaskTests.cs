using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Custom.ProductsHistory.DataBase;
using Radiant.Custom.ProductsHistory.Tasks;
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

            var _Product2 = new Product
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

            AddProduct1();
            Assert.Equal(1, _DataBaseContext.Products.Count());

            ProductsMonitorTask _Task = new ProductsMonitorTask
            {
                IsEnabled = true
            };
            _Task.ForceTriggerNow();

            _DataBaseContext.Entry(_DataBaseContext.Products.Single()).Collection(c => c.ProductHistoryCollection).Load();
            Assert.Equal(1, _DataBaseContext.Products.Single().ProductHistoryCollection.Count);

            // Clean up
            RemoveAllProducts();
            Assert.Equal(0, _DataBaseContext.Products.Count());
        }
    }
}
