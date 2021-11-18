using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using EveFight.Configuration;
using EveFight.Managers;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace EveFight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        public MainWindow()
        {
            InitializeComponent();

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowExTransparent(hwnd);

            // Attach events
            this.Closed -= OnClosed;
            this.Closed += OnClosed;

            // Handle background threads
            Thread _Thread = new(Start);
            _Thread.IsBackground = true;
            _Thread.Start();
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        private void LoadUI()
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            // Handle UI Interactivty
            if (!_Config.UILocked)
            {
                MainGrid.MouseDown -= MainGrid_OnMouseDown;
                MainGrid.MouseDown += MainGrid_OnMouseDown;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                this.IsHitTestVisible = false;
                MainGrid.IsHitTestVisible = false;
            }

            // Load UI position and size
            this.Left = _Config.UILocation.X;
            this.Top = _Config.UILocation.Y;

            this.Width = Math.Max(_Config.UISize.Width, 50);
            this.Height = Math.Max(_Config.UISize.Height, 50);
        }

        private void MainGrid_OnMouseDown(object aSender, MouseButtonEventArgs aE)
        {
            if (aE.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MainWindow_OnLoaded(object aSender, RoutedEventArgs aE)
        {
            LoadUI();
        }

        private void OnClosed(object? aSender, EventArgs aE)
        {
            // Save state
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            _Config.UILocation = new Point((int)this.Left, (int)this.Top);
            _Config.UISize = new Size((int)this.Width, (int)this.Height);

            EveFightConfigurationManager.SaveConfigInMemoryToDisk();
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private void ShowRedBorderIfRequired(int aTotalDps)
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            if (aTotalDps > _Config.TankInfo.TotalDPSRed)
            {
                //Show Border
                MainBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 20, 20));
                return;
            }

            // Hide Border
            MainBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void Start()
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();

            while (true)
            {
                // Regenerate ListItemShips
                ShipsManager.UpdateShips();

                this.Dispatcher.Invoke(() =>
                {
                    ShipsManager.UpdateShipListItems(_Config.CompactUI, ShipsListBox);

                    int _TotalDPS = ShipsManager.GetTotalDPS();
                    StatsUc.Update(_TotalDPS, ShipsManager.UserShip.DPS);

                    ShowRedBorderIfRequired(_TotalDPS);
                });

                // TODO: go as far a 3 sec in non-combat situation and as low as 0.5 sec in combat
                Thread.Sleep(1000);
            }
        }

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}
