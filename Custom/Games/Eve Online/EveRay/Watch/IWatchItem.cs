using System.Windows.Media;

namespace Radiant.Custom.Games.EveOnline.EveRay.Watch
{
    public interface IWatchItem
    {
        int MsToShowZoneOnDetection { get; set; }
        Color StrokeColor { get; set; }
    }
}
