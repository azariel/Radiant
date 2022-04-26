using RadiantReader.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RadiantReader.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public SettingsUserControl()
        {
            InitializeComponent();

            SetControlState();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void SetControlState()
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            var _ForeGroundColorBrush = new SolidColorBrush(_Config.Settings.ForeGroundColor);

            lblTitle.Foreground = _ForeGroundColorBrush;
            lblForeGroundColor.Foreground = _ForeGroundColorBrush;
            ClrPckerForeGroundColor.SelectedColor = _Config.Settings.ForeGroundColor;
            txtBoxFontSize.Text = _Config.Settings.FontSize.ToString();
        }

        private void ClrPcker_Background_OnSelectedColorChanged(object aSender, RoutedPropertyChangedEventArgs<Color?> aE)
        {
            if (!ClrPckerForeGroundColor.SelectedColor.HasValue)
                return;

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.Settings.ForeGroundColor = ClrPckerForeGroundColor.SelectedColor.Value;

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        private void TxtBoxFontSize_OnKeyUp(object aSender, KeyEventArgs aE)
        {
            string _ValueString = txtBoxFontSize.Text;

            if (!int.TryParse(_ValueString, out int _Value))
                return;

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.Settings.FontSize = _Value;

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }
    }
}
