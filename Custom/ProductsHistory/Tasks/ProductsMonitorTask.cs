using System.Linq;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.ProductsHistory.DataBase;

namespace Radiant.Custom.ProductsHistory.Tasks
{
    public class ProductsMonitorTask : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: to implement fetch the next product to update and update it if the time is right



            using var _DataBaseContext = new ProductsDbContext();

            Product? _ProductToUpdate = _DataBaseContext.Products.OrderByDescending(o => o.ProductHistoryCollection.Min(m => m.InsertDateTime)).FirstOrDefault();

            if (_ProductToUpdate == null)
                return;


        }
    }
}
