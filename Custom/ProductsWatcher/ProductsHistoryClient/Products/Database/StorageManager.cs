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
        public static List<RadiantClientProductModel> LoadProducts(bool aLoadProductsHistory)
        {
            using var _DataBaseContext = new ClientProductsDbContext();
            _DataBaseContext.ProductDefinitions.Load();

            if(aLoadProductsHistory)
                _DataBaseContext.ProductsHistory.Load();

            return _DataBaseContext.Products.ToList();
        }
    }
}
