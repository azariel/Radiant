using System.Drawing;

namespace Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems
{
    public class PixelsInZoneWatchItemBitmapNoise : IPixelsInZoneWatchItemByBitmap
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public float NoiseTreshold { get; set; } = 1;
        public float WatchItemNbPixelsToTrigger { get; set; }
        public int MsToShowZoneOnDetection { get; set; } = 1500;
        public Color StrokeColor { get; set; } = Color.FromArgb(255, 139, 0, 0);
    }
}
