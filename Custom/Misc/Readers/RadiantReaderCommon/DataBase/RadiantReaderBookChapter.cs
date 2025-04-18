﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.Readers.RadiantReaderCommon.DataBase
{
    [Table("BookChapters")]
    public class RadiantReaderBookChapter : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public long BookDefinitionId { get; set; }

        [Required]
        public string ChapterContent { get; set; }

        [Required]
        [Key]
        public long ChapterId { get; set; }

        [Required]
        public int ChapterNumber { get; set; }

        public string ChapterPartialUrl { get; set; }

        [Required]
        public int ChapterWordsCount { get; set; }

        [ForeignKey("BookDefinitionId")]
        public virtual RadiantReaderBookDefinitionModel Definition { get; set; }

        public string NextChapterPartialUrl { get; set; }
    }
}
