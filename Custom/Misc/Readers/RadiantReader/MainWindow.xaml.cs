using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Radiant.Common.Diagnostics;
using RadiantReader.Configuration;
using Color = System.Windows.Media.Color;

namespace RadiantReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.MouseLeftButtonDown += MainGridOnMouseLeftButtonDown;
            MainGrid.Background = new SolidColorBrush(Color.FromArgb(128, 50, 50, 50));

            // Wait until app is loaded before applying state
            this.Loaded += OnLoaded;
            this.Closed += OnClosed;
            this.Drop += OnDrop;
        }

        private void OnDrop(object aSender, DragEventArgs aDragEventArgs)
        {
            // Load file if format is handled by app
            string[]? _Files = aDragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];

            if (_Files == null || !_Files.Any())
            {
                MessageBox.Show("Selected file is invalid.");
                return;
            }

            if (_Files.Length > 1)
            {
                MessageBox.Show("Please select only 1 file.");
                return;
            }

            if (!RadiantReaderFileLoader.LoadFile(_Files.Single(), out List<Inline> _LineElements))
            {
                // File couldn't be load. LoadFile should handle a nice log, we'll just handle the user UI part
                MessageBox.Show($"File [{_Files.Single()}] couldn't be load. See logs for more infos.");
                return;
            }

            // Add line elements to textblock
            TextContentTextBlock.Inlines.Clear();
            TextContentTextBlock.Text = "";// Start by emptying it just to be sure we don't forget anything
            TextContentTextBlock.Inlines.AddRange(_LineElements);
        }

        private void OnClosed(object? aSender, EventArgs aE)
        {
            if (!this.IsLoaded)
                return;

            try
            {
                RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
                _Config.State.Width = this.Width;
                _Config.State.Height = this.Height;
                _Config.State.Location = new System.Drawing.Point((int)this.Left, (int)this.Top);

                RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
                RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("34629a8a-9f5c-41ed-8111-b9c8e09cf611", "Couldn't save state configuration.", _Ex);
            }
        }

        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            LoadApplicationStateFromConfiguration();
        }

        private void LoadApplicationStateFromConfiguration()
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
            this.Width = _Config.State.Width;
            this.Height = _Config.State.Height;
            this.Left = _Config.State.Location.X;
            this.Top = _Config.State.Location.Y;
        }

        private void MainGridOnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            DragMove();
        }
    }
}
