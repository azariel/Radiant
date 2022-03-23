using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace RadiantReader.DataBase
{
    [Table("BookDefinitions")]
    public class RadiantReaderBookDefinitionModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public long BookHostId { get; set; }

        public virtual List<RadiantReaderBookChapter> Chapters { get; set; } = new();

        [Required]
        [Key]
        public long BookDefinitionId { get; set; }

        [ForeignKey("BookHostId")]
        public virtual RadiantReaderHostModel Host { get; set; }

        public DateTime? LastFetch { get; set; }

        public string MainCharacters { get; set; }

        public string Rating { get; set; }

        [Required]
        public bool RequireUpdate { get; set; }// Should update that fic when we can to fetch new chapters

        public string SecondaryCharacters { get; set; }

        /// <summary>
        /// Without downloading all chapters and calculating everything, what does the basic info (summary) is giving us about this
        /// </summary>
        public int SoftNbWords { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
