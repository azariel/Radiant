using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions
{
    [Table("ProductSubscriptions")]
    public class RadiantProductSubscriptionModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        [Key]
        public long ProductSubscriptionId { get; set; }

        /// <summary>
        /// Will only notify user if product price is below or equal this price.
        /// If product is monitored when it's 150$, but we know that this product was 120$ last week, we can says that we don't want to be notified if product is 149$ next week for example.
        /// </summary>
        [Required]
        public double MaximalPriceForNotification { get; set; } = 999999;

        [Required]
        public bool SendEmailOnNotification { get; set; } = false;

        /// <summary>
        /// Will notify user if actual price is below or equal (BestPriceOfLastYear + BestPricePercentageForNotification %)
        /// If product goes from 100$ to 50$ next day, then goes back up to 54$, we may still consider this a deal.
        /// </summary>
        [Required]
        public float BestPricePercentageForNotification { get; set; } = 5;
    }
}
