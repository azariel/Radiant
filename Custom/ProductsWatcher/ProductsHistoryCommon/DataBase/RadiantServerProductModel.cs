using System.Collections.Generic;
using Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    public class RadiantServerProductModel : RadiantProductModel
    {
        // ********************************************************************
        //                            Properties
        // *******************************************************************
        public virtual List<RadiantServerProductSubscriptionModel> ProductSubscriptions { get; set; } = new();
        public virtual List<RadiantServerProductDefinitionModel> ProductDefinitionCollection { get; set; } = new();
    }
}

