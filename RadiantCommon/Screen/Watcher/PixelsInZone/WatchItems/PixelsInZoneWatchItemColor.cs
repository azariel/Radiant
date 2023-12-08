using System.Drawing;

namespace Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems
{
    public class PixelsInZoneWatchItemColor : IPixelsInZoneWatchItemByBitmap
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public Color Color { get; set; } = Color.White;
        public float ColorTreshold { get; set; } = 1;
        public float WatchItemNbPixelsToTrigger { get; set; }
        public int MsToShowZoneOnDetection { get; set; } = 1500;
        public Color StrokeColor { get; set; } = Color.FromArgb(255, 139, 0, 0);
    }
}
