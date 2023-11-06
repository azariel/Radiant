namespace Radiant.Common.Screen.Watcher.PixelsInZone.WatchItems
{
    internal interface IPixelsInZoneWatchItemByBitmap : IPixelsInZoneWatchItem 
    {
        /// <summary>
        /// Represent the nb of pixels in the bitmap to match the watch item before we consider this to be triggered
        /// </summary>
        public float WatchItemNbPixelsToTrigger { get; set; }
    }
}
