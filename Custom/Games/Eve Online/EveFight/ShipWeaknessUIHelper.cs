using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EveFight.Models;

namespace EveFight
{
    internal static class ShipWeaknessUIHelper
    {
        internal static DamageWeakness GetBestDamageProfileToDealToShip(ShipDefinition aShipDefinition, bool aConsiderResistsModifier = true)
        {
            if (aShipDefinition == null)
                throw new Exception("Ship doesn't have a definition.");

            float _ShieldResistsModifier = aConsiderResistsModifier ? aShipDefinition.DamageProfileWeakness.ShieldResistsModifier : 1;
            float _ArmorResistsModifier = aConsiderResistsModifier ? aShipDefinition.DamageProfileWeakness.ArmorResistsModifier : 1;

            int _EM = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.EM * _ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.EM * _ArmorResistsModifier);
            int _Thermal = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Thermal * _ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Thermal * _ArmorResistsModifier);
            int _Kinetic = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Kinetic * _ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Kinetic * _ArmorResistsModifier);
            int _Explosion = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Explosion * _ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Explosion * _ArmorResistsModifier);

            int _MinResist = new[] { _EM, _Thermal, _Kinetic, _Explosion }.OrderBy(o => o).Take(count: 1).First();

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

        internal static void RefreshShieldResistsIcons(StackPanel aShieldStackPanel, ShipDefinition aShipDefinition, int aIconSize)
        {
            aShieldStackPanel.Children.Clear();

            if (aShipDefinition == null)
                return;

            int[] _Resists =
            {
                aShipDefinition.DamageProfileWeakness.ShieldResistProfile.EM,
                aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Thermal,
                aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Kinetic,
                aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Explosion
            };
            var _OrderedResists = _Resists.OrderBy(o => o);

            int _Limit = 2;
            int _LimitIteration = 0;
            for (int i = 0; i < 4; i++)
            {
                int _ExactResistToShowIcon = _OrderedResists.Skip(i).Take(count: 1).First();

                if (aShipDefinition.DamageProfileWeakness.ShieldResistProfile.EM == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.EM && aShipDefinition.DamageProfileWeakness.ShieldResistsModifier >= aShipDefinition.DamageProfileWeakness.ArmorResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aShieldStackPanel.Children.Add(_Border);
                    } else
                        aShieldStackPanel.Children.Add(_Image);

                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Thermal == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Thermal && aShipDefinition.DamageProfileWeakness.ShieldResistsModifier >= aShipDefinition.DamageProfileWeakness.ArmorResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aShieldStackPanel.Children.Add(_Border);
                    } else
                        aShieldStackPanel.Children.Add(_Image);

                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Kinetic == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Kinetic && aShipDefinition.DamageProfileWeakness.ShieldResistsModifier >= aShipDefinition.DamageProfileWeakness.ArmorResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aShieldStackPanel.Children.Add(_Border);
                    } else
                        aShieldStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Explosion == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Explosion && aShipDefinition.DamageProfileWeakness.ShieldResistsModifier >= aShipDefinition.DamageProfileWeakness.ArmorResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aShieldStackPanel.Children.Add(_Border);
                    } else
                        aShieldStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (_LimitIteration >= _Limit)
                    break;
            }
        }

        internal static void RefreshArmorResistsIcons(StackPanel aArmorStackPanel, ShipDefinition aShipDefinition, int aIconSize)
        {
            aArmorStackPanel.Children.Clear();

            if (aShipDefinition == null)
                return;

            int[] _Resists =
            {
                aShipDefinition.DamageProfileWeakness.ArmorResistProfile.EM,
                aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Thermal,
                aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Kinetic,
                aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Explosion
            };
            var _OrderedResists = _Resists.OrderBy(o => o);

            int _Limit = 2;
            int _LimitIteration = 0;
            for (int i = 0; i < 4; i++)
            {
                int _ExactResistToShowIcon = _OrderedResists.Skip(i).Take(count: 1).First();

                if (aShipDefinition.DamageProfileWeakness.ArmorResistProfile.EM == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.EM && aShipDefinition.DamageProfileWeakness.ArmorResistsModifier >= aShipDefinition.DamageProfileWeakness.ShieldResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aArmorStackPanel.Children.Add(_Border);
                    } else
                        aArmorStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Thermal == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Thermal && aShipDefinition.DamageProfileWeakness.ArmorResistsModifier >= aShipDefinition.DamageProfileWeakness.ShieldResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aArmorStackPanel.Children.Add(_Border);
                    } else
                        aArmorStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Kinetic == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Kinetic && aShipDefinition.DamageProfileWeakness.ArmorResistsModifier >= aShipDefinition.DamageProfileWeakness.ShieldResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aArmorStackPanel.Children.Add(_Border);
                    } else
                        aArmorStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Explosion == _ExactResistToShowIcon)
                {
                    var _Image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png")),
                        Height = aIconSize,
                        Width = aIconSize
                    };

                    if (GetBestDamageProfileToDealToShip(aShipDefinition) == DamageWeakness.Explosion && aShipDefinition.DamageProfileWeakness.ArmorResistsModifier >= aShipDefinition.DamageProfileWeakness.ShieldResistsModifier)
                    {
                        Border _Border = new()
                        {
                            Child = _Image,
                            BorderBrush = new SolidColorBrush(Colors.DeepSkyBlue),
                            CornerRadius = new CornerRadius(uniformRadius: 25),
                            BorderThickness = new Thickness(uniformLength: 3)
                        };
                        aArmorStackPanel.Children.Add(_Border);
                    } else
                        aArmorStackPanel.Children.Add(_Image);
                    ++_LimitIteration;
                }

                if (_LimitIteration >= _Limit)
                    break;
            }
        }

        private static void AddSingleResist(StackPanel aStackPanelRawResistsArmor, Image aResistImage, int aResistValue)
        {
            aStackPanelRawResistsArmor.Children.Add(aResistImage);

            var _ResistsValueLabel = new Label
            {
                Content = aResistValue,
                Foreground = new SolidColorBrush(Colors.AliceBlue),
                FontSize = 8,
                Margin = new Thickness(-5,0,0,0)
            };

            aStackPanelRawResistsArmor.Children.Add(_ResistsValueLabel);
        }

        private static void RefreshRawResistsIcons(StackPanel aStackPanelRawResistsArmor, int aIconSize, int aEM, int aThermal, int aKinetic, int aExplosive)
        {
            aStackPanelRawResistsArmor.Children.Clear();

            var _EMImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png")),
                Height = aIconSize,
                Width = aIconSize
            };
            AddSingleResist(aStackPanelRawResistsArmor, _EMImage, aEM);

            var _ThermalImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png")),
                Height = aIconSize,
                Width = aIconSize
            };
            AddSingleResist(aStackPanelRawResistsArmor, _ThermalImage, aThermal);

            var _KineticImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png")),
                Height = aIconSize,
                Width = aIconSize
            };
            AddSingleResist(aStackPanelRawResistsArmor, _KineticImage, aKinetic);

            var _ExplosiveImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png")),
                Height = aIconSize,
                Width = aIconSize
            };
            AddSingleResist(aStackPanelRawResistsArmor, _ExplosiveImage, aExplosive);
        }

        public static void RefreshRawArmorResistsIcons(StackPanel aStackPanelRawResistsArmor, ShipDefinition aShipDefinition, int aIconSize)
        {
            RefreshRawResistsIcons(aStackPanelRawResistsArmor, aIconSize, aShipDefinition.DamageProfileWeakness.ArmorResistProfile.EM, aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Thermal, aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Kinetic, aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Explosion);
        }

        public static void RefreshRawShieldResistsIcons(StackPanel aStackPanelRawResistsShield, ShipDefinition aShipDefinition, int aIconSize)
        {
            RefreshRawResistsIcons(aStackPanelRawResistsShield, aIconSize, aShipDefinition.DamageProfileWeakness.ShieldResistProfile.EM, aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Thermal, aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Kinetic, aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Explosion);
        }
    }
}
