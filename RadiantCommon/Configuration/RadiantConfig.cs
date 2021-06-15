namespace Radiant.Common.Configuration
{
    /// <summary>
    /// Global configuration for Radiant
    /// </summary>
    public class RadiantConfig
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public RadiantTasksConfig Tasks { get; set; } = new();
    }
}
