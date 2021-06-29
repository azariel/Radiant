using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistory.DataBase
{
    [Table("ProductHistory")]
    public class ProductHistory : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        public double Price { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// FK to Product
        /// </summary>
        public long ProductId { get; set; }
    }
}
