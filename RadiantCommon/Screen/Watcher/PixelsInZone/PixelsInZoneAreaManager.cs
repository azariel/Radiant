﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems;

namespace Radiant.Common.Screen.Watcher.PixelsInZone
{
    public class PixelsInZoneAreaManager
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private Bitmap fBitmap;// Bitmap that holds the image
        private Size fSize = new(1, 1);
        private bool fIsFirstCheck = true;

        public bool ContainsColor(PixelsInZoneWatchItemColors.WatchItemDetectionType aWatchItemDetectionType, Color aColor, float aTreshold, float aWatchItemNbPixelsToTrigger, bool aSaveImageOnDisk, out Point? aHitPointLocation)
        {
            return ContainsColors(aWatchItemDetectionType, new List<Color> { aColor }, aTreshold, aWatchItemNbPixelsToTrigger, aSaveImageOnDisk, out aHitPointLocation);
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************
        public unsafe bool ContainsColors(PixelsInZoneWatchItemColors.WatchItemDetectionType aWatchItemDetectionType, List<Color> aColors, float aTreshold, float aWatchItemNbPixelsToTrigger, bool aSaveImageOnDisk, out Point? aHitPointLocation)
        {
            aHitPointLocation = null;

            // Manipulatable graphics
            using Graphics bitmapGraphics = Graphics.FromImage(fBitmap);

            // Take a screenshot
            bitmapGraphics.CopyFromScreen(this.Location.X - 6, this.Location.Y - 6, 0, 0, fBitmap.Size);

            BitmapData _BitmapData = fBitmap.LockBits(new Rectangle(0, 0, fBitmap.Width, fBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int _NbPixelsMatchingWatchItem = 0;

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
                        pRow++;// skip alpha

                        float _Tolerance = 0.10f;// 10 % diff max
                        foreach (Color _Color in aColors)
                        {
                            float _GreenColor = Math.Max(0, (float)_Color.G);
                            float _BlueColor = Math.Max(0, (float)_Color.B);
                            float _Green = Math.Max(0, g);
                            float _Blue = Math.Max(0, b);

                            float _CurrentRatioRedToGreen = r / (_Green > 0 ? _Green : r);
                            float _RatioRedToGreen = _Color.R / (_GreenColor > 0 ? _GreenColor : _Color.R);
                            float _CurrentRatioRedToBlue = r / (_Blue > 0 ? _Blue : r);
                            float _RatioRedToBlue = _Color.R / (_BlueColor > 0 ? _BlueColor : _Color.R);
                            float _CurrentRatioGreenToBlue = g / (_Blue > 0 ? _Blue : g);
                            float _RatioGreenToBlue = _Color.G / (_BlueColor > 0 ? _BlueColor : _Color.G);

                            if (Math.Abs(_RatioRedToGreen - _CurrentRatioRedToGreen) > _Tolerance ||
                                Math.Abs(_RatioRedToBlue - _CurrentRatioRedToBlue) > _Tolerance ||
                                Math.Abs(_RatioGreenToBlue - _CurrentRatioGreenToBlue) > _Tolerance ||
                                Math.Abs(_Color.R - r) + Math.Abs(_Color.G - g) + Math.Abs(_Color.B - b) > aTreshold)
                            {
                                if (aWatchItemDetectionType == PixelsInZoneWatchItemColors.WatchItemDetectionType.WhiteList)
                                    continue;
                            }

                            ++_NbPixelsMatchingWatchItem;

                            if (aHitPointLocation == null)
                                aHitPointLocation = new Point(this.Location.X + x, this.Location.Y - 1 + y);

                            if (_NbPixelsMatchingWatchItem >= aWatchItemNbPixelsToTrigger)
                                break;
                        }
                    }
                }
            } finally
            {
                fBitmap.UnlockBits(_BitmapData);
            }

            if (aSaveImageOnDisk)
                SaveImageIfFirstCheckForManualValidation();

            if (_NbPixelsMatchingWatchItem >= aWatchItemNbPixelsToTrigger)
            {
                if (aSaveImageOnDisk)
                {
                    CreateTempFolderIfDoesntExists();
                    fBitmap.Save($"C:\\Temp\\TRIGGERED_COLOR_{Guid.NewGuid()}.png");
                }

                return true;
            }

            return false;
        }

        private void CreateTempFolderIfDoesntExists()
        {
            if (!Directory.Exists("C:\\Temp"))
                Directory.CreateDirectory("C:\\Temp");
        }

        private void SaveImageIfFirstCheckForManualValidation()
        {
            if (!fIsFirstCheck)
                return;

            fIsFirstCheck = false;
            CreateTempFolderIfDoesntExists();
            fBitmap.Save($"C:\\Temp\\FOR_VALIDATION_{Guid.NewGuid()}.png");
        }

        public unsafe bool IsDifferentFromLastEvaluation(float aNoiseTreshold, float aWatchItemNbPixelsToTrigger, bool aSaveImageOnDisk)
        {
            Bitmap _LastBitmap = new Bitmap(fBitmap);

            // Manipulatable graphics
            using Graphics bitmapGraphics = Graphics.FromImage(fBitmap);

            // Take a screenshot
            bitmapGraphics.CopyFromScreen(this.Location.X - 7, this.Location.Y - 7, 0, 0, fBitmap.Size);

            if (_LastBitmap.Height != fBitmap.Height || _LastBitmap.Width != fBitmap.Width)
                return true;

            BitmapData _LastBitmapData = _LastBitmap.LockBits(new Rectangle(0, 0, _LastBitmap.Width, _LastBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData _BitmapData = fBitmap.LockBits(new Rectangle(0, 0, fBitmap.Width, fBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int _NbPixelsMatching = 0;
            try
            {
                int strideLast = Math.Abs(_LastBitmapData.Stride);
                byte* pImgLast = (byte*)_LastBitmapData.Scan0.ToPointer();

                int stride = Math.Abs(_BitmapData.Stride);
                byte* pImg = (byte*)_BitmapData.Scan0.ToPointer();
                for (int y = 0; y < fBitmap.Height; y++)
                {
                    byte* pRow = pImg + y * stride;
                    byte* pRowLast = pImgLast + y * strideLast;
                    for (int x = 0; x < fBitmap.Width; x++)
                    {
                        float b = *pRow++;
                        float g = *pRow++;
                        float r = *pRow++;
                        pRow++;// skip alpha

                        // We're skipping black and white colors for black/white screen avoidance
                        if ((r <= 5 && g <= 5 && b <= 5) ||
                            (r >= 250 && g >= 250 && b >= 250))
                            continue;

                        float bLast = *pRowLast++;
                        float gLast = *pRowLast++;
                        float rLast = *pRowLast++;
                        pRowLast++;// skip alpha

                        if (Math.Abs(rLast - r) + Math.Abs(gLast - g) + Math.Abs(bLast - b) > aNoiseTreshold)
                            ++_NbPixelsMatching;

                        if (_NbPixelsMatching >= aWatchItemNbPixelsToTrigger)
                            break;
                    }
                }
            } finally
            {
                _LastBitmap.UnlockBits(_LastBitmapData);
                fBitmap.UnlockBits(_BitmapData);
            }

            if (aSaveImageOnDisk)
                SaveImageIfFirstCheckForManualValidation();

            if (_NbPixelsMatching >= aWatchItemNbPixelsToTrigger)
            {
                if (aSaveImageOnDisk)
                {
                    CreateTempFolderIfDoesntExists();
                    fBitmap.Save($"C:\\Temp\\NOISE_{Guid.NewGuid()}_A.png");
                    _LastBitmap.Save($"C:\\Temp\\NOISE_{Guid.NewGuid()}_B.png");
                }

                return true;
            }

            return false;
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public Point Location { get; set; } = new(0, 0);
        public Point OnCurrentScreenShowLocation { get; set; } = new(0, 0);

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
