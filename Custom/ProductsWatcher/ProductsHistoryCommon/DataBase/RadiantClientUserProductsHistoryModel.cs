using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Common;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    [Table("ClientUsers")]
    public class RadiantClientUserProductsHistoryModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        public RadiantUserModel.UserType Type { get; set; } = RadiantUserModel.UserType.User;

        [Required]
        [Key]
        public long UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }
    }
}