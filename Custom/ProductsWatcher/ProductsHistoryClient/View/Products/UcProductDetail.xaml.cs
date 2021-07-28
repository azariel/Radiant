using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Radiant.Common.OSDependent.Clipboard;
using Radiant.Common.Utils;
using Radiant.Custom.ProductsHistoryCommon.DataBase;

namespace ProductsHistoryClient.View.Products
{
    /// <summary>
    /// Interaction logic for UcProductDetail.xaml
    /// </summary>
    public partial class UcProductDetail : UserControl
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public class SerieColor
        {
            // ********************************************************************
            //                            Properties
            // ********************************************************************
            public SolidColorBrush FillColor { get; set; }
            public SolidColorBrush StrokeColor { get; set; }
        }

        public class AxisPoint
        {
            // ********************************************************************
            //                            Properties
            // ********************************************************************
            public DateTime DateTime { get; set; }
            public double Value { get; set; }
        }

        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public UcProductDetail()
        {
            InitializeComponent();

            this.Loaded -= OnLoaded;
            this.Loaded += OnLoaded;
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private readonly SerieColor[] fStrokeColors =
        {
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 0, 0, 255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 0, 255, 0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 255, 255, 0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 255, 128, 0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 255, 128, 0))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 255, 0, 128)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 128))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 32, 255, 128)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 32, 255, 128))
            },
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(8, 64, 128, 255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 32, 128, 255))
            }
        };

        private void AddBasicItemsToSerieCollection(SeriesCollection aSeriesCollection)
        {
            for (int i = 0; i < this.DetailProductViewModel.ProductModel.ProductDefinitionCollection.Count; i++)
            {
                RadiantClientProductDefinitionModel _ProductDefinition = this.DetailProductViewModel.ProductModel.ProductDefinitionCollection[i];
                LineSeries _Serie = new()
                {
                    LineSmoothness = 0.8f,
                    PointGeometrySize = 6
                };

                SerieColor _SerieColor = new()
                {
                    FillColor = new SolidColorBrush(Color.FromArgb(8, 255, 0, 0)),
                    StrokeColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0))
                };

                if (i < fStrokeColors.Length)
                    _SerieColor = fStrokeColors[i];

                _Serie.Stroke = _SerieColor.StrokeColor;

                _Serie.Fill = Brushes.Transparent;
                //_Serie.Fill = _SerieColor.FillColor;

                _Serie.Title = RegexUtils.GetWebSiteDomain(_ProductDefinition.Url);

                RadiantClientProductHistoryModel[] _ProductHistoryLast1Y = _ProductDefinition.ProductHistoryCollection.Where(w => w.InsertDateTime > DateTime.Now.AddYears(-1)).ToArray();

                ChartValues<AxisPoint> _Points = new();
                foreach (IGrouping<DateTime, RadiantClientProductHistoryModel> _ProductHistoryGroupOfThatDay in _ProductHistoryLast1Y.OrderBy(o => o.InsertDateTime).GroupBy(g => g.InsertDateTime.Date))
                {
                    if (!_ProductHistoryGroupOfThatDay.Any())
                        continue;

                    //_Points.Add(_ProductHistory.InsertDateTime.Day);
                    _Points.Add(new AxisPoint
                    {
                        DateTime = _ProductHistoryGroupOfThatDay.Key,
                        Value = _ProductHistoryGroupOfThatDay.Min(m => m.Price)
                    });
                }

                _Serie.Values = _Points;
                aSeriesCollection.Add(_Serie);
            }
        }

        private void AddBestPriceToSerieCollection(SeriesCollection aSeriesCollection)
        { // Show price on each points
            LineSeries _BestSerie = new()
            {
                DataLabels = true,
                PointGeometrySize = 10,
                LineSmoothness = 0.8f,
                FontSize = 16,
                LabelPoint = p => p.Y.ToString("F") + "$",
                Foreground = Brushes.White
            };

            SerieColor _BestSerieColor = new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(16, 0, 255, 255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255))
            };

            _BestSerie.Stroke = _BestSerieColor.StrokeColor;
            _BestSerie.Fill = _BestSerieColor.FillColor;
            _BestSerie.Title = "Best Price";

            RadiantClientProductHistoryModel[] _BestProductHistoryLast1Y = this.DetailProductViewModel.ProductModel.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection).Where(w => w.InsertDateTime > DateTime.Now.AddYears(-1)).ToArray();

            ChartValues<AxisPoint> _BestPoints = new();
            foreach (IGrouping<DateTime, RadiantClientProductHistoryModel> _ProductHistoryGroupOfThatDay in _BestProductHistoryLast1Y.OrderBy(o => o.InsertDateTime).GroupBy(g => g.InsertDateTime.Date))
            {
                if (!_ProductHistoryGroupOfThatDay.Any())
                    continue;

                //_Points.Add(_ProductHistory.InsertDateTime.Day);
                _BestPoints.Add(new AxisPoint
                {
                    DateTime = _ProductHistoryGroupOfThatDay.Key,
                    Value = _ProductHistoryGroupOfThatDay.Min(m => m.Price)
                });
            }

            _BestSerie.Values = _BestPoints;
            aSeriesCollection.Add(_BestSerie);
        }

        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            if (this.DetailProductViewModel == null)
                return;

            txtBlockProductName.Text = this.DetailProductViewModel.Name;

            txtBlockLastUpdateDate.Text = this.DetailProductViewModel.ProductModel.ProductDefinitionCollection.SelectMany(sm => sm.ProductHistoryCollection).Max(w => w.InsertDateTime).ToString("yyyy/MM/dd");

            CartesianMapper<AxisPoint> _DayConfig = Mappers.Xy<AxisPoint>().X(x => x.DateTime.Ticks / TimeSpan.FromDays(1).Ticks).Y(y => y.Value);
            SeriesCollection _SeriesCollection = new(_DayConfig);

            // Add Product definitions series
            AddBasicItemsToSerieCollection(_SeriesCollection);

            // Add Best price serie
            AddBestPriceToSerieCollection(_SeriesCollection);

            ProductHistoryChart.AxisX.Clear();
            ProductHistoryChart.AxisX.Add(new Axis());
            ProductHistoryChart.AxisX[0].LabelFormatter = value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("yyyy-MM-dd");
            ProductHistoryChart.Series = _SeriesCollection;

            txtBlockCurrentPrice.Text = $"{this.DetailProductViewModel.CurrentPrice:F}";
            txtBlockBestPrice365.Text = $"{this.DetailProductViewModel.BestPrice1Y:F}";
            txtBlockCurrentUrl.Text = this.DetailProductViewModel.Url;
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static readonly DependencyProperty DetailProductViewModelProperty = DependencyProperty.Register("DetailProductViewModel", typeof(ProductViewModel), typeof(UcProductDetail), new PropertyMetadata(null));

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public ProductViewModel DetailProductViewModel
        {
            get { return (ProductViewModel)GetValue(DetailProductViewModelProperty); }
            set { SetValue(DetailProductViewModelProperty, value); }
        }

        private void TxtBlockCurrentUrl_OnMouseUp(object aSender, MouseButtonEventArgs aE)
        {
            ClipboardManager.SetClipboardValue(txtBlockCurrentUrl.Text, false);
        }
    }
}
