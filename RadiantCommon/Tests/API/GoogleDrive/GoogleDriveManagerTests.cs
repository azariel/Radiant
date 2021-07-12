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
        public void FetchMostRecentVersion()
        {
            GoogleDriveManager.TryFetchDocumentContent("12JqzrT9t1zV39cVNZThLCNsXwFgRVCHD");
        }
    }
}
