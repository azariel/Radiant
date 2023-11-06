using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Radiant.Common.Screen.Watcher.PixelsInZone;
using Radiant.Common.Screen.Watcher.PixelsInZone.Models;
using Radiant.Custom.Games.EveOnline.EveRay.Configuration;
using Point = System.Drawing.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Drawing.Size;

namespace Radiant.Custom.Games.EveOnline.EveRay
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

            EveRayConfiguration _Config = EveRayConfigurationManager.ReloadConfig();
            EveRayConfigurationManager.SaveConfigInMemoryToDisk();

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

            // Show zones if required
            foreach (PixelsInZoneAreaModel _ZoneWatcher in _Config.ZonesWatcher.Where(w => w.AlwaysShowZoneUI))
                ShowZoneAction(new Point(_ZoneWatcher.Zone.OnCurrentScreenShowLocation.X-1, _ZoneWatcher.Zone.OnCurrentScreenShowLocation.Y-1), new Size(_ZoneWatcher.Zone.Size.Width+4, _ZoneWatcher.Zone.Size.Height+4), System.Drawing.Color.FromArgb(255, 24, 115, 204), 1, null);

            // Delay process here to let the player set up ; )
            Thread.Sleep(1000);

            while (true)
            {
                PixelsInZoneEvaluator.EvaluateZones(_Config.ZonesWatcher.Where(w => w.Enabled).ToList(), ShowZoneAction);
                Thread.Sleep(5000);
            }
        }

        private void ShowZoneAction(Point aZoneLocation, Size aZoneSize, System.Drawing.Color aStrokeColor, int aStrokeThickness, int? aTimeOutMs = 1500)
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
                        Stroke = new SolidColorBrush(Color.FromArgb(aStrokeColor.A, aStrokeColor.R, aStrokeColor.G, aStrokeColor.B)),
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
