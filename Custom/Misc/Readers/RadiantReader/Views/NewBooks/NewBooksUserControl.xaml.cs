using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using RadiantReader.Configuration;
using RadiantReader.DataBase;

namespace RadiantReader.Views.NewBooks
{
    /// <summary>
    /// Interaction logic for NewBooksUserControl.xaml
    /// </summary>
    public partial class NewBooksUserControl : UserControl, IContentChild
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public NewBooksUserControl()
        {
            InitializeComponent();

            GenerateNewBooks(aNbElementsPerPage: fNbElementsPerPage);
            FilterControl.SetOverallControlStateAction = SetOverallControlStateAction;
            FilterControl.RefreshFilters = BuildFilters;

            BuildFilters();
            SetControlState();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private int fCurrentPage;
        private int fMaxPage;
        private readonly int fNbElementsPerPage = 10;
        private int fTotalBooks;

        private void BuildFilters(bool aBuildWorlds = true, bool aBuildRatings = true, bool aBuildPairings = true)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();

            // Worlds
            if (aBuildWorlds)
            {
                string[] _Worlds = _DataBaseContext.Hosts.Select(s => s.World).Distinct().OrderBy(o => o).ToArray();
                FilterControl.AvailableWorlds = _Worlds;
            }

            IQueryable<RadiantReaderBookDefinitionModel> _FilteredQuery = _DataBaseContext.BookDefinitions;

            if (!string.IsNullOrWhiteSpace(FilterControl.SelectedWorld))
                _FilteredQuery = _FilteredQuery.Where(w => w.Host.World == FilterControl.SelectedWorld);

            // Ratings
            if (aBuildRatings)
            {
                string[] _Ratings = _FilteredQuery.Select(s => s.Rating).Distinct().OrderBy(o => o).ToArray();
                FilterControl.AvailableRatings = _Ratings;
            }

            if (!string.IsNullOrWhiteSpace(FilterControl.Rating))
                _FilteredQuery = _FilteredQuery.Where(w => w.Rating == FilterControl.Rating);

            // Pairings
            if (aBuildPairings)
            {
                string[] _Pairings = _FilteredQuery.Select(s => s.Pairings).Distinct().OrderBy(o => o).ToArray();
                FilterControl.AvailablePairings = _Pairings;
            }

            FilterControl.Refresh(aBuildWorlds, aBuildRatings, aBuildPairings);
        }

        private void GenerateNewBooks(int aPageNumber = 0, int aNbElementsPerPage = 10, bool aResetScrollViewerToTop = true)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();

            NewBooksMainGrid.Children.Clear();
            IQueryable<RadiantReaderBookDefinitionModel> _FilteredQuery = _DataBaseContext.BookDefinitions.Where(w => !w.Blacklist);

            // Add filters
            // Summary contains word (case insensitive)
            if (!string.IsNullOrWhiteSpace(FilterControl.SummaryContainsWord))
                _FilteredQuery = _FilteredQuery.Where(w => EF.Functions.Like(w.Summary, $"%{FilterControl.SummaryContainsWord}%"));

            // Ratings
            if (!string.IsNullOrWhiteSpace(FilterControl.Rating))
                _FilteredQuery = _FilteredQuery.Where(w => w.Rating == FilterControl.Rating);

            // Pairings
            if (!string.IsNullOrWhiteSpace(FilterControl.Pairings))
                _FilteredQuery = _FilteredQuery.Where(w => EF.Functions.Like(w.Pairings, $"%{FilterControl.Pairings}%"));

            // World
            if (!string.IsNullOrWhiteSpace(FilterControl.SelectedWorld))
                _FilteredQuery = _FilteredQuery.Where(w => w.Host.World == FilterControl.SelectedWorld);

            // LocalBookOption
            switch (FilterControl.LocalBookOption)
            {
                case NewBooksFiltersHeader.ShowLocalBookOption.ShowAll:
                    // No query change
                    break;
                case NewBooksFiltersHeader.ShowLocalBookOption.ShowOnlyLocal:
                    _FilteredQuery = _FilteredQuery.Where(w => w.RequireUpdate || w.Chapters.Any());
                    break;
                case NewBooksFiltersHeader.ShowLocalBookOption.ShowOnlyNonLocal:
                    _FilteredQuery = _FilteredQuery.Where(w => !w.RequireUpdate && !w.Chapters.Any());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"LocalBookOption [{FilterControl.LocalBookOption}] is not handled.");
            }

            // OrderBy
            switch (FilterControl.OrderBy)
            {
                case NewBooksFiltersHeader.NewBooksOrderBy.LastFetch:
                    _FilteredQuery = _FilteredQuery.OrderByDescending(o => o.LastFetch);
                    break;
                case NewBooksFiltersHeader.NewBooksOrderBy.Words:
                    _FilteredQuery = _FilteredQuery.OrderByDescending(o => o.SoftNbWords);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Order [{FilterControl.OrderBy}] is not handled.");
            }

            fTotalBooks = _FilteredQuery.Count(w => !w.Blacklist);
            fMaxPage = fTotalBooks / aNbElementsPerPage;

            int _CurrentPage = Math.Min(aPageNumber, fMaxPage);

            List<RadiantReaderBookDefinitionModel> _BooksDefinitionCollection = _FilteredQuery
                //.OrderByDescending(o => o.LastFetch)
                .Skip(_CurrentPage * aNbElementsPerPage)
                .Take(aNbElementsPerPage)
                .ToList();

            foreach (RadiantReaderBookDefinitionModel _BookDefinition in _BooksDefinitionCollection)
            {
                var _NewBookElement = new NewBookElementUserControl { BookDefinition = _BookDefinition };
                _NewBookElement.SetOverallControlStateAction = SetOverallControlStateAction;
                NewBooksMainGrid.Children.Add(_NewBookElement);
            }

            if (aResetScrollViewerToTop)
                ResetScrollViewerToTop();

            SetControlState();
        }

        private void ImgNextPage_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            ++fCurrentPage;
            GenerateNewBooks(fCurrentPage, fNbElementsPerPage);
            SetControlState();
        }

        private void ImgPreviousPage_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            if (fCurrentPage > 0)
                --fCurrentPage;

            GenerateNewBooks(fCurrentPage, fNbElementsPerPage);
            SetControlState();
        }

        private void ResetScrollViewerToTop()
        {
            NewBooksScrollViewer.ScrollToTop();
        }

        private void SetControlState()
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            var _ForeGroundColor = new SolidColorBrush(_Config.Settings.ForeGroundColor);

            imgPreviousPage.Visibility = fCurrentPage < 1 ? Visibility.Collapsed : Visibility.Visible;
            imgNextPage.Visibility = fCurrentPage >= fMaxPage ? Visibility.Collapsed : Visibility.Visible;

            lblPagination.Content = $"Page {fCurrentPage + 1} / {fMaxPage + 1}";
            lblPagination.Foreground = _ForeGroundColor;
            lblPagination.FontSize = _Config.Settings.FontSize;

            lblTotalBooks.Content = $"{fTotalBooks:N0} Total";
            lblTotalBooks.Foreground = _ForeGroundColor;
            lblTotalBooks.FontSize = _Config.Settings.FontSize;
        }

        private void SetOverallControlStateAction()
        {
            GenerateNewBooks(fCurrentPage, fNbElementsPerPage, false);
        }

        public void UpdateInMemoryConfig() { }// Nothing to save
    }
}
