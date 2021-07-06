namespace Radiant.Notifier.Notifications.Email.Mailkit
{
    public class EmailServerConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int SleepMsBetweenEmailSent { get; set; } = 30000;// To avoid spam
        public string SmtpPassword { get; set; } = "TODO";
        public int SmtpPort { get; set; } = 587;
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public string SmtpUsername { get; set; } = "frost.qc2@gmail.com";
    }
}
