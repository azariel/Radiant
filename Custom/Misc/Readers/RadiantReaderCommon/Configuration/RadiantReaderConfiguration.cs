namespace Radiant.Custom.Readers.RadiantReaderCommon.Configuration
{
    public class RadiantReaderConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public RadiantReaderSettings Settings { get; set; } = new();
        public RadiantReaderState State { get; set; } = new();
    }
}
