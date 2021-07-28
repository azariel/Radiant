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
