using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace EveRay.Zones
{
    public class EveRayZone
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private Bitmap fBitmap; // Bitmap that holds the image
        private Size fSize = new(1, 1);

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public unsafe bool ContainsColors(List<Color> aColors, float aTreshold)
        {
            // Manipulatable graphics
            using Graphics bitmapGraphics = Graphics.FromImage(fBitmap);

            // Take a screenshot
            bitmapGraphics.CopyFromScreen(this.Location.X-7, this.Location.Y-7, 0, 0, fBitmap.Size);

            BitmapData _BitmapData = fBitmap.LockBits(new Rectangle(0, 0, fBitmap.Width, fBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int stride = Math.Abs(_BitmapData.Stride);
                byte* pImg = (byte*)_BitmapData.Scan0.ToPointer();
                for (int y = 0; y < fBitmap.Height; y++)
                {
                    byte* pRow = pImg + y * stride;
                    for (int x = 0; x < fBitmap.Width; x++)
                    {
                        float b = *pRow++;
                        float g = *pRow++;
                        float r = *pRow++;
                        pRow++; // skip alpha

                        //if (aColors.Any(a => Math.Abs(a.R-r) < aTreshold && Math.Abs(a.G-g) < aTreshold && Math.Abs(a.B-b) < aTreshold))
                        //    return true;

                        //if (r + g + b > 605)
                        //    return true;

                        float _Tolerance = 0.10f;// 5 % diff max
                        foreach (Color _Color in aColors)
                        {
                            float _GreenColor = Math.Max(0, (float)_Color.G);
                            float _BlueColor = Math.Max(0, (float)_Color.B);
                            float _Green = Math.Max(0, g);
                            float _Blue = Math.Max(0, b);

                            float _CurrentRatioRedToGreen = r / (_Green > 0 ? _Green : r);
                            float _RatioRedToGreen = (float)_Color.R / (_GreenColor > 0 ? _GreenColor : _Color.R);

                            if (Math.Abs(_RatioRedToGreen - _CurrentRatioRedToGreen) > _Tolerance)
                                continue;

                            float _CurrentRatioRedToBlue = r / (_Blue > 0 ? _Blue : r);
                            float _RatioRedToBlue = (float)_Color.R / (_BlueColor > 0 ? _BlueColor : _Color.R);

                            if (Math.Abs(_RatioRedToBlue - _CurrentRatioRedToBlue) > _Tolerance)
                                continue;

                            float _CurrentRatioGreenToBlue = g / (_Blue > 0 ? _Blue : g);
                            float _RatioGreenToBlue = (float)_Color.G / (_BlueColor > 0 ? _BlueColor : _Color.G);

                            if (Math.Abs(_RatioGreenToBlue - _CurrentRatioGreenToBlue) > _Tolerance)
                                continue;

                            if (Math.Abs(_Color.R - r) > aTreshold ||
                                Math.Abs(_Color.G - g) > aTreshold ||
                                Math.Abs(_Color.B - b) > aTreshold)
                                continue;

                            return true;
                        }
                    }
                }
            } finally
            {
                fBitmap.UnlockBits(_BitmapData);
                //fBitmap.Save("C:\\Temp\\a.png");
            }

            return false;
        }

        public bool ContainsColor(Color aColor, float aTreshold)
        {
            return ContainsColors(new List<Color> { aColor }, aTreshold);
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public Point Location { get; set; } = new(0, 0);

        public Size Size
        {
            get => fSize;
            set
            {
                fSize = value;
                fBitmap = new Bitmap(fSize.Width, fSize.Height);
            }
        }
    }
}
