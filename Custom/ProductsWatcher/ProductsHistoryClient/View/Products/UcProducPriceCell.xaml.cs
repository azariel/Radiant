using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProductsHistoryClient.View.Products
{
    /// <summary>
    /// Interaction logic for UcProductRow.xaml
    /// </summary>
    public partial class UcProductPriceCell : UserControl
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public UcProductPriceCell()
        {
            InitializeComponent();

            this.Loaded -= OnLoaded;
            this.Loaded += OnLoaded;
        }


        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static readonly DependencyProperty ProductViewModelProperty = DependencyProperty.Register("ProductViewModel", typeof(ProductViewModel), typeof(UcProductPriceCell), new PropertyMetadata(null));

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            RefreshControlState();
        }

        public void RefreshControlState()
        {
            if (this.ProductViewModel == null || !this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice.HasValue)
                return;

            if (this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice > 3)
            {
                txtBlockPrice.Foreground = new SolidColorBrush(Color.FromArgb(255, 210, 15, 10));

                // The +- X % will be brighter if the deal is good
                float _DifferenceColorAlpha = (float)this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice.Value / 5;
                _DifferenceColorAlpha = (float)Math.Max(1, Math.Min(1.5, _DifferenceColorAlpha));

                Color _DifferenceColor = Color.FromArgb(255, (byte)(170 * _DifferenceColorAlpha), (byte)(15 * _DifferenceColorAlpha), (byte)(10 * _DifferenceColorAlpha));
                var _BlockDifferenceColor = new SolidColorBrush(_DifferenceColor);
                txtBlockDifference.Foreground = _BlockDifferenceColor;
                txtBlockDifferenceQuote.Foreground = _BlockDifferenceColor;
                txtBlockDifferenceQuote.Text = "+";
            } else if (this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice > 0)
            {
                txtBlockPrice.Foreground = new SolidColorBrush(Color.FromArgb(255, 210, 70, 0));

                // The +- X % will be brighter if the deal is good
                float _DifferenceColorAlpha = (float)this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice.Value / 5;
                _DifferenceColorAlpha = (float)Math.Max(1, Math.Min(1.5, _DifferenceColorAlpha));

                Color _DifferenceColor = Color.FromArgb(255, (byte)(170 * _DifferenceColorAlpha), (byte)(70 * _DifferenceColorAlpha), (byte)(1 * _DifferenceColorAlpha));
                var _BlockDifferenceColor = new SolidColorBrush(_DifferenceColor);
                txtBlockDifference.Foreground = _BlockDifferenceColor;
                txtBlockDifferenceQuote.Foreground = _BlockDifferenceColor;
                txtBlockDifferenceQuote.Text = "+";
            } else if (this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice == 0)
            {
                var _BrushColor = new SolidColorBrush(Color.FromArgb(255, 25, 130, 225));
                txtBlockPrice.Foreground = _BrushColor;

                txtBlockDifference.Foreground = _BrushColor;
                txtBlockDifferenceQuote.Foreground = _BrushColor;
                txtBlockDifferenceQuote.Text = "+";
            } else
            {
                txtBlockPrice.Foreground = new SolidColorBrush(Color.FromArgb(255, 25, 175, 25));

                // The +- X % will be brighter if the deal is good
                float _DifferenceColorAlpha = (float)this.ProductViewModel.DifferenceBestPrice1YVsCurrentPrice.Value / 5;
                _DifferenceColorAlpha = (float)Math.Max(1, Math.Min(1.5, _DifferenceColorAlpha));

                Color _DifferenceColor = Color.FromArgb(255, (byte)(50 * _DifferenceColorAlpha), (byte)(170 * _DifferenceColorAlpha), (byte)(50 * _DifferenceColorAlpha));
                var _BlockDifferenceColor = new SolidColorBrush(_DifferenceColor);
                txtBlockDifference.Foreground = _BlockDifferenceColor;
                txtBlockDifferenceQuote.Foreground = _BlockDifferenceColor;
                //txtBlockDifferenceQuote.Text = "-"; - is already contained in price
            }
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductViewModel ProductViewModel
        {
            get { return (ProductViewModel)GetValue(ProductViewModelProperty); }
            set { SetValue(ProductViewModelProperty, value); }
        }
    }
}
