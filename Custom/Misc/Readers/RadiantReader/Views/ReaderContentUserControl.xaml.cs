using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Radiant.Common.Diagnostics;
using RadiantReader.Configuration;
using RadiantReader.DataBase;
using RadiantReader.Utils;

namespace RadiantReader.Views
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

            TextContentTextBlock.Tag = "Draggable";
            LoadBookContentFromConfig();
            SetControlState();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void LoadBookContentFromConfig()
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            if (_Config.State.SelectedBook == null)
                return;

            // TODO: Async load and show progress UI representation
            using var _DataBaseContext = new RadiantReaderDbContext();
            RadiantReaderBookDefinitionModel _SelectedBookDefinition = _DataBaseContext.BookDefinitions.Include(i => i.Chapters).Single(w => w.BookDefinitionId == _Config.State.SelectedBook.BookDefinitionId);

            if (_SelectedBookDefinition.Chapters.Count <= _Config.State.SelectedBook.BookChapterIndex)
            {
                // Don't crash, but log it
                LoggingManager.LogToFile("5b929e2e-be6f-4137-a016-8fae49f0d399", $"Chapter [{_Config.State.SelectedBook.BookChapterIndex}] to open is greater than total chapters [{_SelectedBookDefinition.Chapters.Count}] available for book id [{_Config.State.SelectedBook.BookDefinitionId}].");
                return;
            }

            string _ChapterContent = _SelectedBookDefinition.Chapters[_Config.State.SelectedBook.BookChapterIndex].ChapterContent;
            List<Inline> _Inlines = StringConvertUtils.GetInlinesFromString(_ChapterContent);

            // Just add the title before the chapter content
            _Inlines.Insert(0, new LineBreak());
            _Inlines.Insert(0, new Underline(new Bold(new Run(_SelectedBookDefinition.Title))));

            SetTextContent(_Inlines);
        }

        private void SetControlState()
        {
            var _Config = RadiantReaderConfigurationManager.GetConfigFromMemory();

            TextContentTextBlock.Foreground = new SolidColorBrush(_Config.Settings.ForeGroundColor);
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
    }
}
