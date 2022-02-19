using System.Windows.Media;

namespace EveRay.Watch
{
    public interface IWatchItem
    {
        int MsToShowZoneOnDetection { get; set; }
        Color StrokeColor { get; set; }
    }
}
