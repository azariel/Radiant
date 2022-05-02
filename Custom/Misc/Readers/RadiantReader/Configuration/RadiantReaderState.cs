using System.Drawing;

namespace RadiantReader.Configuration
{
    public class RadiantReaderState
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public double Height { get; set; } = 300;
        public Point Location { get; set; } = new(0, 0);
        public double Width { get; set; } = 200;
        public SelectedBookState SelectedBook { get; set; }
    }
}
