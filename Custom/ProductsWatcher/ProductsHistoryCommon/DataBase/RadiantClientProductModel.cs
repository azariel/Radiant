using System.Collections.Generic;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    public class RadiantClientProductModel : RadiantProductModel
    {
        // ********************************************************************
        //                            Properties
        // *******************************************************************
        public virtual List<RadiantClientProductSubscriptionModel> ProductSubscriptions { get; set; } = new();
        public virtual List<RadiantClientProductDefinitionModel> ProductDefinitionCollection { get; set; } = new();
    }
}

