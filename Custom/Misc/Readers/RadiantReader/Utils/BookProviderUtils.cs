using System;
using Radiant.Custom.Readers.RadiantReader.Business;

namespace Radiant.Custom.Readers.RadiantReader.Utils
{
    public static class BookProviderUtils
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        /// <summary>
        /// Get enum provider from url
        /// </summary>
        /// <returns>Provider</returns>
        /// <exception cref="Exception">Provider not found</exception>
        public static BookProvider GetProviderFromUrl(string aUrl)
        {
            string _LowerInvariantUrl = aUrl.ToLowerInvariant();

            if (_LowerInvariantUrl.Contains("fanfiction.net"))
                return BookProvider.Fanfiction;

            if (_LowerInvariantUrl.Contains("archiveofourown.org"))
                return BookProvider.ArchiveOfOurOwn;

            throw new Exception($"No provider found for url [{aUrl}].");
        }
    }
}
