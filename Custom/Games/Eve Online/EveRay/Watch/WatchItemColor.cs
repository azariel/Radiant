using System.Drawing;

namespace Radiant.Custom.Games.EveOnline.EveRay.Watch
{
    public class WatchItemColor : IWatchItemByBitmap
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public Color Color { get; set; } = Color.White;
        public float ColorTreshold { get; set; } = 1;
        public float WatchItemNbPixelsToTrigger { get; set; }
        public int MsToShowZoneOnDetection { get; set; } = 1500;
        public System.Windows.Media.Color StrokeColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 139, 0, 0);
    }
}
