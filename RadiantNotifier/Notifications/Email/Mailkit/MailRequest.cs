using System.Collections.Generic;

namespace Radiant.Notifier.Notifications.Email.Mailkit
{
    public class MailRequest : INotificationRequest
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<MailRequestAttachment> Attachments { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public List<string> ToAddresses { get; set; }
        public string EmailFrom { get; set; }
    }
}
