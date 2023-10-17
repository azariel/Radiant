using System.Collections.Generic;

namespace Radiant.Common.API.Google.Sheets
{
    public class GoogleSheetRowData
    {
        /// <summary>
        /// List of objects representing data. Ex: first element is "Cell1Content", second element is 2, etc. Each cells are represented this way
        /// </summary>
        public List<object> RowCellData { get; set; } = new();
    }
}
