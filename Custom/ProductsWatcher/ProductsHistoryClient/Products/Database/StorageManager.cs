using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace ProductsHistoryClient.Products.Database
{
    public static class StorageManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<RadiantProductModel> LoadProducts(bool aLoadProductsHistory)
        {
            using var _DataBaseContext = new ProductsDbContext();

            if(aLoadProductsHistory)
                _DataBaseContext.ProductsHistory.Load();

            return _DataBaseContext.Products.ToList();
        }
    }
}
