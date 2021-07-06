namespace Radiant.Notifier.Notifications
{
    public interface IRadiantNotification
    {
        bool Send(INotificationRequest aNotificationRequest);
    }
}
