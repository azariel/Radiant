using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RadiantReader.Views
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

        private void SetImageButtonVisibility(Image aImage, bool aVisible)
        {
            if (aImage == null)
                return;

            aImage.IsEnabled = aVisible;
            aImage.Visibility = aVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ImgCloseApp_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            // Close application. this will call onClosed event contrary to environment.exit
            Application.Current.Shutdown();
        }

        private void ImgDashboard_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            // Open new books control
            fSetReaderContentModule?.Invoke(new NewBooksUserControl());
        }

        private void SetControlState()
        {
            SetImageButtonVisibility(imgSettings, fSettingsButtonAvailable);
            SetImageButtonVisibility(imgCloseApp, fCloseButtonAvailable);
            SetImageButtonVisibility(imgQueueFetch, fShowDownloadAvailable);
            SetImageButtonVisibility(imgDashboard, fShowNewBooksAvailable);
            SetImageButtonVisibility(imgNextChapter, fShowNextAvailable);
            SetImageButtonVisibility(imgPreviousChapter, fShowPreviousAvailable);
            SetImageButtonVisibility(imgReader, fShowReaderAvailable);
        }

        // ********************************************************************
        //                            Delegates
        // ********************************************************************
        public Action<UIElement> fSetReaderContentModule;

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
