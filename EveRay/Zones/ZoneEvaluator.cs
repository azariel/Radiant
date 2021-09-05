using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using EveRay.TriggerActions;
using EveRay.Watch;
using Color = System.Windows.Media.Color;

namespace EveRay.Zones
{
    public static class ZoneEvaluator
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

        public static void EvaluateZones(List<ZoneWatcher> aConfigZonesWatcher, Action<Point, Size, Color, int, int?> aShowZoneAction)
        {
            foreach (ZoneWatcher _ZoneWatcher in aConfigZonesWatcher)
            {
                bool _Triggered = false;
                foreach (IWatchItem _ZoneWatcherWatchItem in _ZoneWatcher.WatchItems)
                {
                    if (_Triggered)
                        break;

                    switch (_ZoneWatcherWatchItem)
                    {
                        case WatchItemBitmapNoise _WatchItemBitmapNoise:

                            if (_ZoneWatcher.Zone.IsDifferentFromLastEvaluation(_WatchItemBitmapNoise.NoiseTreshold, _WatchItemBitmapNoise.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDisk))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);
                                aShowZoneAction?.Invoke(_ZoneWatcher.Zone.Location, _ZoneWatcher.Zone.Size, _ZoneWatcherWatchItem.StrokeColor, 1, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

                                Thread.Sleep(1000);
                                _Triggered = true;
                            }

                            break;
                        case WatchItemColor _WatchItemColor:
                            if (_ZoneWatcher.Zone.ContainsColor(_WatchItemColor.Color, _WatchItemColor.ColorTreshold, _WatchItemColor.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDisk, out Point? _HitPointLocationOfColor))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);

                                if (_HitPointLocationOfColor.HasValue)
                                    aShowZoneAction?.Invoke(_HitPointLocationOfColor.Value, new Size(533, 18), _ZoneWatcherWatchItem.StrokeColor, 2, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

                                Thread.Sleep(1000);
                                _Triggered = true;
                            }

                            break;
                        case WatchItemColors _WatchItemColors:
                            if (_ZoneWatcher.Zone.ContainsColors(_WatchItemColors.Colors, _WatchItemColors.ColorTreshold, _WatchItemColors.WatchItemNbPixelsToTrigger, _ZoneWatcher.SaveImageOnDisk, out Point? _HitPointLocationOfColors))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);

                                if (_HitPointLocationOfColors.HasValue)
                                    aShowZoneAction?.Invoke(_HitPointLocationOfColors.Value, new Size(533, 18), _ZoneWatcherWatchItem.StrokeColor, 2, _ZoneWatcherWatchItem.MsToShowZoneOnDetection);

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
