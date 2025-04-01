namespace Radiant.Custom.Readers.RadiantReaderCommon.Configuration;

public class SelectedBookState
{
    // ********************************************************************
    //                            Properties
    // ********************************************************************
    public int BookChapterIndex { get; set; }
    public long BookDefinitionId { get; set; }
    public string AlternativeBookPathOnDisk { get; set; } = string.Empty; // If user isn't using the built-in way to handle books, he can load them from a physical space. We'll keep that path here so we can load it up when the app opens
}
