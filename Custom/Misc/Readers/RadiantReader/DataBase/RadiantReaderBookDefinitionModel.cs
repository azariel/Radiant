using Radiant.Common.Database.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public bool Blacklist { get; set; }// Book should be ghosted forever. User doesn't want to see it anymore

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

        [MaxLength(2048)]
        public string Pairings { get; set; }

        public string SecondaryCharacters { get; set; }

        /// <summary>
        /// Without downloading all chapters and calculating everything, what does the basic info (summary) is giving us about this
        /// </summary>
        public int SoftNbWords { get; set; }

        [MaxLength(8192)]
        public string Summary { get; set; }

        [MaxLength(512)]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
