using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using Radiant.Custom.Readers.RadiantReader.Utils;
using Radiant.Custom.Readers.RadiantReaderCommon.Configuration;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;

namespace Radiant.Custom.Readers.RadiantReader.Views
{
    /// <summary>
    /// Interaction logic for ReaderContentUserControl.xaml
    /// </summary>
    public partial class ReaderContentUserControl : UserControl, IContentChild
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public ReaderContentUserControl()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                // Make sure keyboard focus goes to the scroll viewer when loaded
                ContentScrollViewer.Focusable = true;
                ContentScrollViewer.Focus();
                Keyboard.Focus(ContentScrollViewer);
            };

            TextContentTextBlock.Tag = "Draggable";
            Reload();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void LoadBookContentFromConfig()
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_Config.State.SelectedBook == null)
                return;

            // TODO: Async load and show progress UI representation

            if (string.IsNullOrWhiteSpace(_Config.State.SelectedBook.AlternativeBookPathOnDisk) || !System.IO.File.Exists(_Config.State.SelectedBook.AlternativeBookPathOnDisk))
            {
                SetChapterContentFromStorage(_Config);
            } else
            {
                SetChapterContentFromOnDiskFile(_Config);
            }
        }

        private void SetChapterContentFromStorage(RadiantReaderConfiguration aConfig)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            RadiantReaderBookDefinitionModel _SelectedBookDefinition = _DataBaseContext.BookDefinitions.Include(i => i.Chapters).SingleOrDefault(w => w.BookDefinitionId == aConfig.State.SelectedBook.BookDefinitionId);

            if (_SelectedBookDefinition == null)
                return;

            if (_SelectedBookDefinition.Chapters.Count <= aConfig.State.SelectedBook.BookChapterIndex)
            {
                // Don't crash, but log it
                LoggingManager.LogToFile("5b929e2e-be6f-4137-a016-8fae49f0d399", $"Chapter [{aConfig.State.SelectedBook.BookChapterIndex}] to open is greater than total chapters [{_SelectedBookDefinition.Chapters.Count}] available for book id [{aConfig.State.SelectedBook.BookDefinitionId}].");
                return;
            }

            string _ChapterContent = _SelectedBookDefinition.Chapters[aConfig.State.SelectedBook.BookChapterIndex].ChapterContent;


            List<Inline> _Inlines = StringConvertUtils.GetInlinesFromString(_ChapterContent);

            // Just add the title before the chapter content
            _Inlines.Insert(0, new LineBreak());
            _Inlines.Insert(0, new Underline(new Bold(new Run(_SelectedBookDefinition.Title))));

            SetTextContent(_Inlines);
        }

        private void SetChapterContentFromOnDiskFile(RadiantReaderConfiguration aConfig)
        {
            List<Inline> _LineElements;
            try
            {
                if (!RadiantReaderFileLoader.LoadFile(aConfig.State.SelectedBook.AlternativeBookPathOnDisk, out _LineElements))
                {
                    // File couldn't be load. LoadFile should handle a nice log, we'll just handle the user UI part
                    MessageBox.Show($"File [{aConfig.State.SelectedBook.AlternativeBookPathOnDisk}] couldn't be loaded. See logs for more infos.");
                    return;
                }
            } catch (Exception _Exception)
            {
                MessageBox.Show($"File [{aConfig.State.SelectedBook.AlternativeBookPathOnDisk}] couldn't be loaded. [{_Exception.Message}] [{_Exception.StackTrace}].");
                if (MessageBox.Show($"Would you like to load raw text instead ?", "Raw Load", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SetTextContent(System.IO.File.ReadAllText(aConfig.State.SelectedBook.AlternativeBookPathOnDisk));
                }

                return;
            }

            SetTextContent(_LineElements);
        }

        private void SetControlState()
        {
            var _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();

            TextContentTextBlock.Foreground = new SolidColorBrush(Color.FromArgb(_Config.Settings.ForeGroundColor.A, _Config.Settings.ForeGroundColor.R, _Config.Settings.ForeGroundColor.G, _Config.Settings.ForeGroundColor.B));
            TextContentTextBlock.FontSize = _Config.Settings.FontSize;

            ContentScrollViewer.ScrollToVerticalOffset(_Config.State.VerticalScrollbarOffset);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void UpdateInMemoryConfig()
        {
            RadiantReaderConfiguration _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();
            _Config.State.VerticalScrollbarOffset = ContentScrollViewer.VerticalOffset;
        }

        public void SetTextContent(string aRawText)
        {
            // Add line elements to textblock
            TextContentTextBlock.Inlines.Clear();
            TextContentTextBlock.Text = aRawText;
        }

        public void SetTextContent(List<Inline> aLineElements)
        {
            // Add line elements to textblock
            TextContentTextBlock.Inlines.Clear();
            TextContentTextBlock.Text = "";// Start by emptying it just to be sure we don't forget anything
            TextContentTextBlock.Inlines.AddRange(aLineElements);
        }

        public void Reload()
        {
            LoadBookContentFromConfig();
            SetControlState();
        }
    }
}
