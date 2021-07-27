using System;
using System.Linq;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace ProductsHistoryClient.View.Products
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
            IOrderedEnumerable<RadiantClientProductHistoryModel> _OrderedProductHistory = _HistoryCollection.OrderByDescending(o => o.InsertDateTime);

            // Current Price
            RadiantClientProductHistoryModel? _LatestProductHistory = _OrderedProductHistory.FirstOrDefault();
            this.CurrentPrice = _LatestProductHistory?.Price;

            // BestPrice1Y
            RadiantClientProductHistoryModel? _Best1YPriceProduct = _HistoryCollection.Where(w => w.InsertDateTime > _Now.AddYears(-1)).OrderBy(o => o.Price).FirstOrDefault();
            this.BestPrice1Y = _Best1YPriceProduct?.Price;
            this.Url = _Best1YPriceProduct?.ProductDefinition.Url;
            this.Name = _Best1YPriceProduct?.ProductDefinition.Product.Name;

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
        public string Name { get; set; }
        public RadiantClientProductModel ProductModel { get; }

        /// <summary>
        /// Url of current best price
        /// </summary>
        public string Url { get; set; }
    }
}
