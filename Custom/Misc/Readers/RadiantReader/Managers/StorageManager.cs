using Microsoft.EntityFrameworkCore;
using RadiantReader.DataBase;
using System.Collections.Generic;
using System.Linq;

namespace RadiantReader.Managers
{
    public static class StorageManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static List<RadiantReaderHostModel> LoadBooks(bool aLoadBooksChapters)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();

            if(aLoadBooksChapters)
                _DataBaseContext.BookContent.Load();

            return _DataBaseContext.Hosts.ToList();
        }
    }
}
