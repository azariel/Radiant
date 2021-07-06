using Radiant.Common.Tasks.Triggers;

namespace Radiant.Notifier.Tasks
{
    public class NotificationsMonitorTask : RadiantTask
    {
        // ********************************************************************
        //                            Protected
        // ********************************************************************
        protected override void TriggerNowImplementation()
        {
            RadiantNotificationsManager.EvaluateNotificationsInStorage();
        }
    }
}
