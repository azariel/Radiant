using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Managers
{
    public static class StorageManager
    {

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static void AddOrRefreshBooksDefinition(RadiantReaderHostModel aHost, List<RadiantReaderBookDefinitionModel> aBooks)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();

            foreach (RadiantReaderBookDefinitionModel _BookToAddOrUpdate in aBooks)
            {
                _BookToAddOrUpdate.BookHostId = aHost.BookHostId;

                if (!_DataBaseContext.BookDefinitions.Any())
                {
                    _BookToAddOrUpdate.LastFetch = DateTime.UtcNow;
                    _DataBaseContext.BookDefinitions.Add(_BookToAddOrUpdate);
                    _DataBaseContext.SaveChanges();
                    continue;
                }

                RadiantReaderBookDefinitionModel[] _MatchingBookDefinitions = _DataBaseContext.BookDefinitions.Where(w => w.Url == _BookToAddOrUpdate.Url).ToArray();

                if (_MatchingBookDefinitions.Length > 1)
                {
                    LoggingManager.LogToFile("85fd190a-6a21-430b-ab18-61c1f475e645", $"Found [{_MatchingBookDefinitions.Length}] book definitions matching url [{_BookToAddOrUpdate.Url}]. Updating all book definitions...");
                    foreach (var _BookDefinition in _MatchingBookDefinitions)
                        UpdateBookDefinitionMetaData(_BookDefinition, _BookToAddOrUpdate);

                    _DataBaseContext.SaveChanges();
                }

                if (_MatchingBookDefinitions.Length <= 0)
                {
                    // Add new
                    _BookToAddOrUpdate.LastFetch = DateTime.UtcNow;
                    _DataBaseContext.BookDefinitions.Add(_BookToAddOrUpdate);
                    _DataBaseContext.SaveChanges();// Save changes right now instead of only once at the end to avoid dropping batch due to a single error
                    continue;
                }

                UpdateBookDefinitionMetaData(_MatchingBookDefinitions.Single(), _BookToAddOrUpdate);
                _DataBaseContext.SaveChanges();
            }
        }

        private static void UpdateBookDefinitionMetaData(RadiantReaderBookDefinitionModel aBookDefinitionToUpdate, RadiantReaderBookDefinitionModel aUpdatedBookDefinition)
        {
            // TODO: ParseBooksFromFanfictionDOM should return it's own model and not RadiantReaderBookDefinitionModel
            aBookDefinitionToUpdate.MainCharacters = aUpdatedBookDefinition.MainCharacters;
            aBookDefinitionToUpdate.Rating = aUpdatedBookDefinition.Rating;
            //aBookDefinitionToUpdate.Blacklist = aUpdatedBookDefinition.Blacklist; Note: this isn't a metadata, it's a status
            aBookDefinitionToUpdate.SoftNbWords = aUpdatedBookDefinition.SoftNbWords;
            aBookDefinitionToUpdate.Summary = aUpdatedBookDefinition.Summary;
            aBookDefinitionToUpdate.Title = aUpdatedBookDefinition.Title;
            aBookDefinitionToUpdate.LastFetch = DateTime.UtcNow;
            aBookDefinitionToUpdate.Pairings = aUpdatedBookDefinition.Pairings;
            aBookDefinitionToUpdate.RequireUpdate = false;// Book was just updated
            aBookDefinitionToUpdate.SecondaryCharacters = aUpdatedBookDefinition.SecondaryCharacters;
        }

        public static List<RadiantReaderHostModel> LoadBooks(bool aLoadBooksChapters)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();

            if (aLoadBooksChapters)
                _DataBaseContext.BookContent.Load();

            return _DataBaseContext.Hosts.ToList();
        }
    }
}
