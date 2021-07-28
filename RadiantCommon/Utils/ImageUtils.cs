using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Radiant.Common.Utils
{
    public static class ImageUtils
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static byte[] ImageToByte2(Bitmap aBitmap)
        {
            using var _Stream = new MemoryStream();
            aBitmap.Save(_Stream, ImageFormat.Png);
            return _Stream.ToArray();
        }
    }
}
