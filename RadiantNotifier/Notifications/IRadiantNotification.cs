namespace RadiantNotifier.Notifications
{
    public interface IRadiantNotification
    {
        bool Send(INotificationRequest aNotificationRequest);
    }
}
