using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistory.DataBase
{
    [Table("Products")]
    public class Product : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string Name { get; set; }
        public virtual List<ProductHistory> ProductHistoryCollection { get; set; } = new();
    }
}
