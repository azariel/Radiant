using System;
using System.Linq;
using Radiant.Common.Utils;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryClient.View.Products
{
    public class ProductViewModel
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ProductViewModel() { }

        public ProductViewModel(RadiantClientProductModel aRadiantProductModel)
        {
            this.ProductModel = aRadiantProductModel;

            RadiantClientProductHistoryModel[] _HistoryCollection = aRadiantProductModel.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection).ToArray();

            DateTime _Now = DateTime.Now;
            DateTime? _MostRecentDate = _HistoryCollection.OrderByDescending(o => o.InsertDateTime).FirstOrDefault()?.InsertDateTime.Date;

            this.Name = aRadiantProductModel.Name;
            if (!_MostRecentDate.HasValue)
            {
                // TODO: set yellow

                return;
            }

            // Raw Price
            RadiantClientProductHistoryModel[] _LatestProductHistoryModels = _HistoryCollection.Where(w => w.InsertDateTime.Date == _MostRecentDate).ToArray();

            RadiantClientProductHistoryModel _LatestProductHistory = _LatestProductHistoryModels.OrderBy(o => o.Price).FirstOrDefault();
            this.RawPrice = _LatestProductHistory?.Price;

            this.ShippingCost = _LatestProductHistory?.ShippingCost ?? 0;
            this.DiscountPrice = _LatestProductHistory?.DiscountPrice ?? 0;
            this.DiscountPercentage = _LatestProductHistory?.DiscountPercentage ?? 0;

            // BestPrice1Y
            RadiantClientProductHistoryModel? _Best1YPriceProduct = _HistoryCollection.Where(w => w.InsertDateTime > _Now.AddYears(-1)).OrderBy(o => o.Price).FirstOrDefault();

            this.BestPrice1Y = -1;
            if (_Best1YPriceProduct != null)
                this.BestPrice1Y = _Best1YPriceProduct.Price - (_Best1YPriceProduct.DiscountPrice ?? 0) - (_Best1YPriceProduct.Price - (_Best1YPriceProduct.DiscountPrice ?? 0)) / 100 * (_Best1YPriceProduct.DiscountPercentage ?? 0) + (_Best1YPriceProduct.ShippingCost ?? 0);

            this.Url = _Best1YPriceProduct?.ProductDefinition.Url;
            this.Domain = RegexUtils.GetWebSiteDomain(_Best1YPriceProduct?.ProductDefinition.Url);
            this.Name = _Best1YPriceProduct?.ProductDefinition.Product.Name;

            // Current Price
            this.CurrentPrice = this.RawPrice - this.DiscountPrice;
            this.CurrentPrice = this.CurrentPrice - this.CurrentPrice / 100 * this.DiscountPercentage.Value + this.ShippingCost;

            // Difference from BestPrice1Y vs CurrentPrice
            if (this.CurrentPrice.HasValue && this.BestPrice1Y.HasValue)
            {
                double _DifferenceBestPrice1YVsCurrentPrice = Math.Round(this.CurrentPrice.Value * 100 / this.BestPrice1Y.Value - 100, 2);
                this.DifferenceBestPrice1YVsCurrentPrice = _DifferenceBestPrice1YVsCurrentPrice;
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double? BestPrice1Y { get; }
        public double? CurrentPrice { get; }
        public double? DifferenceBestPrice1YVsCurrentPrice { get; }
        public double? DiscountPercentage { get; }
        public double? DiscountPrice { get; }

        /// <summary>
        /// Url of current best price
        /// </summary>
        public string Domain { get; set; }

        public string Name { get; set; }
        public RadiantClientProductModel ProductModel { get; }

        /// <summary>
        /// Price without discount and shipping
        /// </summary>
        public double? RawPrice { get; }

        public double? ShippingCost { get; }

        public string Url { get; set; }
    }
}
