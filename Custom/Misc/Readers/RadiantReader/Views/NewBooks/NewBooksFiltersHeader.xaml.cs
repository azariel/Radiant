using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace RadiantReader.Views.NewBooks
{
    /// <summary>
    /// Interaction logic for NewBooksFiltersHeader.xaml
    /// </summary>
    public partial class NewBooksFiltersHeader : UserControl
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum NewBooksOrderBy
        {
            LastFetch,
            Words
        }

        public enum ShowLocalBookOption
        {
            ShowAll,
            ShowOnlyLocal,
            ShowOnlyNonLocal
        }

        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public NewBooksFiltersHeader()
        {
            InitializeComponent();

            foreach (NewBooksOrderBy _EnumValue in Enum.GetValues<NewBooksOrderBy>())
                cmbBoxOrderBy.Items.Add(_EnumValue);

            cmbBoxOrderBy.SelectedIndex = 0;

            foreach (ShowLocalBookOption _EnumValue in Enum.GetValues<ShowLocalBookOption>())
                cmbBoxShowLocalBooks.Items.Add(_EnumValue);

            cmbBoxShowLocalBooks.SelectedIndex = 0;
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************

        private void CmbBoxOrderBy_OnSelectionChanged(object aSender, SelectionChangedEventArgs aE)
        {
            this.OrderBy = (NewBooksOrderBy)cmbBoxOrderBy.SelectedItem;
            this.SetOverallControlStateAction?.Invoke();
        }

        private void CmbBoxPairings_OnSelectionChanged(object aSender, SelectionChangedEventArgs aE)
        {
            this.Pairings = cmbBoxPairings.SelectedItem as string;
            this.SetOverallControlStateAction?.Invoke();
        }

        private void CmbBoxRating_OnSelectionChanged(object aSender, SelectionChangedEventArgs aE)
        {
            this.Rating = cmbBoxRating.SelectedItem as string;

            // Refresh pairings filter options
            this.RefreshFilters?.Invoke(arg1: false, arg2: false, arg3: true);

            this.SetOverallControlStateAction?.Invoke();
        }

        private void CmbBoxShowLocalBooks_OnSelectionChanged(object aSender, SelectionChangedEventArgs aE)
        {
            this.LocalBookOption = (ShowLocalBookOption)cmbBoxShowLocalBooks.SelectedItem;
            this.SetOverallControlStateAction?.Invoke();
        }

        private void CmbBoxWorld_OnSelectionChanged(object aSender, SelectionChangedEventArgs aE)
        {
            this.SelectedWorld = cmbBoxWorld.SelectedItem as string;

            // Refresh ratings filter options
            this.RefreshFilters?.Invoke(arg1: false, arg2: true, arg3: true);

            this.SetOverallControlStateAction?.Invoke();
        }

        private void RegeneratePairingsItems()
        {
            cmbBoxPairings.Items.Clear();

            cmbBoxPairings.Items.Add("");// Add a clear item value
            foreach (string _AvailablePairing in this.AvailablePairings)
                cmbBoxPairings.Items.Add(_AvailablePairing);
        }

        private void RegenerateRatingsItems()
        {
            cmbBoxRating.Items.Clear();

            cmbBoxRating.Items.Add("");// Add a clear item value
            foreach (string _AvailableRating in this.AvailableRatings)
                cmbBoxRating.Items.Add(_AvailableRating);
        }

        private void RegenerateWorldsItems()
        {
            cmbBoxWorld.Items.Clear();

            cmbBoxWorld.Items.Add("");// Add a clear item value
            foreach (string _AvailableWorld in this.AvailableWorlds)
                cmbBoxWorld.Items.Add(_AvailableWorld);
        }

        private void TxtSummaryContainsWord_OnKeyUp(object aSender, KeyEventArgs aE)
        {
            if (aE.Key == Key.Enter)
                this.SetOverallControlStateAction?.Invoke();
        }

        private void TxtSummaryContainsWord_OnTextChanged(object aSender, TextChangedEventArgs aE)
        {
            this.SummaryContainsWord = txtSummaryContainsWord.Text;
        }

        public void Refresh(bool aBuildWorlds = true, bool aBuildRatings = true, bool aBuildPairings = true)
        {
            if (aBuildWorlds)
                RegenerateWorldsItems();

            if (aBuildRatings)
                RegenerateRatingsItems();

            if (aBuildPairings)
                RegeneratePairingsItems();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string[] AvailablePairings { get; set; }

        public string[] AvailableRatings { get; set; }

        public string[] AvailableWorlds { get; set; }

        public ShowLocalBookOption LocalBookOption { get; set; } = ShowLocalBookOption.ShowAll;

        public NewBooksOrderBy OrderBy { get; set; } = NewBooksOrderBy.LastFetch;

        public string Pairings { get; set; }

        public string Rating { get; set; }
        public Action<bool, bool, bool> RefreshFilters { get; set; }
        public string SelectedWorld { get; set; }
        public Action SetOverallControlStateAction { get; set; }
        public string SummaryContainsWord { get; set; }
    }
}
