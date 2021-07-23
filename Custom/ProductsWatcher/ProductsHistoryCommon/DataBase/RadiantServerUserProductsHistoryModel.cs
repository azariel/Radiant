using System.Collections.Generic;
using Radiant.Common.Database.Common;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    public class RadiantServerUserProductsHistoryModel : RadiantUserModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public virtual List<RadiantServerProductSubscriptionModel> ProductSubscriptions { get; set; } = new();
    }
}
