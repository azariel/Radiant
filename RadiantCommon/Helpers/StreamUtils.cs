using System.IO;

namespace Radiant.Common.Helpers
{
    public static class StreamUtils
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static Stream ToStream(this string aString)
        {
            MemoryStream _Stream = new MemoryStream();
            StreamWriter _Writer = new StreamWriter(_Stream);
            _Writer.Write(aString);
            _Writer.Flush();
            _Stream.Position = 0;
            return _Stream;
        }
    }
}
