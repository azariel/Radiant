namespace RadiantReader.Utils
{
    public static class BookStringUtils
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string FormatSummary(string aRawTitle)
        {
            string _OutputString = aRawTitle;

            _OutputString = _OutputString.Replace("&amp;", "&");

            return _OutputString;
        }

        public static string FormatTitle(string aRawTitle)
        {
            string _OutputString = aRawTitle;

            _OutputString = _OutputString.Replace("&amp;", "&");

            return _OutputString;
        }
    }
}
