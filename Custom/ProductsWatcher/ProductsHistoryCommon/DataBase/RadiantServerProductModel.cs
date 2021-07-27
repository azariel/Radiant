using System.Collections.Generic;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
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

