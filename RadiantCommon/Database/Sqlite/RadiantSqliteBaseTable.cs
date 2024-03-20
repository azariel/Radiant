using System;
using System.ComponentModel.DataAnnotations;

namespace Radiant.Common.Database.Sqlite
{
    /// <summary>
    /// All table in Sqlite must inherit from this class
    /// </summary>
    public class RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [Required]
        public DateTime InsertDateTime { get; set; } = DateTime.UtcNow;
    }
}
