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
        internal static DamageWeakness GetBestDamageProfileToDealToShip(ShipDefinition aShipDefinition)
        {
            if (aShipDefinition == null)
                throw new Exception("Ship doesn't have a definition.");

            int _EM = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.EM * aShipDefinition.DamageProfileWeakness.ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.EM * aShipDefinition.DamageProfileWeakness.ArmorResistsModifier);
            int _Thermal = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Thermal * aShipDefinition.DamageProfileWeakness.ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Thermal * aShipDefinition.DamageProfileWeakness.ArmorResistsModifier);
            int _Kinetic = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Kinetic * aShipDefinition.DamageProfileWeakness.ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Kinetic * aShipDefinition.DamageProfileWeakness.ArmorResistsModifier);
            int _Explosion = (int)(aShipDefinition.DamageProfileWeakness.ShieldResistProfile.Explosion * aShipDefinition.DamageProfileWeakness.ShieldResistsModifier) + (int)(aShipDefinition.DamageProfileWeakness.ArmorResistProfile.Explosion * aShipDefinition.DamageProfileWeakness.ArmorResistsModifier);

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
    }
}
