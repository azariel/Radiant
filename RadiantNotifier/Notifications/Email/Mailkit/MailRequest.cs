using System.Collections.Generic;

namespace RadiantNotifier.Notifications.Email.Mailkit
{
    public class MailRequest : INotificationRequest
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<byte[]> Attachments { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public List<string> ToAddresses { get; set; }
        public List<string> FromAddresses { get; set; }
    }
}
