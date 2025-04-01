using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Radiant.Custom.Readers.RadiantReaderCommon.Configuration;

namespace Radiant.Custom.Readers.RadiantReader.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl, IContentChild
    {
        // ********************************************************************
        //                            Constructors
        // ********************************************************************
        public SettingsUserControl()
        {
            InitializeComponent();

            SetControlState();
        }

        private void ChkTopMost_OnCheckChanged(object aSender, RoutedEventArgs aE)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.Settings.TopMost = chkTopMost.IsChecked ?? false;

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        private void ClrPcker_Background_OnSelectedColorChanged(object aSender, RoutedPropertyChangedEventArgs<Color?> aE)
        {
            if (!ClrPckerForeGroundColor.SelectedColor.HasValue)
                return;

            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.Settings.ForeGroundColor = System.Drawing.Color.FromArgb(ClrPckerForeGroundColor.SelectedColor.Value.A, ClrPckerForeGroundColor.SelectedColor.Value.R, ClrPckerForeGroundColor.SelectedColor.Value.G, ClrPckerForeGroundColor.SelectedColor.Value.B);

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        // ********************************************************************
        //                            Private
        // ********************************************************************
        private void SetControlState()
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();
            var _ForeGroundColorBrush = new SolidColorBrush(Color.FromArgb(_Config.Settings.ForeGroundColor.A, _Config.Settings.ForeGroundColor.R, _Config.Settings.ForeGroundColor.G, _Config.Settings.ForeGroundColor.B));

            lblTitle.Foreground = _ForeGroundColorBrush;
            lblForeGroundColor.Foreground = _ForeGroundColorBrush;
            ClrPckerForeGroundColor.SelectedColor = Color.FromArgb(_Config.Settings.ForeGroundColor.A, _Config.Settings.ForeGroundColor.R, _Config.Settings.ForeGroundColor.G, _Config.Settings.ForeGroundColor.B);
            txtBoxFontSize.Text = _Config.Settings.FontSize.ToString();
            chkTopMost.IsChecked = _Config.Settings.TopMost;
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

        public void UpdateInMemoryConfig() { }// Nothing to save, we save on the fly each config
    }
}
