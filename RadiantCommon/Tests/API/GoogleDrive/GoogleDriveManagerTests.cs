using System.IO;
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
            GoogleDriveManager _GoogleDriveManager = new GoogleDriveManager("API/GoogleDrive/radiant-319014-d7d51b9a40d1.json");
            _GoogleDriveManager.PruneEverything();
            string _FileId = _GoogleDriveManager.GenerateFileId();
            string _FileContentEmpty = _GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.Empty(_FileContentEmpty);

            // Create dummy Version File
            _GoogleDriveManager.TryCreateNewFile(_FileId, "1.0.0.0", _FileId);
            string _FileContent = _GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.NotNull(_FileContent);
            Assert.NotEmpty(_FileContent);
            Assert.Equal("1.0.0.0", _FileContent);

            // Update dummy version
            _GoogleDriveManager.CreateOrUpdateFile(_FileId, "2.0.0.0");
            string _FileContentUpdated = _GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.NotNull(_FileContentUpdated);
            Assert.NotEmpty(_FileContentUpdated);
            Assert.Equal("2.0.0.0", _FileContentUpdated);

            // Clean up
            _GoogleDriveManager.DeleteFiles(new[] { _FileId });
            string _FileContentDeleted = _GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.Empty(_FileContentDeleted);
        }

        [Fact]
        public void ByteArrayFileCRUD()
        {
            GoogleDriveManager _GoogleDriveManager = new GoogleDriveManager("API/GoogleDrive/radiant-319014-d7d51b9a40d1.json");
            _GoogleDriveManager.PruneEverything();
            string _FileId = _GoogleDriveManager.GenerateFileId();
            string _FileContentEmpty = _GoogleDriveManager.TryFetchDocumentContentAsString(_FileId);
            Assert.Empty(_FileContentEmpty);

            // Create dummy byte[] File
            _GoogleDriveManager.TryCreateNewFile(_FileId, new byte[] { 10, 11 }, _FileId);
            byte[] _FileContent = _GoogleDriveManager.TryFetchDocumentContentAsByteArray(_FileId);
            Assert.NotNull(_FileContent);
            Assert.NotEmpty(_FileContent);
            Assert.Equal(new byte[] { 10, 11 }, _FileContent);

            // Update dummy byte[]
            _GoogleDriveManager.CreateOrUpdateFile(_FileId, new byte[] { 12, 13 });
            byte[] _FileContentUpdated = _GoogleDriveManager.TryFetchDocumentContentAsByteArray(_FileId);
            Assert.NotNull(_FileContentUpdated);
            Assert.NotEmpty(_FileContentUpdated);
            Assert.Equal(new byte[] { 12, 13 }, _FileContentUpdated);

            // Clean up
            _GoogleDriveManager.DeleteFiles(new[] { _FileId });
            byte[] _FileContentDeleted = _GoogleDriveManager.TryFetchDocumentContentAsByteArray(_FileId);
            Assert.Empty(_FileContentDeleted);
        }

        //[Fact]
        public void DEBUG_CreateDefaultFilesInRemote()
        {
            GoogleDriveManager _GoogleDriveManager = new GoogleDriveManager("API/GoogleDrive/radiant-319014-d7d51b9a40d1.json");
            _GoogleDriveManager.PruneEverything();

            string _VersionFileId = _GoogleDriveManager.TryCreateNewFile("ProductsHistoryClientVersion", "1.0.0.0");

            byte[] _DatabaseByteArray = File.ReadAllBytes("RadiantCommon.db");
            string _DatabaseFileId = _GoogleDriveManager.TryCreateNewFile("ProductsHistoryDataBase", _DatabaseByteArray);
        }
    }
}
