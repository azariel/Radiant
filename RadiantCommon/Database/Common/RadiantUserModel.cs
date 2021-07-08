using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Common.Database.Common
{
    [Table("Users")]
    public class RadiantUserModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum UserType// TODO: replace by groups ?
        {
            User,
            Admin
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Password { get; set; }// TODO: encrypt

        [Required]
        public UserType Type { get; set; } = UserType.User;

        [Required]
        [Key]
        public long UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }
    }
}
