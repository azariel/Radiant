﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReaderCommon.Configuration;
using Radiant.Custom.Readers.RadiantReaderCommon.Managers;
using Radiant.Custom.Readers.RadiantReader.Views;
using Point = System.Drawing.Point;

namespace Radiant.Custom.Readers.RadiantReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.PreviewMouseLeftButtonDown += MainGridOnMouseLeftButtonDown;
            this.KeyDown += MainGridOnKeyDown;
            MainGrid.Background = new SolidColorBrush(Color.FromArgb(a: 10, r: 1, g: 1, b: 1));

            // Wait until app is loaded before applying state
            this.Loaded += OnLoaded;
            this.Closed += OnClosed;
            this.Drop += OnDrop;

            GridContent.Children.Clear();
            GridContent.Children.Add(fReaderContentUserControl);

            HeaderControl.fSetReaderContentModule = SetReaderContentModule;
        }

        private void MainGridOnKeyDown(object aSender, KeyEventArgs aE)
        {
            switch (aE.Key)
            {
                case Key.Down:
                    fReaderContentUserControl.ContentScrollViewer.LineDown();
                    break;
                case Key.Up:
                    fReaderContentUserControl.ContentScrollViewer.LineUp();
                    break;
            }
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private readonly ReaderContentUserControl fReaderContentUserControl = new();

        private void LoadApplicationStateFromConfiguration()
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
            this.Width = _Config.State.Width;
            this.Height = _Config.State.Height;
            this.Left = _Config.State.Location.X;
            this.Top = _Config.State.Location.Y;

            SetControlState();
        }

        private void MainGridOnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            var _OriginalType = aE.OriginalSource.GetType();
            if (_OriginalType != typeof(TextBlock) && _OriginalType != typeof(Grid) && _OriginalType != typeof(ScrollViewer) && aE.OriginalSource is not Inline)
                return;

            if (_OriginalType == typeof(TextBlock) && ((TextBlock)aE.OriginalSource).Tag as string != "Draggable")
                return;

            DragMove();
            aE.Handled = true;
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
                _Config.State.Location = new Point((int)this.Left, (int)this.Top);

                foreach (IContentChild _ContentChild in GridContent.Children.OfType<IContentChild>())
                    _ContentChild.UpdateInMemoryConfig();

                RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
                RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
            }
            catch (Exception _Ex)
            {
                LoggingManager.LogToFile("34629a8a-9f5c-41ed-8111-b9c8e09cf611", "Couldn't save state configuration.", _Ex);
            }
        }

        private void OnDrop(object aSender, DragEventArgs aDragEventArgs)
        {
            // Load file if format is handled by app
            if (aDragEventArgs.Data.GetData(DataFormats.FileDrop) is not string[] _Files || !_Files.Any())
            {
                MessageBox.Show("Selected file is invalid.");
                return;
            }

            if (_Files.Length > 1)
            {
                MessageBox.Show("Please select only 1 file.");
                return;
            }

            List<Inline> _LineElements;
            try
            {
                if (!RadiantReaderFileLoader.LoadFile(_Files.Single(), out _LineElements))
                {
                    // File couldn't be load. LoadFile should handle a nice log, we'll just handle the user UI part
                    MessageBox.Show($"File [{_Files.Single()}] couldn't be loaded. See logs for more infos.");
                    return;
                }
            }
            catch (Exception _Exception)
            {
                MessageBox.Show($"File [{_Files.Single()}] couldn't be loaded. [{_Exception.Message}] [{_Exception.StackTrace}].");
                if (MessageBox.Show($"Would you like to load raw text instead ?", "Raw Load", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    fReaderContentUserControl.SetTextContent(File.ReadAllText(_Files.Single()));
                }

                return;
            }

            //fReaderContentUserControl.SetTextContent(_LineElements);
            
            // save config
            StateManager.SetCurrentBook(_Files.Single());

            fReaderContentUserControl.Reload();

            // set scrollbar to start
            fReaderContentUserControl.ContentScrollViewer.ScrollToTop();
        }

        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            LoadApplicationStateFromConfiguration();
        }

        private void SetControlState()
        {
            this.Topmost = RadiantReaderConfigurationManager.GetConfigFromMemory().Settings.TopMost;
        }

        private void SetReaderContentModule(UIElement aContent, HeaderOptions aHeaderOptions)
        {
            // Show only header options we want
            HeaderControl.RefreshOptions(aHeaderOptions);

            // Override content
            GridContent.Children.Clear();
            GridContent.Children.Add(aContent);

            SetControlState();
        }
    }
}
