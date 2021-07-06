using RadiantNotifier.Notifications.Email.Mailkit;

namespace RadiantNotifier.Configuration
{
    /// <summary>
    /// Notifications configuration for Radiant
    /// </summary>
    public class RadiantNotificationConfig
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public EmailServerConfiguration EmailServer { get; set; } = new();
    }
}

