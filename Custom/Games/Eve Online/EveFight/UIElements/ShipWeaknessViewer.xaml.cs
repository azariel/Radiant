using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using EveFight.Configuration;
using EveFight.Models;

namespace EveFight.UIElements
{
    /// <summary>
    /// Interaction logic for ShipWeaknessViewer.xaml
    /// </summary>
    public partial class ShipWeaknessViewer : UserControl
    {
        private EveFightConfiguration fConfig;

        public ShipWeaknessViewer()
        {
            InitializeComponent();

            // bind ship list to control
            fConfig = EveFightConfigurationManager.ReloadConfig();
            ShipsComboBox.ItemsSource = fConfig.ShipDefinitions.Select(s => s.ShipType);

            Loaded += OnLoaded;
        }

        private void OnLoaded(object aSender, RoutedEventArgs aE)
        {
            var textbox = (TextBox)ShipsComboBox.Template.FindName("PART_EditableTextBox", ShipsComboBox);
            if (textbox != null)
            {
                var parent = (Border)textbox.Parent;
                parent.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            }
        }

        private ShipDefinition? GetShipDefinitionFromShipName(string aShipName)
        {
            if (string.IsNullOrWhiteSpace(aShipName))
                return null;

            var _MatchingShips = fConfig.ShipDefinitions.Where(w => w.ShipType.Equals(aShipName.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToArray();

            if (_MatchingShips.Length > 1)
                throw new Exception($"found [{_MatchingShips.Length}] ships matching [{aShipName}] name in config.");

            return _MatchingShips.SingleOrDefault();
        }

        private void ShipsComboBox_OnKeyUp(object aSender, KeyEventArgs aE)
        {
            if (aE.Key != Key.Enter)
                return;

            // Show ship weaknesses
            string _ShipName = ShipsComboBox.SelectedValue as string;
            lblShipName.Content = $"{_ShipName}";


            ShipDefinition? _ShipDefinition = GetShipDefinitionFromShipName(_ShipName);

            if (_ShipDefinition == null)
                return;// just ignore

            lblThreatType.Content = _ShipDefinition.ThreatType;
            ShipsComboBox.Text = "";// clear input
            ShipWeaknessUIHelper.RefreshShieldResistsIcons(ResistShieldStackPanel, _ShipDefinition, 24);
            ShipWeaknessUIHelper.RefreshArmorResistsIcons(ResistArmorStackPanel, _ShipDefinition, 24);
            ShipWeaknessUIHelper.RefreshRawShieldResistsIcons(StackPanelRawResistsShield, _ShipDefinition, 12);
            ShipWeaknessUIHelper.RefreshRawArmorResistsIcons(StackPanelRawResistsArmor, _ShipDefinition, 12);

            DamageWeakness _DamageWeakness = ShipWeaknessUIHelper.GetBestDamageProfileToDealToShip(_ShipDefinition);

            SetBestDamageWeaknessUI(_DamageWeakness);
            RefreshWeaknessesBorders(_ShipDefinition);
        }

        private void RefreshWeaknessesBorders(ShipDefinition aShipDefinition)
        {
            if (!(aShipDefinition.DamageProfileWeakness.ShieldResistsModifier > aShipDefinition.DamageProfileWeakness.ArmorResistsModifier))
            {
                ArmorBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                ArmorBorder.BorderThickness = new Thickness(0, 0, 0, 2);
                ShieldBorder.BorderThickness = new Thickness(0);
            } else

            {
                ShieldBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                ShieldBorder.BorderThickness = new Thickness(0, 0, 0, 2);
                ArmorBorder.BorderThickness = new Thickness(0);
            }
        }

        private void SetBestDamageWeaknessUI(DamageWeakness aDamageWeakness)
        {
            BitmapImage? _SourceImage;
            switch (aDamageWeakness)
            {
                case DamageWeakness.EM:
                    _SourceImage = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/EM.png"));
                    break;
                case DamageWeakness.Thermal:
                    _SourceImage = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Thermal.png"));
                    break;
                case DamageWeakness.Explosion:
                    _SourceImage = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Explosion.png"));
                    break;
                case DamageWeakness.Kinetic:
                    _SourceImage = new BitmapImage(new Uri("pack://application:,,,/EveFight;component/Resources/Kinetic.png"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aDamageWeakness), aDamageWeakness, null);
            }

            BestDamageWeaknessLeftImage.Source = _SourceImage;
        }

        public void Refresh()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(() =>
                {
                    ShipsComboBox.Focus();// Set Logical Focus
                    Keyboard.Focus(ShipsComboBox);// Set Keyboard Focus
                }));
        }
    }
}
