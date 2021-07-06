using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsHistory.Configuration;
using Radiant.Custom.ProductsHistory.DataBase;
using Radiant.Custom.ProductsHistory.Scraper;
using Radiant.WebScraper;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.Custom.ProductsHistory.Tasks
{
    public class ProductsMonitorTask : RadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void EvaluateProduct(Product aProduct)
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

        private void UpdateNextFetchDateTime(Product aProduct, DateTime aNow)
        {
            if (aProduct == null || !aProduct.FetchProductHistoryEnabled)
                return;

            Random _Random = new Random();
            int _Modifier = _Random.Next(-100, 100);
            float _NoisePercValue = (aProduct.FetchProductHistoryTimeSpanNoiseInPerc * ((float)_Modifier / 100));
            TimeSpan _NoiseValue = (aProduct.FetchProductHistoryEveryX / 100) * ((_NoisePercValue / 100) + 100);
            DateTime _NextFetchDateTime = aNow + _NoiseValue;

            aProduct.NextFetchProductHistory = _NextFetchDateTime;
        }

        private void FetchNewProductHistory(Product aProduct, DateTime aNow)
        {
            ManualScraper _ManualScraper = new();
            ProductTargetScraper _ProductScraper = new(BaseTargetScraper.TargetScraperCoreOptions.Screenshot);
            ProductsHistoryConfiguration _Config = ProductsHistoryConfigurationManager.ReloadConfig();
            List<ManualScraperItemParser> _ManualScrapers = _Config.ManualScraperSequenceItems.Select(s => (ManualScraperItemParser)s).ToList();
            
            _ManualScraper.GetTargetValueFromUrl(SupportedBrowser.Firefox, aProduct.Url, _ProductScraper, _ManualScrapers, _Config.DOMParserItems.Select(s => (DOMParserItem)s).ToList());

            ProductHistory? _MostRecentProductHistory = aProduct.ProductHistoryCollection.FirstOrDefault(f => f.InsertDateTime == aProduct.ProductHistoryCollection.Min(m => m.InsertDateTime));

            ProductHistory _ProductHistory = CreateProductHistoryFromProductTargetScraper(_ProductScraper, aProduct, _MostRecentProductHistory?.Title);

            if (_ProductHistory == null)
            {
                LoggingManager.LogToFile("988B416D-BE97-42A3-BA2F-438FFBFEDAF4", $"Couldn't fetch new product history of product [{aProduct.Name}] or Url [{aProduct.Url}].");
                // TODO: fail sequence

                // To avoid a query loop
                UpdateNextFetchDateTime(aProduct, aNow);

                return;
            }

            aProduct.ProductHistoryCollection.Add(_ProductHistory);

            // Update product in db to set the next trigger time
            UpdateNextFetchDateTime(aProduct, aNow);
        }

        private ProductHistory CreateProductHistoryFromProductTargetScraper(ProductTargetScraper aProductScraper, Product aProduct, string aProductTitle)
        {
            if (aProductScraper?.Information?.Price == null)
                return null;

            // Check that the Title match. Some website (cough couch Amazon) keep the SAME url, but change the product..
            if (!string.IsNullOrWhiteSpace(aProductTitle) && !string.Equals(aProductScraper.Information.Title, aProductTitle, StringComparison.CurrentCultureIgnoreCase))
            {
                LoggingManager.LogToFile("E24BD4BD-0AE7-41B5-BF66-1D703B75905A", $"Product Id [{aProduct.ProductId}] was fetched but Title was mismatching. Title expected: [{aProductTitle}] but found [{aProductScraper.Information.Title.Trim()}].");

                return null;
            }

            return new ProductHistory
            {
                InsertDateTime = DateTime.Now,
                Price = aProductScraper.Information.Price.Value,
                Title = aProductScraper.Information.Title.Trim(),
                ProductId = aProduct.ProductId
            };
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: to implement fetch the next product to update and update it if the time is right
            Thread.Sleep(1000);
            using var _DataBaseContext = new ProductsDbContext();

            Product? _ProductToUpdate = _DataBaseContext.Products.OrderByDescending(o => o.ProductHistoryCollection.Min(m => m.InsertDateTime)).FirstOrDefault();

            if (_ProductToUpdate == null)
                return;

            foreach (Product _Product in _DataBaseContext.Products.Where(w => w.FetchProductHistoryEnabled).ToArray())
            {
                // Load specific product histories
                _DataBaseContext.Entry(_Product).Collection(c => c.ProductHistoryCollection).Load();
                EvaluateProduct(_Product);

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
