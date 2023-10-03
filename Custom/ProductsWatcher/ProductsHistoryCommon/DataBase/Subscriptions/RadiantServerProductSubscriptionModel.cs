using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions
{
    public class RadiantServerProductSubscriptionModel : RadiantProductSubscriptionModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("UserId")]
        public virtual RadiantServerUserProductsHistoryModel User { get; set; }

        public long UserId { get; set; }

        [ForeignKey("ProductId")]
        public virtual RadiantServerProductModel Product { get; set; }

        public long ProductId { get; set; }
    }
}
