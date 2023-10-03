using System.Collections.Generic;
using Radiant.Common.Database.Common;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    public class RadiantServerUserProductsHistoryModel : RadiantUserModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public virtual List<RadiantServerProductSubscriptionModel> ProductSubscriptions { get; set; } = new();
    }
}
