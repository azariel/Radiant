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

            RefreshShieldResistsIcons();
            RefreshArmorResistsIcons();
        }

        private DamageWeakness GetBestDamageProfileToDealToShip()
        {
            if (this.Ship.Definition == null)
                throw new Exception("Ship doesn't have a definition.");

            int _EM = (int)(this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.EM * this.Ship.Definition.DamageProfileWeakness.ShieldResistsModifier) + (int)(this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.EM * this.Ship.Definition.DamageProfileWeakness.ArmorResistsModifier);
            int _Thermal = (int)(this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Thermal * this.Ship.Definition.DamageProfileWeakness.ShieldResistsModifier) + (int)(this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Thermal * this.Ship.Definition.DamageProfileWeakness.ArmorResistsModifier);
            int _Kinetic = (int)(this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Kinetic * this.Ship.Definition.DamageProfileWeakness.ShieldResistsModifier) + (int)(this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Kinetic * this.Ship.Definition.DamageProfileWeakness.ArmorResistsModifier);
            int _Explosion = (int)(this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Explosion * this.Ship.Definition.DamageProfileWeakness.ShieldResistsModifier) + (int)(this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Explosion * this.Ship.Definition.DamageProfileWeakness.ArmorResistsModifier);

            int _MinResist = new[] { _EM, _Thermal, _Kinetic, _Explosion }.OrderBy(o => o).Take(1).First();

            if (_EM == _MinResist)
                return DamageWeakness.EM;

            if (_Thermal == _MinResist)
                return DamageWeakness.Thermal;

            if (_Kinetic == _MinResist)
                return DamageWeakness.Kinetic;

            if (_Explosion == _MinResist)
                return DamageWeakness.Explosion;

            throw new Exception("Ship weakness unknown to get best damage profile.");
        }

        private void RefreshShieldResistsIcons()
        {
            ResistShieldStackPanel.Children.Clear();

            if (this.Ship.Definition == null)
                return;

            int _IconSize = 18;
            int[] _Resists =
            {
                this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.EM,
                this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Thermal,
                this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Kinetic,
                this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Explosion
            };
            var _OrderedResists = _Resists.OrderBy(o => o);

            int _Limit = 2;
            int _LimitIteration = 0;
            for (int i = 0; i < 4; i++)
            {
                int _ExactResistToShowIcon = _OrderedResists.Skip(i).Take(1).First();

                if (this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.EM == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    };

                    if (GetBestDamageProfileToDealToShip() == DamageWeakness.EM)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(25),
                            BorderThickness = new Thickness(3)
                        };
                        ResistShieldStackPanel.Children.Add(_Border);
                    } else
                        ResistShieldStackPanel.Children.Add(_Image);

                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Thermal == _ExactResistToShowIcon)
                {
                    ResistShieldStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Kinetic == _ExactResistToShowIcon)
                {
                    ResistShieldStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ShieldResistProfile.Explosion == _ExactResistToShowIcon)
                {
                    ResistShieldStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (_LimitIteration >= _Limit)
                    break;
            }
        }

        private void RefreshArmorResistsIcons()
        {
            ResistArmorStackPanel.Children.Clear();

            if (this.Ship.Definition == null)
                return;

            int _IconSize = 18;
            int[] _Resists =
            {
                this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.EM,
                this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Thermal,
                this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Kinetic,
                this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Explosion
            };
            var _OrderedResists = _Resists.OrderBy(o => o);

            int _Limit = 2;
            int _LimitIteration = 0;
            for (int i = 0; i < 4; i++)
            {
                int _ExactResistToShowIcon = _OrderedResists.Skip(i).Take(1).First();

                if (this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.EM == _ExactResistToShowIcon)
                {
                    ResistArmorStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Thermal == _ExactResistToShowIcon)
                {
                    ResistArmorStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Kinetic == _ExactResistToShowIcon)
                {
                    ResistArmorStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (this.Ship.Definition.DamageProfileWeakness.ArmorResistProfile.Explosion == _ExactResistToShowIcon)
                {
                    ResistArmorStackPanel.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png")),
                        Height = _IconSize,
                        Width = _IconSize
                    });
                    ++_LimitIteration;
                }

                if (_LimitIteration >= _Limit)
                    break;
            }
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
