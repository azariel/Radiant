using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.Readers.RadiantReaderCommon.DataBase
{
    [Table("BookHosts")]
    public class RadiantReaderHostModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public virtual List<RadiantReaderBookDefinitionModel> Definitions { get; set; } = new();

        [Required]
        [Key]
        public long BookHostId { get; set; }

        [Required]
        public string HostLandingPage { get; set; }// The page to fetch new fics

        [Required]
        [MaxLength(512)]
        public string World { get; set; }// Ex: Naruto, Harry Potter, etc
    }
}
