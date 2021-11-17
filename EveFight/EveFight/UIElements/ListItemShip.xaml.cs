using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EveFight.Configuration;
using EveFight.Models;

namespace EveFight.UIElements
{
    /// <summary>
    /// Interaction logic for ListItemShip.xaml
    /// </summary>
    public partial class ListItemShip : UserControl
    {
        public ListItemShip()
        {
            InitializeComponent();

            EveFightConfiguration _Config = EveFightConfigurationManager.ReloadConfig();
            fPlayerNameMaxDigits = _Config.PlayerNameMaxDigits;
            fUseThreatColorByDPS = _Config.UseThreatColorByDPS;
            fThreatColorByDPSModel = _Config.ThreatColorByDps;
        }

        private bool fCompactUI;
        private readonly int fPlayerNameMaxDigits;
        private Ship fShip;
        private readonly ThreatColorByDps fThreatColorByDPSModel;

        private readonly bool fUseThreatColorByDPS;

        private Brush GetBackgroundColorFromDPS()
        {
            const int _Alpha = 150;
            SolidColorBrush _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, 255, 255, 255));

            if (this.Ship == null)
                return _ColorBrush;

            foreach (ThreatColorDps _ThreatColorDps in fThreatColorByDPSModel.ThreatColorDpsList.OrderByDescending(o => o.Treshold))
            {
                if (this.Ship.DPS > _ThreatColorDps.Treshold)
                {
                    _ColorBrush = _ThreatColorDps.Color;
                    break;
                }
            }

            return _ColorBrush;
        }

        private Brush GetBackgroundColorFromType()
        {
            const int _Alpha = 150;

            if (this.Ship == null)
                return new SolidColorBrush(Color.FromArgb(_Alpha, 255, 255, 255));

            SolidColorBrush _ColorBrush;
            switch (this.Ship.ThreatLevel)
            {
                case ThreatLevel.Low:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, 175, 175, 0));
                    break;
                case ThreatLevel.Medium:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, 200, 125, 0));
                    break;
                case ThreatLevel.High:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, 150, 0, 0));
                    break;
                case ThreatLevel.Priority:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, 255, 0, 0));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _ColorBrush;
        }

        public void SetCompactUI()
        {
            // CompactUI
            LblPlayerName.FontSize = 10;
            LblDPS.FontSize = 10;
            LblThreatType.FontSize = 10;
            LblShipName.FontSize = 8;

            ListBoxItemMainGrid.Margin = new Thickness(0, -5, 0, 0);
            Panel.Margin = new Thickness(0, -12, 0, 0);
        }

        public void SetControlState()
        {
            ListBoxItemMainGrid.Background = fUseThreatColorByDPS ? GetBackgroundColorFromDPS() : GetBackgroundColorFromType();
            LblThreatType.Content = this.Ship.ThreatType;
            LblDPS.Content = $"{this.Ship.DPS} dps";
            LblPlayerName.Content = this.Ship.PlayerName[..Math.Min(fPlayerNameMaxDigits, this.Ship.PlayerName.Length)];
            LblShipName.Content = this.Ship.ShipName;
        }

        public Ship Ship
        {
            get => fShip;
            set
            {
                fShip = value;
                SetControlState();
            }
        }
    }
}
