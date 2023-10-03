using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    [Table("ProductDefinitions")]
    public class RadiantProductDefinitionModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Enable this product definition to fetch new product history
        /// </summary>
        [Required]
        public bool FetchProductHistoryEnabled { get; set; }

        /// <summary>
        /// Fetch a new product history every X
        /// </summary>
        [Required]
        public TimeSpan FetchProductHistoryEveryX { get; set; }

        /// <summary>
        /// To avoid bot detection, we may want to fetch product every 24H +- 0-60 min for example
        /// If this is 2.5 for example, and FetchProductHistoryEveryX is 1 day, we will fetch a new product history every 0.975 to
        /// 1.025 day
        /// </summary>
        [Required]
        public float FetchProductHistoryTimeSpanNoiseInPerc { get; set; }

        /// <summary>
        /// Built from FetchProductHistoryEveryX using FetchProductHistoryTimeSpanNoiseInPerc if FetchProductHistoryEnabled is
        /// enabled
        /// </summary>
        public DateTime? NextFetchProductHistory { get; set; }

        [Required]
        [Key]
        public long ProductDefinitionId { get; set; }

        [Required]
        [MaxLength(2048)]
        public string Url { get; set; } = "";
    }
}
