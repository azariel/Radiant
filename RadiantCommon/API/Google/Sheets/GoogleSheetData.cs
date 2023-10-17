using System.Collections.Generic;

namespace Radiant.Common.API.Google.Sheets
{
    public class GoogleSheetData
    {
        public List<GoogleSheetRowData> RowDataCollection { get; set; } = new();
    }
}
