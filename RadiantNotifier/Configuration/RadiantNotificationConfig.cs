using Radiant.Common.Emails.Mailkit;

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

