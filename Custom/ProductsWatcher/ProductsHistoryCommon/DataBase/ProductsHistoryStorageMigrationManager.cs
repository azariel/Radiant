using System;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    /// <summary>
    /// Offer functionalities to migrate from Server database to Client one. 
    /// </summary>
    public static class ProductsHistoryStorageMigrationManager
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static bool CleanClientDatabase()
        {
            try
            {
                using ClientProductsDbContext _ClientProductsDbContext = new ClientProductsDbContext();
                _ClientProductsDbContext.Products.Load();
                _ClientProductsDbContext.ProductDefinitions.Load();
                _ClientProductsDbContext.ProductsHistory.Load();
                _ClientProductsDbContext.Users.Load();
                _ClientProductsDbContext.Subscriptions.Load();

                foreach (var _Product in _ClientProductsDbContext.Products)
                    _ClientProductsDbContext.Products.Remove(_Product);

                foreach (var _ProductDefinition in _ClientProductsDbContext.ProductDefinitions)
                    _ClientProductsDbContext.ProductDefinitions.Remove(_ProductDefinition);

                foreach (var _ProductHistory in _ClientProductsDbContext.ProductsHistory)
                    _ClientProductsDbContext.ProductsHistory.Remove(_ProductHistory);

                foreach (var _ProductHistory in _ClientProductsDbContext.Users)
                    _ClientProductsDbContext.Users.Remove(_ProductHistory);

                foreach (var _ProductHistory in _ClientProductsDbContext.Subscriptions)
                    _ClientProductsDbContext.Subscriptions.Remove(_ProductHistory);

                _ClientProductsDbContext.SaveChanges();
                return true;

            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("17163C79-F592-4465-840B-5FDC20A3FE88", $"Couldn't clean local Client Database.", _Exception);
                return false;
            }
        }

        // ********************************************************************
        //                            Public 
        // ********************************************************************
        /// <summary>
        /// Transfer non-sensible data from local Server database to local Client one.
        /// </summary>
        public static bool OverrideLocalClientDatabaseWithServerDatabase()
        {
            if (!CleanClientDatabase())
                return false;

            try
            {
                using ClientProductsDbContext _ClientProductsDbContext = new ClientProductsDbContext();
                _ClientProductsDbContext.Products.Load();
                _ClientProductsDbContext.ProductDefinitions.Load();
                _ClientProductsDbContext.ProductsHistory.Load();
                _ClientProductsDbContext.Users.Load();
                _ClientProductsDbContext.Subscriptions.Load();

                using ServerProductsDbContext _ServerProductsDbContext = new ServerProductsDbContext();
                _ServerProductsDbContext.Products.Load();
                _ServerProductsDbContext.ProductDefinitions.Load();
                _ServerProductsDbContext.ProductsHistory.Load();
                _ServerProductsDbContext.Users.Load();
                _ServerProductsDbContext.Subscriptions.Load();

                foreach (var _ServerProduct in _ServerProductsDbContext.Products)
                {
                    _ClientProductsDbContext.Products.Add(new RadiantClientProductModel
                    {
                        FetchProductHistoryEnabled = _ServerProduct.FetchProductHistoryEnabled,
                        InsertDateTime = _ServerProduct.InsertDateTime,
                        Name = _ServerProduct.Name,
                        ProductId = _ServerProduct.ProductId,
                    });
                }

                foreach (var _ServerProductDefinition in _ServerProductsDbContext.ProductDefinitions)
                {
                    _ClientProductsDbContext.ProductDefinitions.Add(new RadiantClientProductDefinitionModel
                    {
                        ProductId = _ServerProductDefinition.ProductId,
                        FetchProductHistoryEnabled = _ServerProductDefinition.FetchProductHistoryEnabled,
                        FetchProductHistoryEveryX = _ServerProductDefinition.FetchProductHistoryEveryX,
                        FetchProductHistoryTimeSpanNoiseInPerc = _ServerProductDefinition.FetchProductHistoryTimeSpanNoiseInPerc,
                        InsertDateTime = _ServerProductDefinition.InsertDateTime,
                        NextFetchProductHistory = _ServerProductDefinition.NextFetchProductHistory,
                        ProductDefinitionId = _ServerProductDefinition.ProductDefinitionId,
                        Url = _ServerProductDefinition.Url
                    });
                }

                foreach (var _ServerProductHistory in _ServerProductsDbContext.ProductsHistory)
                {
                    _ClientProductsDbContext.ProductsHistory.Add(new RadiantClientProductHistoryModel
                    {
                        InsertDateTime = _ServerProductHistory.InsertDateTime,
                        Price = _ServerProductHistory.Price,
                        ProductDefinitionId = _ServerProductHistory.ProductDefinitionId,
                        ProductHistoryId = _ServerProductHistory.ProductHistoryId,
                        Title = _ServerProductHistory.Title
                    });
                }

                foreach (var _ServerUser in _ServerProductsDbContext.Users)
                {
                    _ClientProductsDbContext.Users.Add(new RadiantClientUserProductsHistoryModel()
                    {
                        InsertDateTime = _ServerUser.InsertDateTime,
                        Type = _ServerUser.Type,
                        UserId = _ServerUser.UserId,
                        UserName = _ServerUser.UserName
                    });
                }

                foreach (var _ServerSubscription in _ServerProductsDbContext.Subscriptions)
                {
                    _ClientProductsDbContext.Subscriptions.Add(new RadiantClientProductSubscriptionModel
                    {
                        ProductId = _ServerSubscription.ProductId,
                        BestPricePercentageForNotification = _ServerSubscription.BestPricePercentageForNotification,
                        MaximalPriceForNotification = _ServerSubscription.MaximalPriceForNotification,
                        ProductSubscriptionId = _ServerSubscription.ProductSubscriptionId,
                        SendEmailOnNotification = _ServerSubscription.SendEmailOnNotification,
                        UserId = _ServerSubscription.UserId,
                    });
                }

                _ClientProductsDbContext.SaveChanges();

                return true;
            } catch (Exception _Exception)
            {
                LoggingManager.LogToFile("C657A796-2FF3-4B96-BB27-B714C1626FF2", $"Couldn't transfer local Server Database to local Client Database.", _Exception);
                return false;
            }
        }
    }
}
