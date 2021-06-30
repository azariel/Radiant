using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Custom.ProductsHistory.DataBase;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.DataBase
{
    public class ProductsDbContextTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicConnectToDataBaseTest()
        {
            using var _DataBaseContext = new ProductsDbContext();
        }

        [Fact]
        public void BasicCRUDOperations()
        {
            using var _DataBaseContext = new ProductsDbContext();

            // Validate that the db is empty
            Assert.Equal(0, _DataBaseContext.Products.Count());

            DateTime _Now = DateTime.Now;
            string _Name1 = "ProductName-041BEB2A-A362-4139-9134-798B8B8AB770";
            string _Title1 = "UnitTestTitle-C3292360-2AD5-4399-93B9-4B68139F9E91";
            double _Price1 = 89.98;

            // Add a new product
            var _Product1 = new Product
            {
                Name = "ProductName-041BEB2A-A362-4139-9134-798B8B8AB770",
                InsertDateTime = _Now
            };
            _DataBaseContext.Products.Add(_Product1);

            // Assert that the product wasn't added as we haven't saved yet
            Assert.Equal(0, _DataBaseContext.Products.Count());

            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Products.Count());
            Assert.Equal(_Name1, _DataBaseContext.Products.First().Name);
            Assert.Equal(0, _DataBaseContext.Products.First().ProductHistoryCollection.Count());

            // Add a second product with history
            string _Name2 = "ProductName-2520BC65-4DE7-45D5-83B6-3243B51064F0";
            string _Title2 = "UnitTestTitle-C2F867C4-9CAA-49F4-90B1-96752BCFCDAE";
            double _Price2 = 13.14;

            var _Product2 = new Product
            {
                Name = _Name2,
                InsertDateTime = _Now,
                FetchProductHistoryEnabled = true,
                FetchProductHistoryEveryX = new TimeSpan(0, 0, 10),
                FetchProductHistoryTimeSpanNoiseInPerc = 0
            };

            _Product2.ProductHistoryCollection.Add(new ProductHistory
            {
                InsertDateTime = _Now,
                Price = _Price2,
                Title = _Title2
            });

            _DataBaseContext.Products.Add(_Product2);

            _DataBaseContext.SaveChanges();
            _DataBaseContext.Dispose();

            // Switching to a brand new context to test navigation properties / FK / Lazy loading
            using var _DataBaseContext2 = new ProductsDbContext();

            Product _Product2FromStorage = _DataBaseContext2.Products.Single(s => s.Name == _Name2);

            // Load the product history of product 2
            _DataBaseContext2.Entry(_Product2FromStorage).Collection(c => c.ProductHistoryCollection).Load();

            Assert.NotEqual(null, _Product2FromStorage);
            Assert.Equal(_Now, _Product2FromStorage.InsertDateTime);
            Assert.Equal(_Price2, _Product2FromStorage.ProductHistoryCollection.Single().Price);
            Assert.Equal(_Title2, _Product2FromStorage.ProductHistoryCollection.Single().Title);

            // Update second product price
            var _LastProductHistory = _DataBaseContext2.Products.Single(w => w.Name == _Name2).ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).First();
            _LastProductHistory.Price = 12.11;
            _DataBaseContext2.SaveChanges();

            var _LastProductHistoryAfterModif = _DataBaseContext2.Products.Single(w => w.Name == _Name2).ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).First();

            Assert.Equal(12.11, _LastProductHistoryAfterModif.Price);

            // Clean up
            _DataBaseContext2.Products.Remove(_Product1);
            _DataBaseContext2.Products.Remove(_Product2FromStorage);
            _DataBaseContext2.SaveChanges();

            // Check that clean up worked
            Assert.Equal(0, _DataBaseContext2.Products.Count());
        }

        [Fact]
        public void AddProductPersistent()
        {
            using var _DataBaseContext = new ProductsDbContext();

            var _Product2 = new Product
            {
                Name = "TestProductName",
                InsertDateTime = DateTime.Now,
                FetchProductHistoryEnabled = true,
                FetchProductHistoryEveryX = new TimeSpan(0, 10, 0),
                FetchProductHistoryTimeSpanNoiseInPerc = 2.5f
            };

            _DataBaseContext.Products.Add(_Product2);
            _DataBaseContext.SaveChanges();
        }
    }
}
