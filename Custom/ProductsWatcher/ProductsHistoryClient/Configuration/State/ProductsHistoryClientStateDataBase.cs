using System;

namespace ProductsHistoryClient.Configuration.State
{
    public class ProductsHistoryClientStateDataBase
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// The last time we fetched the Database, the modifiedTime was this DateTime. This serves to know if we should update the
        /// local database using the remote one, among other things
        /// </summary>
        public DateTime LastFetchedDataBaseModifiedDateTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Last time we fetched the API to check state. This is mainly to avoid spamming the API
        /// </summary>
        public DateTime LastTimeEvaluated { get; set; } = DateTime.MinValue;
    }
}
