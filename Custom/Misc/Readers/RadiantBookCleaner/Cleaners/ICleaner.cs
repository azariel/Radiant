namespace Radiant.Custom.Readers.RadiantBookCleaner.Cleaners
{
    public interface ICleaner
    {
        void CleanFilesInDirectory(string aDirectoryPath, string aSearchPattern, SearchOption aSearchOption);
        string Clean(string aRawText);
    }
}
