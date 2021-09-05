using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using EveRay.Configuration;
using EveRay.Zones;
using Color = System.Windows.Media.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Drawing.Size;

namespace EveRay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        public MainWindow()
        {
            InitializeComponent();

            //IntPtr _WindowHandle = new WindowInteropHelper(this).Handle;
            //int initialStyle = GetWindowLong(_WindowHandle, -20);
            //SetWindowLong(_WindowHandle, -20, initialStyle | 0x80000 | 0x20);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowExTransparent(hwnd);

            Thread _Thread = new(Start);
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        private void Start()
        {
            EveRayConfiguration _Config = EveRayConfigurationManager.ReloadConfig();
            //_Config.ZonesWatcher.Clear();
            //_Config.ZonesWatcher.Add(new ZoneWatcher()
            //{
            //    WatchItems = new List<IWatchItem>()
            //    {
            //        new WatchItemColor()
            //        {
            //            StrokeColorBrush = Brushes.DarkRed
            //        }
            //    }
            //});
            //EveRayConfigurationManager.SaveConfigInMemoryToDisk();

            // Show zones if required
            foreach (ZoneWatcher _ZoneWatcher in _Config.ZonesWatcher.Where(w => w.AlwaysShowZone))
                ShowZoneAction(_ZoneWatcher.Zone.Location, _ZoneWatcher.Zone.Size, Color.FromArgb(255, 24, 115, 204), 1, null);

            while (true)
            {
                ZoneEvaluator.EvaluateZones(_Config.ZonesWatcher.Where(w => w.Enabled).ToList(), ShowZoneAction);
                Thread.Sleep(3000);
            }
        }

        private void ShowZoneAction(Point aZoneLocation, Size aZoneSize, Color aStrokeColor, int aStrokeThickness, int? aTimeOutMs = 1500)
        {
            Task.Run(() =>
            {
                Rectangle rec = null;
                this.Dispatcher.Invoke(() =>
                {
                    // Create the rectangle
                    rec = new()
                    {
                        Width = aZoneSize.Width,
                        Height = aZoneSize.Height,
                        Fill = new SolidColorBrush(Color.FromArgb(8, aStrokeColor.R, aStrokeColor.G, aStrokeColor.B)),
                        Stroke = new SolidColorBrush(aStrokeColor),
                        StrokeThickness = aStrokeThickness,
                        IsHitTestVisible = false,
                    };

                    // Add to a canvas for example
                    TopCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, aZoneLocation.Y);
                    Canvas.SetLeft(rec, aZoneLocation.X);

                    var hwnd = new WindowInteropHelper(this).Handle;
                    SetWindowExTransparent(hwnd);
                });

                // Remove rectangle from screen
                if (aTimeOutMs.HasValue)
                {
                    //Task.Delay(aTimeOutMs.Value);
                    Thread.Sleep(aTimeOutMs.Value);
                    this.Dispatcher.Invoke(() =>
                    {
                        TopCanvas.Children.Remove(rec);
                    });
                }
            });
        }
    }
}
