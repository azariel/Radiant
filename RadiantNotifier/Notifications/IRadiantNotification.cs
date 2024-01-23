using Radiant.Common;
using Radiant.Common.Notifications;

namespace Radiant.Notifier.Notifications
{
    public interface IRadiantNotification
    {
        bool Send(INotificationRequest aNotificationRequest);
    }
}
