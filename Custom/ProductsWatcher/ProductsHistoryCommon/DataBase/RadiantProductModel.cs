using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    [Table("Products")]
    public class RadiantProductModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Enable this product to fetch new product history
        /// </summary>
        [Required]
        public bool FetchProductHistoryEnabled { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Key]
        public long ProductId { get; set; }
    }
}
