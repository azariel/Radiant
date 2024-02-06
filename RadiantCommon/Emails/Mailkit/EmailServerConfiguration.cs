namespace Radiant.Common.Emails.Mailkit
{
    public class EmailServerConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public int SleepMsBetweenEmailSent { get; set; } = 30000;// To avoid spam
        public string SmtpPassword { get; set; } = "INSERT_YOUR_PASSWORD";
        public int SmtpPort { get; set; } = 587;
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public string SmtpUsername { get; set; } = "INSERT_YOUR_GMAIL_EMAIL";
    }
}
