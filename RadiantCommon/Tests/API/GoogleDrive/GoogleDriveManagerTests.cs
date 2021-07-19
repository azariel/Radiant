using Radiant.Common.API.GoogleDrive;
using Xunit;

namespace Radiant.Common.Tests.API.GoogleDrive
{
    public class GoogleDriveManagerTests
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        [Fact]
        public void CRUD()
        {
            string _FileId = GoogleDriveManager.GenerateFileId();
            GoogleDriveManager.PruneEverything();
            string _FileContentEmpty = GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.Empty(_FileContentEmpty);

            // Create dummy Version File
            GoogleDriveManager.TryCreateNewFile(_FileId, "1.0.0.0", _FileId);
            string _FileContent = GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.NotNull(_FileContent);
            Assert.NotEmpty(_FileContent);
            Assert.Equal("1.0.0.0", _FileContent);

            // Update dummy version
            GoogleDriveManager.CreateOrUpdateFile(_FileId, "2.0.0.0");
            string _FileContentUpdated = GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.NotNull(_FileContentUpdated);
            Assert.NotEmpty(_FileContentUpdated);
            Assert.Equal("2.0.0.0", _FileContentUpdated);

            // Clean up
            GoogleDriveManager.DeleteFiles(new[] { _FileId });
            string _FileContentDeleted = GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.Empty(_FileContentDeleted);
        }
    }
}
