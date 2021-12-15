using System;
using System.Collections.Generic;
using System.Linq;
using Radiant.Common.Database.Common;
using Radiant.Common.Tests;
using Radiant.Custom.ProductsHistoryCommon.DataBase;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;
using Xunit;

namespace Radiant.Custom.ProductsHistory.Tests.DataBase
{
    public class ProductsDbContextTests
    {
        private void RemoveAllProducts()
        {
            using ServerProductsDbContext _DbContext = new();

            foreach (var _Product in _DbContext.Products)
                _DbContext.Products.Remove(_Product);

            _DbContext.SaveChanges();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void RemoveAllUsers()
        {
            using ServerProductsDbContext _DbContext = new();

            foreach (RadiantServerUserProductsHistoryModel _User in _DbContext.Users)
                _DbContext.Users.Remove(_User);

            _DbContext.SaveChanges();
        }

        //[Fact(Skip = "Add a new Product in Database to Test ServerConsole or other client apps")]
        [Fact]
        public void AddTypicalProductAndDependencesPersistent()
        {
            RemoveAllProducts();

            using var _DataBaseContext = new ServerProductsDbContext();

            // Add product
            var _Product = new RadiantServerProductModel
            {
                FetchProductHistoryEnabled = true,
                Name = "TestProductName"
            };
            _DataBaseContext.Products.Add(_Product);

            var _ProductDefinition = new RadiantServerProductDefinitionModel
            {
                FetchProductHistoryEnabled = true,
                FetchProductHistoryEveryX = new TimeSpan(0, 10, 0),
                FetchProductHistoryTimeSpanNoiseInPerc = 10.0f,
                NextFetchProductHistory = DateTime.Now,
                Url = "https://www.amazon.ca/PlayStation-DualSense-Wireless-Controller-Midnight/dp/B0951JZDWT",
                Product = _Product
            };

            _DataBaseContext.ProductDefinitions.Add(_ProductDefinition);

            // Add User
            var _User = new RadiantServerUserProductsHistoryModel
            {
                Email = RadiantCommonUnitTestsConstants.EMAIL,
                Password = "MySuperPassword",
                Type = RadiantUserModel.UserType.Admin,
                UserName = "MySuperUser"
            };

            _DataBaseContext.Users.Add(_User);

            // Add subscription to recently added product
            var _Subscription = new RadiantServerProductSubscriptionModel
            {
                Product = _Product,
                User = _User,
                MaximalPriceForNotification = 90,
                SendEmailOnNotification = true
            };

            _User.ProductSubscriptions.Add(_Subscription);

            _DataBaseContext.SaveChanges();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void BasicConnectToDataBaseTest()
        {
            using var _DataBaseContext = new ServerProductsDbContext();
        }

        [Fact]
        public void BasicCRUDOperations()
        {
            using var _DataBaseContext = new ServerProductsDbContext();

            RemoveAllProducts();

            // Validate that the db is empty
            Assert.Equal(0, _DataBaseContext.Products.Count());

            DateTime _Now = DateTime.Now;
            string _Name1 = "ProductName-041BEB2A-A362-4139-9134-798B8B8AB770";
            string _Title1 = "UnitTestTitle-C3292360-2AD5-4399-93B9-4B68139F9E91";
            double _Price1 = 89.98;

            // Add a new product
            var _Product1 = new RadiantServerProductModel
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
            Assert.Equal(0, _DataBaseContext.Products.First().ProductDefinitionCollection.Count);

            // Add a second product with history
            string _Name2 = "ProductName-2520BC65-4DE7-45D5-83B6-3243B51064F0";
            string _Title2 = "UnitTestTitle-C2F867C4-9CAA-49F4-90B1-96752BCFCDAE";
            double _Price2 = 13.14;

            var _Product2 = new RadiantServerProductModel
            {
                InsertDateTime = _Now,
                Name = _Name2,
                FetchProductHistoryEnabled = true
            };

            var _ProductDefinition2 = new RadiantServerProductDefinitionModel
            {
                InsertDateTime = _Now,
                FetchProductHistoryEnabled = true,
                FetchProductHistoryEveryX = new TimeSpan(0, 0, 10),
                FetchProductHistoryTimeSpanNoiseInPerc = 0,
                Product = _Product2
            };

            _ProductDefinition2.ProductHistoryCollection.Add(new RadiantServerProductHistoryModel
            {
                InsertDateTime = _Now,
                Price = _Price2,
                Title = _Title2
            });

            _DataBaseContext.ProductDefinitions.Add(_ProductDefinition2);

            _DataBaseContext.SaveChanges();

            //_DataBaseContext.Dispose();

            // Switching to a brand new context to test navigation properties / FK / Lazy loading
            //using var _DataBaseContext2 = new ProductsDbContext();

            RadiantServerProductModel _Product2FromStorage = _DataBaseContext.Products.Single(s => s.Name == _Name2);

            // Load the product history of product 2
            //_DataBaseContext.Entry(_Product2FromStorage).Collection(c => c.ProductDefinitionCollection).Load();

            Assert.NotEqual(null, _Product2FromStorage);
            Assert.Equal(_Now, _Product2FromStorage.InsertDateTime);
            Assert.Equal(1, _Product2FromStorage.ProductDefinitionCollection.Count);
            Assert.Equal(_Price2, _Product2FromStorage.ProductDefinitionCollection.Single().ProductHistoryCollection.Single().Price);
            Assert.Equal(_Title2, _Product2FromStorage.ProductDefinitionCollection.Single().ProductHistoryCollection.Single().Title);

            // Update second product price
            var _LastProductHistory = _DataBaseContext.Products.Single(w => w.Name == _Name2).ProductDefinitionCollection.Single().ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).First();
            _LastProductHistory.Price = 12.11;
            _DataBaseContext.SaveChanges();

            var _LastProductHistoryAfterModif = _DataBaseContext.Products.Single(w => w.Name == _Name2).ProductDefinitionCollection.Single().ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime).First();

            Assert.Equal(12.11, _LastProductHistoryAfterModif.Price);

            // Test product history
            string _Name3 = "ProductName-6ECFC80B-0C4D-46C1-8ACF-EFDD043AB4F3";
            string _Title3 = "UnitTestTitle-3B94FD66-CB5B-4A70-97DB-FE527593F507";
            double _Price3 = 8.22;

            var _Product3 = new RadiantServerProductModel
            {
                Name = _Name3,
                ProductDefinitionCollection = new List<RadiantServerProductDefinitionModel>
                {
                    new()
                    {
                        InsertDateTime = _Now,
                        FetchProductHistoryEnabled = true,
                        FetchProductHistoryEveryX = new TimeSpan(0, 0, 10),
                        FetchProductHistoryTimeSpanNoiseInPerc = 0
                    }
                }
            };
            _DataBaseContext.Products.Add(_Product3);
            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Products.Single(w => w.Name == _Name3).ProductDefinitionCollection.Count);
            Assert.Equal(0, _DataBaseContext.Products.Single(w => w.Name == _Name3).ProductDefinitionCollection.Single().ProductHistoryCollection.Count);

            using (var _DataBaseContextTemp = new ServerProductsDbContext())
            {
                _Product3.ProductDefinitionCollection.Single().ProductHistoryCollection.Add(new RadiantServerProductHistoryModel
                {
                    InsertDateTime = _Now,
                    Price = _Price3,
                    Title = _Title3
                });
                _DataBaseContextTemp.SaveChanges();
            }

            Assert.Equal(1, _DataBaseContext.Products.Single(w => w.Name == _Name3).ProductDefinitionCollection.Single().ProductHistoryCollection.Count);

            // Clean up
            _DataBaseContext.Products.Remove(_Product1);
            _DataBaseContext.Products.Remove(_Product2FromStorage);
            _DataBaseContext.Products.Remove(_Product3);
            _DataBaseContext.SaveChanges();

            // Check that clean up worked
            Assert.Equal(0, _DataBaseContext.Products.Count());
        }

        [Fact]
        public void SubscriptionBasicDbContextTest()
        {
            using var _DataBaseContext = new ServerProductsDbContext();
            _DataBaseContext.SaveChanges();
        }

        [Fact]
        public void SubscriptionBasicTest()
        {
            using var _DataBaseContext = new ServerProductsDbContext();

            RemoveAllProducts();
            RemoveAllUsers();

            // Add basic product
            var _Product = new RadiantServerProductModel
            {
                Name = "A8FBFC17-BF62-458F-A018-AB87A0347EC4",
                ProductDefinitionCollection = new List<RadiantServerProductDefinitionModel>
                {
                    new()
                    {
                        FetchProductHistoryEnabled = true,
                        FetchProductHistoryEveryX = new TimeSpan(0, 10, 0),
                        FetchProductHistoryTimeSpanNoiseInPerc = 5,
                        Url = "AGoodUrl.com"
                    }
                }
            };

            _DataBaseContext.Products.Add(_Product);
            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Products.Count());

            // Add new product history to test remove cascade while we're at it
            var _ProductHistory = new RadiantServerProductHistoryModel
            {
                Price = 13.13,
                ProductDefinitionId = _Product.ProductDefinitionCollection.Single().ProductDefinitionId// Let's just refer to product id instead of directly product as we do later on in this unitTest (for subscription)
            };

            _DataBaseContext.ProductsHistory.Add(_ProductHistory);
            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Products.First().ProductDefinitionCollection.Single().ProductHistoryCollection.Count);

            // Add a new basic user
            var _User = new RadiantServerUserProductsHistoryModel
            {
                Email = RadiantCommonUnitTestsConstants.EMAIL,
                Password = "61B859D2-44CA-4834-8BEB-E3FE1707258A",
                Type = RadiantUserModel.UserType.User,
                UserName = "F8183CF2-67B3-4B12-8553-EEB41322F04C"
            };

            _DataBaseContext.Users.Add(_User);
            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Users.Count());

            // Add a new subscription to basic product
            var _Subscription = new RadiantServerProductSubscriptionModel
            {
                Product = _DataBaseContext.Products.First(),
                User = _DataBaseContext.Users.First(),
                MaximalPriceForNotification = 57.35
            };

            _User.ProductSubscriptions.Add(_Subscription);
            _DataBaseContext.SaveChanges();

            _DataBaseContext.Users.Update(_User);
            _DataBaseContext.SaveChanges();

            Assert.Equal(1, _DataBaseContext.Subscriptions.Count());

            // Clean
            RemoveAllProducts();

            Assert.Equal(0, _DataBaseContext.Products.Count());
            Assert.Equal(0, _DataBaseContext.Subscriptions.Count());// Should have cascaded from product deletion
            Assert.Equal(0, _DataBaseContext.ProductsHistory.Count());// Should have cascaded from product deletion

            RemoveAllUsers();
            Assert.Equal(0, _DataBaseContext.Users.Count());
        }
    }
}
