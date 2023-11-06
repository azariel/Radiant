using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Radiant.Common.Screen.Watcher.PixelsInZone.Models;
using Radiant.Common.Screen.Watcher.PixelsInZone.TriggerActions;
using Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems;

namespace Radiant.Common.Screen.Watcher.PixelsInZone
{
    public static class PixelsInZoneEvaluator
    {
        // ********************************************************************
        //                            Private
        // ********************************************************************
        private static void TriggerAction(ITriggerAction aTriggerAction)
        {
            aTriggerAction?.Trigger();
        }

        // ********************************************************************
        //                            Public
        // ********************************************************************

        public static void EvaluateZones(List<PixelsInZoneAreaModel> aConfigZonesWatcher, Action<Point, Size, Color, int, int?> aShowZoneAction)
        {
            foreach (PixelsInZoneAreaModel _ZoneWatcher in aConfigZonesWatcher)
            {
                bool _Triggered = false;
                foreach (IPixelsInZoneWatchItem _ZoneWatcherWatchItem in _ZoneWatcher.WatchItems)
                {
                    if (_Triggered)
                        break;

                    switch (_ZoneWatcherWatchItem)
                    {
                        case PixelsInZoneWatchItemBitmapNoise _WatchItemBitmapNoise:

                            if (_ZoneWatcher.Zone.IsDifferentFromLastEvaluation(_WatchItemBitmapNoise.NoiseTreshold, _WatchItemBitmapNoise.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDiskOnTriggered))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);
                                aShowZoneAction?.Invoke(_ZoneWatcher.Zone.Location, _ZoneWatcher.Zone.Size, _ZoneWatcherWatchItem.StrokeColor, 1, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

                                Thread.Sleep(1000);
                                _Triggered = true;
                            }

                            break;
                        case PixelsInZoneWatchItemColor _WatchItemColor:
                            if (_ZoneWatcher.Zone.ContainsColor(PixelsInZoneWatchItemColors.WatchItemDetectionType.WhiteList, _WatchItemColor.Color, _WatchItemColor.ColorTreshold, _WatchItemColor.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDiskOnTriggered, out Point? _HitPointLocationOfColor))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);

                                if (_HitPointLocationOfColor.HasValue)
                                    aShowZoneAction?.Invoke(new Point(_HitPointLocationOfColor.Value.X-5, _HitPointLocationOfColor.Value.Y-5), new Size(10,10), _ZoneWatcherWatchItem.StrokeColor, 2, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

                                Thread.Sleep(1000);
                                _Triggered = true;
                            }

                            break;
                        case PixelsInZoneWatchItemColors _WatchItemColors:
                            if (_ZoneWatcher.Zone.ContainsColors(_WatchItemColors.DetectionType, _WatchItemColors.Colors, _WatchItemColors.ColorTreshold, _WatchItemColors.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDiskOnTriggered, out Point? _HitPointLocationOfColors))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);

                                if (_HitPointLocationOfColors.HasValue)
                                    aShowZoneAction?.Invoke(new Point(_HitPointLocationOfColors.Value.X-5, _HitPointLocationOfColors.Value.Y), new Size(15,15), _ZoneWatcherWatchItem.StrokeColor, 2, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

                                Thread.Sleep(1000);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("C2DFF49A-3C5B-4B19-8058-3CC708F3B8D8");
                    }
                }
            }
        }
    }
}
