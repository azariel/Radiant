namespace Radiant.Custom.Games.EveOnline.EveRay.Watch
{
    internal interface IWatchItemByBitmap : IWatchItem 
    {
        /// <summary>
        /// Represent the nb of pixels in the bitmap to match the watch item before we consider this to be triggered
        /// </summary>
        public float WatchItemNbPixelsToTrigger { get; set; }
    }
}
