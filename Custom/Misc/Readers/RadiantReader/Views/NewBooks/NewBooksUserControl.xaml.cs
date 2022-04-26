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
    public partial class NewBooksUserControl : UserControl
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public NewBooksUserControl()
        {
            InitializeComponent();

            GenerateNewBooks(aNbElementsPerPage: fNbElementsPerPage);
            SetControlState();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private int fCurrentPage;
        private int fMaxPage;
        private int fTotalBooks;
        private int fNbElementsPerPage = 10;

        private void GenerateNewBooks(int aPageNumber = 0, int aNbElementsPerPage = 10, bool aResetScrollViewerToTop = true)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.Hosts.Load();
            _DataBaseContext.BookDefinitions.Load();

            fTotalBooks = _DataBaseContext.BookDefinitions.Count(w => !w.Blacklist);
            fMaxPage = fTotalBooks / aNbElementsPerPage;

            int _CurrentPage = Math.Min(aPageNumber, fMaxPage);

            NewBooksMainGrid.Children.Clear();
            List<RadiantReaderBookDefinitionModel> _BooksDefinitionCollection = _DataBaseContext.BookDefinitions.Where(w => !w.Blacklist).OrderByDescending(o => o.LastFetch).Skip(_CurrentPage * aNbElementsPerPage).Take(aNbElementsPerPage).ToList();
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

        private void SetOverallControlStateAction()
        {
            GenerateNewBooks(fCurrentPage, fNbElementsPerPage, false);
        }

        private void ImgNextPage_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            ++fCurrentPage;
            GenerateNewBooks(fCurrentPage, fNbElementsPerPage);
            SetControlState();
        }

        private void ImgPreviousPage_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            if (fCurrentPage > 1)
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
    }
}
