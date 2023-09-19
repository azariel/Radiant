using System.Windows.Media;

namespace RadiantReader.Configuration
{
    /// <summary>
    /// Model representing the configuration related to settings
    /// </summary>
    public class RadiantReaderSettings
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int FontSize { get; set; } = 12;
        public Color ForeGroundColor { get; set; } = Color.FromArgb(255, 75, 75, 75);
        public bool TopMost { get; set; } = false;
    }
}
