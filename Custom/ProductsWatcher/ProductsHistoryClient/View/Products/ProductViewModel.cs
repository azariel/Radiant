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

        public ProductViewModel(RadiantClientProductDefinitionModel aRadiantProductModel)
        {
            this.ProductModel = aRadiantProductModel;

            DateTime _Now = DateTime.Now;
            IOrderedEnumerable<RadiantClientProductHistoryModel> _OrderedProductHistory = aRadiantProductModel.ProductHistoryCollection.OrderByDescending(o => o.InsertDateTime);

            RadiantClientProductHistoryModel? _LatestProductHistory = _OrderedProductHistory.FirstOrDefault();
            this.CurrentPrice = _LatestProductHistory?.Price;

            RadiantClientProductHistoryModel? _Best1YPriceProduct = aRadiantProductModel.ProductHistoryCollection.Where(w => w.InsertDateTime > _Now.AddYears(-1)).OrderBy(o => o.Price).FirstOrDefault();
            this.BestPrice1Y = _Best1YPriceProduct?.Price;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double? BestPrice1Y { get; }
        public double? CurrentPrice { get; }
        public RadiantClientProductDefinitionModel ProductModel { get; }
    }
}
