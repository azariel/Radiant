using System.Drawing;

namespace Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems
{
    public interface IPixelsInZoneWatchItem
    {
        int MsToShowZoneOnDetection { get; set; }
        Color StrokeColor { get; set; }
    }
}
