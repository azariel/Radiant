using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Radiant.Common.Database.Sqlite;
using Radiant.Custom.ProductsHistoryCommon.DataBase.Subscriptions;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    [Table("Products")]
    public class RadiantProductModel : RadiantSqliteBaseTable
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Enable this product to fetch new product history
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

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Built from FetchProductHistoryEveryX using FetchProductHistoryTimeSpanNoiseInPerc if FetchProductHistoryEnabled is
        /// enabled
        /// </summary>
        public DateTime? NextFetchProductHistory { get; set; }

        public virtual List<RadiantProductHistoryModel> ProductHistoryCollection { get; set; } = new();

        [Required]
        [Key]
        public long ProductId { get; set; }

        public virtual List<RadiantProductSubscriptionModel> ProductSubscriptions { get; set; } = new();

        [Required]
        [MaxLength(2048)]
        public string Url { get; set; } = "";
    }
}
