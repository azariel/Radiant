using System.Drawing;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Configuration
{
    public class RadiantReaderState
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double Height { get; set; } = 300;
        public Point Location { get; set; } = new(0, 0);
        public SelectedBookState SelectedBook { get; set; }
        public double VerticalScrollbarOffset { get; set; }
        public double Width { get; set; } = 200;
    }
}
