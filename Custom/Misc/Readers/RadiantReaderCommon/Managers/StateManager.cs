using Radiant.Custom.Readers.RadiantReaderCommon.Configuration;
using Radiant.Custom.Readers.RadiantReaderCommon.DataBase;

namespace Radiant.Custom.Readers.RadiantReaderCommon.Managers
{
    public static class StateManager
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static void SetCurrentBook(RadiantReaderBookDefinitionModel aBookDefinition)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.State.SelectedBook ??= new SelectedBookState();

            _Config.State.SelectedBook.BookDefinitionId = aBookDefinition.BookDefinitionId;
            _Config.State.SelectedBook.BookChapterIndex = 0;

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }

        public static void SetCurrentBook(string aBookFilePath)
        {
            var _Config = RadiantReaderConfigurationManager.ReloadConfig();

            _Config.State.SelectedBook ??= new SelectedBookState();

            _Config.State.SelectedBook.BookDefinitionId = 0;
            _Config.State.SelectedBook.BookChapterIndex = 0;
            _Config.State.SelectedBook.AlternativeBookPathOnDisk = aBookFilePath;

            RadiantReaderConfigurationManager.SetConfigInMemory(_Config);
            RadiantReaderConfigurationManager.SaveConfigInMemoryToDisk();
        }
    }
}
