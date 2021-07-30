using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    [Table("ProductHistory")]
    public class RadiantProductHistoryModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double? DiscountPercentage { get; set; }
        public double? DiscountPrice { get; set; }

        /// <summary>
        /// Raw price, without shipping cost, discount, etc.
        /// </summary>
        [Required]
        public double Price { get; set; }

        [Required]
        [Key]
        public long ProductHistoryId { get; set; }

        public double? ShippingCost { get; set; }

        [MaxLength(512)]
        public string Title { get; set; }
    }
}
