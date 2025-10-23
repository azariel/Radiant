using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReaderCommon.Configuration;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;
using Radiant.Custom.Readers.RadiantReaderCommon.Utils;
using Radiant.Custom.Readers.RadiantReader.Views.NewBooks;
using Radiant.Custom.Readers.RadiantReader.Views.Settings;

namespace Radiant.Custom.Readers.RadiantReader.Views
{
    /// <summary>
    /// Interaction logic for HeaderUserControl.xaml
    /// </summary>
    public partial class HeaderUserControl : UserControl
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public HeaderUserControl()
        {
            InitializeComponent();

            HeaderMainGrid.Background = new SolidColorBrush(Color.FromArgb(a: 25, r: 25, g: 25, b: 25));
            SetControlState();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private bool fCloseButtonAvailable = true;
        private bool fSettingsButtonAvailable;
        private bool fShowDownloadAvailable;
        private bool fShowNewBooksAvailable;
        private bool fShowNextAvailable;
        private bool fShowPreviousAvailable;
        private bool fShowReaderAvailable;

        private void ImgCloseApp_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            // Close application. this will call onClosed event contrary to environment.exit
            Application.Current.Shutdown();
        }

        private void ImgDashboard_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            // Open new books control
            fSetReaderContentModule?.Invoke(new NewBooksUserControl(), new HeaderOptions
            {
                CloseButtonAvailable = true,
                ShowReaderAvailable = true,
                SettingsButtonAvailable = true
            });
        }

        private void ImgNextChapter_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_Config.State.SelectedBook == null)
                return;

            if (!string.IsNullOrEmpty(_Config.State.SelectedBook.AlternativeBookPathOnDisk))
            {
                NextAlternativeChapter();
                OpenReaderContentModule();
                return;
            }

            _Config = RadiantReaderConfigurationManager.ReloadConfig();
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.BookDefinitions.Load();
            _DataBaseContext.BookContent.Load();

            var _SelectedBook = _DataBaseContext.BookDefinitions.Single(s => s.BookDefinitionId == _Config.State.SelectedBook.BookDefinitionId);

            if (_SelectedBook.Chapters.Count > _Config.State.SelectedBook.BookChapterIndex + 1)
            {
                _Config.State.SelectedBook.BookChapterIndex++;
                _Config.State.VerticalScrollbarOffset = 0;
            }

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();

            OpenReaderContentModule();
            SetControlState();
        }

        private void ImgPreviousChapter_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_Config.State.SelectedBook == null)
                return;

            if (!string.IsNullOrEmpty(_Config.State.SelectedBook.AlternativeBookPathOnDisk))
            {
                PreviousAlternativeChapter();
                OpenReaderContentModule();
                SetControlState();
                return;
            }

            if (_Config.State.SelectedBook.BookChapterIndex > 0)
            {
                _Config.State.SelectedBook.BookChapterIndex--;
                _Config.State.VerticalScrollbarOffset = 0;
            }

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();

            OpenReaderContentModule();
            SetControlState();
        }

        private void ImgReader_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            OpenReaderContentModule();
        }

        private void ImgSettings_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            // Open settings control
            fSetReaderContentModule?.Invoke(new SettingsUserControl(), new HeaderOptions
            {
                CloseButtonAvailable = true,
                ShowReaderAvailable = true
            });
        }

        private void NextAlternativeChapter()
        {
            string _NewFilePath = AlternativeBookContentHelper.FindAlternativeChapterWithOffset(1);

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_NewFilePath == null)
            {
                LoggingManager.LogToFile("c92bb8ca-3506-49ea-bd57-cc473babf43f", $"Can't find file for next chapter of following file [{_Config.State.SelectedBook.AlternativeBookPathOnDisk}].");
                return;
            }

            _Config.State.SelectedBook.AlternativeBookPathOnDisk = _NewFilePath;
            _Config.State.VerticalScrollbarOffset = 0;
            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        private void OpenReaderContentModule()
        {
            // Open reader content control
            fSetReaderContentModule?.Invoke(new ReaderContentUserControl(), new HeaderOptions
            {
                CloseButtonAvailable = true,
                SettingsButtonAvailable = true,
                ShowDownloadAvailable = true,
                ShowNewBooksAvailable = true,
                ShowNextAvailable = true,
                ShowPreviousAvailable = true
            });

            SetControlState();
        }

        private void PreviousAlternativeChapter()
        {
            string _NewFilePath = AlternativeBookContentHelper.FindAlternativeChapterWithOffset(-1);

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_NewFilePath == null)
            {
                LoggingManager.LogToFile("e9acc386-8e27-4dfd-a50e-54b93fed54fb", $"Can't find file for previous chapter of following file [{_Config.State.SelectedBook.AlternativeBookPathOnDisk}].");
                return;
            }

            _Config.State.SelectedBook.AlternativeBookPathOnDisk = _NewFilePath;
            _Config.State.VerticalScrollbarOffset = 0;
            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        private void SetChapterInfosUIRepresentation()
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.BookDefinitions.Load();
            _DataBaseContext.BookContent.Load();

            // Set chapter infos
            if (_Config.State.SelectedBook == null)
                return;

            var _SelectedBook = _DataBaseContext.BookDefinitions.SingleOrDefault(s => s.BookDefinitionId == _Config.State.SelectedBook.BookDefinitionId);

            if (_SelectedBook == null)
            {
                // Don't throw, if the user may had loaded a book from disk instead of inStorage
                lblChapterIndex.Content = $"chp.{AlternativeBookContentHelper.GetAlternativeChapterIndex(0)}";
                return;
            }

            lblChapterIndex.Content = $"chp.{_Config.State.SelectedBook.BookChapterIndex + 1}";
            double _CurrentChaptersWords = GetCurrentWordsRead(_SelectedBook);
            double _TotalWords = _SelectedBook.Chapters.Aggregate(seed: 0, (count, val) => count + val.ChapterWordsCount);
            lblWordsCount.Content = $"{_CurrentChaptersWords:N0}/{_TotalWords:N0}";
            lblWordsPerc.Content = $"{Math.Round(_CurrentChaptersWords * 100 / _TotalWords, digits: 0)}%";
        }

        private long GetCurrentWordsRead(RadiantReaderBookDefinitionModel aRadiantBook)
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
            long _NbWordsRead = aRadiantBook.Chapters.Where(w => w.ChapterNumber < _Config.State.SelectedBook.BookChapterIndex + 1).Aggregate(seed: 0, (count, val) => count + val.ChapterWordsCount);

            // TODO Add an estimation of words read from scrollviewer vertical offset
            return _NbWordsRead;
        }

        private void SetControlState()
        {
            SetImageButtonVisibility(imgSettings, fSettingsButtonAvailable);
            SetImageButtonVisibility(imgCloseApp, fCloseButtonAvailable);

            // TODO: that option will set all local books to be refreshed instead of having to manually set them all one by one.
            //SetImageButtonVisibility(imgQueueFetch, fShowDownloadAvailable);
            SetImageButtonVisibility(imgDashboard, fShowNewBooksAvailable);
            SetImageButtonVisibility(imgNextChapter, fShowNextAvailable);
            SetImageButtonVisibility(imgPreviousChapter, fShowPreviousAvailable);
            SetImageButtonVisibility(imgReader, fShowReaderAvailable);

            SetChapterInfosUIRepresentation();
        }

        private void SetImageButtonVisibility(Image aImage, bool aVisible)
        {
            if (aImage == null)
                return;

            aImage.IsEnabled = aVisible;
            aImage.Visibility = aVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        // ********************************************************************
        //                            Delegates
        // ********************************************************************
        public Action<UIElement, HeaderOptions> fSetReaderContentModule;

        public void RefreshOptions(HeaderOptions aHeaderOptions)
        {
            this.CloseButtonAvailable = aHeaderOptions.CloseButtonAvailable;
            this.SettingsButtonAvailable = aHeaderOptions.SettingsButtonAvailable;
            this.ShowDownloadAvailable = aHeaderOptions.ShowDownloadAvailable;
            this.ShowNewBooksAvailable = aHeaderOptions.ShowNewBooksAvailable;
            this.ShowNextAvailable = aHeaderOptions.ShowNextAvailable;
            this.ShowPreviousAvailable = aHeaderOptions.ShowPreviousAvailable;
            this.ShowReaderAvailable = aHeaderOptions.ShowReaderAvailable;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public bool CloseButtonAvailable
        {
            get => fCloseButtonAvailable;
            set
            {
                fCloseButtonAvailable = value;
                SetControlState();
            }
        }

        public bool SettingsButtonAvailable
        {
            get => fSettingsButtonAvailable;
            set
            {
                fSettingsButtonAvailable = value;
                SetControlState();
            }
        }

        public bool ShowDownloadAvailable
        {
            get => fShowDownloadAvailable;
            set
            {
                fShowDownloadAvailable = value;
                SetControlState();
            }
        }

        public bool ShowNewBooksAvailable
        {
            get => fShowNewBooksAvailable;
            set
            {
                fShowNewBooksAvailable = value;
                SetControlState();
            }
        }

        public bool ShowNextAvailable
        {
            get => fShowNextAvailable;
            set
            {
                fShowNextAvailable = value;
                SetControlState();
            }
        }

        public bool ShowPreviousAvailable
        {
            get => fShowPreviousAvailable;
            set
            {
                fShowPreviousAvailable = value;
                SetControlState();
            }
        }

        public bool ShowReaderAvailable
        {
            get => fShowReaderAvailable;
            set
            {
                fShowReaderAvailable = value;
                SetControlState();
            }
        }
    }
}
