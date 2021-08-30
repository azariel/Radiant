using System.Collections.Generic;
using System.Drawing;

namespace EveRay.Watch
{
    public class WatchItemColors : IWatchItem
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public List<Color> Colors { get; set; } = new();
    }
}
