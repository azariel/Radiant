using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistory.DataBase
{
    [Table("ProductHistory")]
    public class RadiantProductHistoryModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        public double Price { get; set; }

        public virtual RadiantProductModel Product { get; set; }

        [Required]
        [Key]
        public long ProductHistoryId { get; set; }

        /// <summary>
        /// FK to Product
        /// </summary>
        [Required]
        public long ProductId { get; set; }

        [MaxLength(512)]
        public string Title { get; set; }
    }
}
