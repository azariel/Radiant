using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Common.Tasks.Triggers;
using Radiant.Custom.Readers.RadiantReaderCommon.Business;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;
using Radiant.Custom.Readers.RadiantReaderCommon.Managers;
using Radiant.Custom.Readers.RadiantReaderCommon.Parsers;
using Radiant.Custom.Readers.RadiantReaderCommon.Utils;
using Radiant.WebScraper.RadiantClientWebScraper;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Tasks
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
                LoggingManager.LogToFile("3003f205-bc55-4e9c-ab3a-57679867a0c3", $"Fetching new books on landing page [{_Host.HostLandingPage}].");
                string _DOM = AutomaticWebScraperClient.GetDOMAsync(_Host.HostLandingPage).Result;
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
            foreach (RadiantReaderBookDefinitionModel _BookDefinition in _BookDefinitions)
            {
                int _NbChaptersBeforeFetch = _BookDefinition.Chapters.Count;
                bool _ChapterFetchErrorOrEnd = false;
                RadiantReaderBookChapter _NewChapter;
                do
                {
                    _NewChapter = BookFetcher.FetchNextChapterFromBookDefinition(_BookDefinition, out fShouldStop);

                    if (_NewChapter == null)
                    {
                        _ChapterFetchErrorOrEnd = true;
                        continue;
                    }

                    _BookDefinition.Chapters.Add(_NewChapter);
                    _DataBaseContext.SaveChanges();

                    if (fShouldStop)
                        break;

                    // Add a little sleep to avoid being tagged as a bot too easily
                    Thread.Sleep(new Random().Next(5765, 14457));// TODO: config
                } while (_NewChapter != null);

                if (_ChapterFetchErrorOrEnd || _BookDefinition.Chapters.Count > _NbChaptersBeforeFetch)
                {
                    _BookDefinition.RequireUpdate = false;
                    _DataBaseContext.SaveChanges();
                }

            }
        }

        private void ParseBooksFromDOMLandingPage(RadiantReaderHostModel aHost, string aDOM)
        {
            // TODO: by domain. ex: parse fanfiction, parse archiveOfOurOwn, etc etc
            BookProvider _BookProvider = BookProviderUtils.GetProviderFromUrl(aHost.HostLandingPage);
            List<RadiantReaderBookDefinitionModel> _Books = BookBuilderManager.ParseBooksFromDOM(_BookProvider, aDOM);

            StorageManager.AddOrRefreshBooksDefinition(aHost, _Books);
        }

        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            // TODO: we should split this into 2 tasks... 1 to fetch new books and another one to update chapters

            // Get newly updated books from monitored main pages such as fanfiction.net, AoO, etc
            if (HandleNewBooks)
                FetchBooksOnLandingPage();

            // Fetch new chapters from book definitions requiring an update
            if (HandleNewChapters)
                FetchChaptersFromBookDefinitionsRequiringUpdate();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool HandleNewBooks { get; set; }
        public bool HandleNewChapters { get; set; }
    }
}
