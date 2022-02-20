namespace Radiant.Notifier.Notifications.Email.Mailkit
{
    public class MailRequestAttachment
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string MediaSubType { get; set; }
        public string MediaType { get; set; }
    }
}
