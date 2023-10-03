using System.Windows.Controls;
using System.Windows.Media;
using Radiant.Custom.Games.EveOnline.EveFight.Configuration;

namespace Radiant.Custom.Games.EveOnline.EveFight.UIElements
{
    /// <summary>
    /// Interaction logic for HeaderStatsUC.xaml
    /// </summary>
    public partial class HeaderStatsUC : UserControl
    {
        public HeaderStatsUC()
        {
            InitializeComponent();
        }

        private void SetControlState()
        {
            EveFightConfiguration _Config = EveFightConfigurationManager.GetConfigFromMemory();

            LblDPSTank.Content = $"{this.TotalDPSInbound}";
            LblUserDPSTank.Content = this.TotalDPSOutbound;

            switch (this.TotalDPSInbound)
            {
                case int _Red when _Red >= _Config.TankInfo.TotalDPSRed:
                    LblDPSTank.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    break;
                case int _Orange when _Orange >= _Config.TankInfo.TotalDPSOrange:
                    LblDPSTank.Foreground = new SolidColorBrush(Color.FromArgb(255, 225, 95, 0));
                    break;
                case int _Yellow when _Yellow >= _Config.TankInfo.TotalDPSYellow:
                    LblDPSTank.Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 0));
                    break;
                default:
                    LblDPSTank.Foreground = new SolidColorBrush(Color.FromArgb(255, 25, 200, 25));
                    break;
            }

        }

        public void Update(int aTotalDpsInbound, int aTotalDpsOutbound)
        {
            this.TotalDPSInbound = aTotalDpsInbound;
            this.TotalDPSOutbound = aTotalDpsOutbound;

            SetControlState();
        }

        public int TotalDPSInbound { get; private set; }
        public int TotalDPSOutbound { get; private set; }
        
    }
}
