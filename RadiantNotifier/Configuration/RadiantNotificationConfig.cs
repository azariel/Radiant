using Radiant.Notifier.Notifications.Email.Mailkit;

namespace Radiant.Notifier.Configuration
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

