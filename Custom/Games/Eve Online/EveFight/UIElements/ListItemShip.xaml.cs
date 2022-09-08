using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        private readonly int fPlayerNameMaxDigits;
        private Ship fShip;
        private readonly ThreatColorByDps fThreatColorByDPSModel;

        private readonly bool fUseThreatColorByDPS;

        private Brush GetBackgroundColorFromDPS()
        {
            const int _Alpha = 150;
            SolidColorBrush _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, r: 255, g: 255, b: 255));

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

            // If ship is hasn't hit us for half the cycle time, gray it out
            if ((DateTime.Now - this.Ship.LastUpdate).TotalMilliseconds > EveFightConfigurationManager.ReloadConfig().DpsCycleMs / 2)
                _ColorBrush = new SolidColorBrush(Color.FromArgb(a: 50, _ColorBrush.Color.R, _ColorBrush.Color.G, _ColorBrush.Color.B));

            return _ColorBrush;
        }

        private Brush GetBackgroundColorFromType()
        {
            const int _Alpha = 150;

            if (this.Ship == null)
                return new SolidColorBrush(Color.FromArgb(_Alpha, r: 255, g: 255, b: 255));

            SolidColorBrush _ColorBrush;
            switch (this.Ship.ThreatLevel)
            {
                case ThreatLevel.Low:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, r: 175, g: 175, b: 0));
                    break;
                case ThreatLevel.Medium:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, r: 200, g: 125, b: 0));
                    break;
                case ThreatLevel.High:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, r: 150, g: 0, b: 0));
                    break;
                case ThreatLevel.Priority:
                    _ColorBrush = new SolidColorBrush(Color.FromArgb(_Alpha, r: 255, g: 0, b: 0));
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

            ListBoxItemMainGrid.Margin = new Thickness(left: 0, top: -3, right: 0, bottom: 0);
            LblShipName.Margin = new Thickness(left: 0, top: -12, right: 0, bottom: 0);
            LblThreatType.Margin = new Thickness(left: 0, top: -12, right: 0, bottom: 0);
        }

        public void SetControlState()
        {
            ListBoxItemMainGrid.Background = fUseThreatColorByDPS ? GetBackgroundColorFromDPS() : GetBackgroundColorFromType();
            LblThreatType.Content = this.Ship.ThreatType;
            LblDPS.Content = $"{this.Ship.DPS} dps";
            LblPlayerName.Content = this.Ship.PlayerName[..Math.Min(fPlayerNameMaxDigits, this.Ship.PlayerName.Length)];
            LblShipName.Content = this.Ship.ShipName;
            
            ShipWeaknessUIHelper.RefreshShieldResistsIcons(ResistArmorStackPanel, this.Ship.Definition, 18);
            ShipWeaknessUIHelper.RefreshArmorResistsIcons(ResistArmorStackPanel, this.Ship.Definition, 18);
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
