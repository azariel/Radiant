using System.Collections.Generic;
using System.Drawing;

namespace EveRay.Watch
{
    public class WatchItemColors : IWatchItemByBitmap
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public List<Color> Colors { get; set; } = new();
        public float ColorTreshold { get; set; } = 1;
        public float WatchItemNbPixelsToTrigger { get; set; }
        public int MsToShowZoneOnDetection { get; set; } = 1500;
        public System.Windows.Media.Color StrokeColor { get; set; } = System.Windows.Media.Color.FromArgb(255, 139, 0, 0);
    }
}
