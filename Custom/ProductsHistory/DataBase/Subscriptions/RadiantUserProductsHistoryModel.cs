﻿using System.Collections.Generic;
using Radiant.Common.Database.Common;

namespace Radiant.Custom.ProductsHistory.DataBase.Subscriptions
{
    public class RadiantUserProductsHistoryModel : RadiantUserModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public virtual List<RadiantProductSubscriptionModel> ProductSubscriptions { get; set; } = new();
    }
}
