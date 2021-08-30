using System;
using System.Collections.Generic;
using System.Threading;
using EveRay.TriggerActions;
using EveRay.Watch;

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

        public static void EvaluateZones(List<ZoneWatcher> aConfigZonesWatcher, Action<ZoneWatcher> aShowZoneAction)
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
                        case WatchItemColor _WatchItemColor:
                            if (_ZoneWatcher.Zone.ContainsColor(_WatchItemColor.Color, _ZoneWatcher.Treshold))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);
                                aShowZoneAction?.Invoke(_ZoneWatcher);

                                Thread.Sleep(1000);
                                _Triggered = true;
                            }

                            break;
                        case WatchItemColors _WatchItemColors:
                            if (_ZoneWatcher.Zone.ContainsColors(_WatchItemColors.Colors, _ZoneWatcher.Treshold))
                            {
                                TriggerAction(_ZoneWatcher.TriggerAction);
                                aShowZoneAction?.Invoke(_ZoneWatcher);

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
