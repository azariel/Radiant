namespace Radiant.Custom.Games.EveOnline.EveFight.Configuration
{
    /// <summary>
    /// Contains information on how to replay a specific log for debug purpose or to test the configuration.
    /// </summary>
    public record ReplayLogInfo
    {
        /// <summary>
        /// Enable this feature or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Specific path of log to replay
        /// </summary>
        public string LogFilePath { get; set; }
    }
}
