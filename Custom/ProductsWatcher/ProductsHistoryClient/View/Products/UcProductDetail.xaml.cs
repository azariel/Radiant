using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Extensions.DependencyModel.Resolution;
using Radiant.Custom.ProductsHistoryCommon.DataBase;
using Brushes = System.Drawing.Brushes;

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
        private SerieColor[] fStrokeColors =
        {
            new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,0,255,0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,0,255,0))
            }, new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,0,0,255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,0,0,255))
            },new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,0,255,255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,0,255,255))
            },new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,255,255,0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,255,255,0))
            },new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,255,128,0)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,255,128,0))
            }, new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,255,0,128)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,255,0,128))
            }, new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,32,255,128)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,32,255,128))
            }, new()
            {
                FillColor = new SolidColorBrush(Color.FromArgb(128,64,128,255)),
                StrokeColor = new SolidColorBrush(Color.FromArgb(255,32,128,255))
            }
        };

        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            if (this.DetailProductViewModel == null)
                return;

            SeriesCollection _SeriesCollection = new SeriesCollection();
            for (int i = 0; i < this.DetailProductViewModel.ProductModel.ProductDefinitionCollection.Count; i++)
            {
                RadiantClientProductDefinitionModel _ProductDefinition = this.DetailProductViewModel.ProductModel.ProductDefinitionCollection[i];
                StepLineSeries _Serie = new();

                SerieColor _SerieColor = new()
                {
                    FillColor = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)),
                    StrokeColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0))
                };

                if (i < fStrokeColors.Length)
                    _SerieColor = fStrokeColors[i];

                _Serie.Stroke = _SerieColor.StrokeColor;
                _Serie.Fill = _SerieColor.FillColor;

                _Serie.Title = Regex.Match(_ProductDefinition.Url, "^(?:https?:\\/\\/)?(?:[^@\\n]+@)?(?:www\\.)?([^:\\/\\n?]+)").Value
                    .Replace("http://www.", "", StringComparison.InvariantCultureIgnoreCase)
                    .Replace("https://www.", "", StringComparison.InvariantCultureIgnoreCase);

                RadiantClientProductHistoryModel[] _ProductHistoryLast1Y = _ProductDefinition.ProductHistoryCollection.Where(w => w.InsertDateTime > DateTime.Now.AddYears(-1)).ToArray();

                ChartValues<double> _Points = new();
                foreach (IGrouping<DateTime, RadiantClientProductHistoryModel> _ProductHistoryGroupOfThatDay in _ProductHistoryLast1Y.OrderBy(o => o.InsertDateTime).GroupBy(g => g.InsertDateTime.Date))
                {
                    if (!_ProductHistoryGroupOfThatDay.Any())
                        continue;

                    //_Points.Add(_ProductHistory.InsertDateTime.Day);
                    _Points.Add(_ProductHistoryGroupOfThatDay.Min(m => m.Price));
                }

                _Serie.Values = _Points;
                _SeriesCollection.Add(_Serie);
            }

            ProductHistoryChart.Series = _SeriesCollection;

            txtBlockCurrentPrice.Text = $"{this.DetailProductViewModel.CurrentPrice:C}";
            txtBlockBestPrice365.Text = $"{this.DetailProductViewModel.BestPrice1Y:C}";

            //CartesianChart.Colors = new List<Color>()
            //{
            //    Color.FromArgb(255,255,255,255),
            //    Color.FromArgb(255,255,0,0),
            //    Color.FromArgb(255,255,255,0),
            //    Color.FromArgb(255,255,0,255),

            //};
            //ProductHistoryChart.LegendLocation = LegendLocation.Right;
            //ProductHistoryChart.AxisX = new AxesCollection()
            //{
            //    new DateAxis()
            //    {
            //        Title = "Date",
            //        Separator = new LiveCharts.Wpf.Separator()
            //        {
            //            IsEnabled = false
            //        }
            //    }
            //};
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
    }
}
