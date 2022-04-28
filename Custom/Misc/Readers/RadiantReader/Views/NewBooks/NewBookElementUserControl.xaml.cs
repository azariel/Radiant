using System;
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
    /// Interaction logic for NewBookElement.xaml
    /// </summary>
    public partial class NewBookElementUserControl : UserControl
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public NewBookElementUserControl()
        {
            InitializeComponent();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private RadiantReaderBookDefinitionModel fBookDefinition;

        private void ImgAddToDownload_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.BookDefinitions.Load();

            var _MatchingBooksDefinitionCollection = _DataBaseContext.BookDefinitions.Where(w => w.BookDefinitionId == fBookDefinition.BookDefinitionId);

            foreach (RadiantReaderBookDefinitionModel _MatchingBookDefinition in _MatchingBooksDefinitionCollection)
                _MatchingBookDefinition.RequireUpdate = true;

            _DataBaseContext.SaveChanges();

            // Reflect change locally as well
            fBookDefinition.RequireUpdate = true;

            SetControlState();
        }

        private void ImgBlacklist_OnMouseLeftButtonDown(object aSender, MouseButtonEventArgs aE)
        {
            using var _DataBaseContext = new RadiantReaderDbContext();
            _DataBaseContext.BookDefinitions.Load();

            var _MatchingBooksDefinitionCollection = _DataBaseContext.BookDefinitions.Where(w => w.BookDefinitionId == fBookDefinition.BookDefinitionId);

            foreach (RadiantReaderBookDefinitionModel _MatchingBookDefinition in _MatchingBooksDefinitionCollection)
                _MatchingBookDefinition.Blacklist = true;

            _DataBaseContext.SaveChanges();

            // Reflect change locally as well
            fBookDefinition.Blacklist = true;

            this.SetOverallControlStateAction?.Invoke();
        }

        private void SetControlState()
        {
            if (fBookDefinition == null)
                return;

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            var _ForeGroundColor = new SolidColorBrush(_Config.Settings.ForeGroundColor);

            var _ForeGroundColorBrighter = new SolidColorBrush(new Color
            {
                A = _Config.Settings.ForeGroundColor.A,
                R = (byte)(_Config.Settings.ForeGroundColor.R * 1.2f),
                G = (byte)(_Config.Settings.ForeGroundColor.G * 1.2f),
                B = (byte)(_Config.Settings.ForeGroundColor.B * 1.2f)
            });

            var _ForeGroundColorDarker = new SolidColorBrush(new Color
            {
                A = _Config.Settings.ForeGroundColor.A,
                R = (byte)(_Config.Settings.ForeGroundColor.R * 0.8f),
                G = (byte)(_Config.Settings.ForeGroundColor.G * 0.8f),
                B = (byte)(_Config.Settings.ForeGroundColor.B * 0.8f)
            });

            // Title
            lblTitle.Text = fBookDefinition.Title;
            lblTitle.Foreground = _ForeGroundColorBrighter;
            lblTitle.FontSize = _Config.Settings.FontSize + 2;

            // Separator
            SeparatorControl.Background = _ForeGroundColorBrighter;

            // Summary
            txtBlockSummary.Text = fBookDefinition.Summary;
            txtBlockSummary.Foreground = _ForeGroundColor;
            txtBlockSummary.FontSize = _Config.Settings.FontSize;

            // Infos
            // - Words
            lblWords.Content = $"{fBookDefinition.SoftNbWords:N0} words";
            lblWords.Foreground = _ForeGroundColorDarker;
            lblWords.FontSize = _Config.Settings.FontSize;

            // - Rating
            lblRating.Content = $"Rating: {fBookDefinition.Rating}";
            lblRating.Foreground = _ForeGroundColorDarker;
            lblRating.FontSize = _Config.Settings.FontSize;

            // - Main Characters
            lblMainCharacters.Content = fBookDefinition.MainCharacters;
            lblMainCharacters.Foreground = _ForeGroundColorDarker;
            lblMainCharacters.FontSize = _Config.Settings.FontSize;

            // - Pairings
            lblPairings.Content = fBookDefinition.Pairings;
            lblPairings.Foreground = _ForeGroundColorDarker;
            lblPairings.FontSize = _Config.Settings.FontSize;

            // Download button
            imgAddToDownload.Visibility = fBookDefinition.RequireUpdate ? Visibility.Collapsed : Visibility.Visible;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public RadiantReaderBookDefinitionModel BookDefinition
        {
            get => fBookDefinition;
            set
            {
                fBookDefinition = value;
                SetControlState();
            }
        }

        // ********************************************************************
        //                            Delegates
        // ********************************************************************
        public Action SetOverallControlStateAction { get; set; }
    }
}
