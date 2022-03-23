using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace RadiantReader.DataBase
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
    }
}
