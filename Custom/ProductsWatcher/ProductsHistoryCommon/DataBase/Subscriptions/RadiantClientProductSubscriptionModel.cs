using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase.Subscriptions
{
    public class RadiantClientProductSubscriptionModel : RadiantProductSubscriptionModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("UserId")]
        public virtual RadiantClientUserProductsHistoryModel User { get; set; }

        public long UserId { get; set; }

        [ForeignKey("ProductId")]
        public virtual RadiantClientProductModel Product { get; set; }

        public long ProductId { get; set; }
    }
}
