using Radiant.Common.Diagnostics;
using System;
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

        public static byte[] TakeScreenshot(string aOutPutPath, out string aOutFilePath)
        {
            aOutFilePath = null;

            if (string.IsNullOrWhiteSpace(aOutPutPath))
            {
                LoggingManager.LogToFile("dbf7c7da-5970-45ad-9a91-d821c0793e6e", $"Provided output path is invalid [{aOutPutPath}]. Aborting screenshot.");
                return null;
            }

            try

            {
                using var _Bitmap = new Bitmap(1920, 1080, PixelFormat.Format32bppArgb);// TODO: get screen size dynamically
                using Graphics _Graphics = Graphics.FromImage(_Bitmap);

                _Graphics.CopyFromScreen(0, 0, 0, 0, _Bitmap.Size, CopyPixelOperation.SourceCopy);

                DateTime _Now = DateTime.Now;
                string _ImagePath = $"{_Now:yyyy-MM-dd HH.mm.ss.fff}.png";

                if (!Directory.Exists(aOutPutPath))
                    Directory.CreateDirectory(aOutPutPath);

                _Bitmap.Save(Path.Combine(aOutPutPath, _ImagePath));

                aOutFilePath = _ImagePath;

                return ImageToByte2(_Bitmap);
            }
            catch (Exception _Exception)
            {
                LoggingManager.LogToFile("1D33CEE8-20F0-4627-9EFE-B2FCFC4E71CE", "Couldn't take screenshot. Operation will be ignored.", _Exception);
            }

            return null;
        }
    }
}
