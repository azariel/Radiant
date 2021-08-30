using System;
using System.Drawing;
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
using Rectangle = System.Windows.Shapes.Rectangle;

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
            //    Zone = new EveRayZone()
            //    {
            //        Location = new Point(10, 10),
            //        Size = new Size(1000, 1000)
            //    },
            //    WatchItem = new WatchItemColor()
            //    {
            //        Color = Color.White
            //    },
            //    TriggerAction = new SoundAlertTriggerAction()
            //    {
            //       SoundFilePath = @"H:\Games\Misc\EVE\PixelNotifier\EVE Online Audio Warnings and Chimes v2\EVE Online - Structure Warning.wav"
            //    }
            //});
            //EveRayConfigurationManager.SaveConfigInMemoryToDisk();

            while (true)
            {
                ZoneEvaluator.EvaluateZones(_Config.ZonesWatcher.Where(w=>w.Enabled).ToList(), ShowZoneAction);
                Thread.Sleep(3000);
            }
        }

        private void ShowZoneAction(ZoneWatcher aZoneWatcher)
        {
            Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Rectangle rec = new();

                    // Create the rectangle
                    rec = new()
                    {
                        Width = aZoneWatcher.Zone.Size.Width,
                        Height = aZoneWatcher.Zone.Size.Height,
                        Fill = new SolidColorBrush(Color.FromArgb(8,255,0,0)),
                        Stroke = System.Windows.Media.Brushes.DarkRed,
                        StrokeThickness = 1,
                        IsHitTestVisible = false,
                    };

                    // Add to a canvas for example
                    TopCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, aZoneWatcher.Zone.Location.Y);
                    Canvas.SetLeft(rec, aZoneWatcher.Zone.Location.X);

                    var hwnd = new WindowInteropHelper(this).Handle;
                    SetWindowExTransparent(hwnd);
                });

                Thread.Sleep(1500);

                this.Dispatcher.Invoke(() =>
                {
                    TopCanvas.Children.Clear();
                });
            });
        }
    }
}
