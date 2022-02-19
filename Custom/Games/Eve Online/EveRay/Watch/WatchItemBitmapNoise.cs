using System.Windows.Media;

namespace EveRay.Watch
{
    public class WatchItemBitmapNoise : IWatchItemByBitmap
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
