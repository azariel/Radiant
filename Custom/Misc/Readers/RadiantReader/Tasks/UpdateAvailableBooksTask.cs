using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Tasks.Triggers;
using RadiantClientWebScraper;
using RadiantReader.DataBase;
using RadiantReader.Managers;
using RadiantReader.Parsers;
using RadiantReader.Utils;

namespace RadiantReader.Tasks
{
    /// <summary>
    /// Fetch available books / fictions from the internet and store them in storage.
    /// Only fetch summary and information about the fic, not the actual entire fic.
    /// </summary>
    public class UpdateAvailableBooksTask : RadiantTask
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void FetchBooksOnLandingPage()
        {
            // Load all DataBase
            List<RadiantReaderHostModel> _HostBooks = StorageManager.LoadBooks(false);

            foreach (RadiantReaderHostModel _Host in _HostBooks)
            {
                string _DOM = AutomaticWebScraperClient.GetDOM(_Host.HostLandingPage);
                ParseBooksFromDOMLandingPage(_Host, _DOM);
            }
        }

        private void FetchChaptersFromBookDefinitionsRequiringUpdate()
        {
            // Load all DataBase
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();
            _DataBaseContext.BookContent.Load();

            RadiantReaderBookDefinitionModel[] _BookDefinitions = _DataBaseContext.BookDefinitions.Where(w => w.RequireUpdate).ToArray();
            foreach (var _BookDefinition in _BookDefinitions)
            {
                FanfictionFetcher.FetchNewChaptersFromBookDefinition(_BookDefinition);
                _DataBaseContext.SaveChanges();
            }
        }

        private void ParseBooksFromDOMLandingPage(RadiantReaderHostModel aHost, string aDOM)
        {
            // TODO: by domain. ex: parse fanfiction, parse archiveOfOurOwn, etc etc
            List<RadiantReaderBookDefinitionModel> _Books = FanfictionDOMUtils.ParseBooksFromFanfictionDOM(aDOM);

            StorageManager.AddOrRefreshBooksDefinition(aHost, _Books);
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: we should split this into 2 tasks... 1 to fetch new books and another one to update chapters

            // Get newly updated books from monitored main pages such as fanfiction.net, AoO, etc
            //FetchBooksOnLandingPage();

            // Fetch new chapters from book definitions requiring an update
            FetchChaptersFromBookDefinitionsRequiringUpdate();
        }
    }
}
