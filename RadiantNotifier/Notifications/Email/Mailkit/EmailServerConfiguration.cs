namespace RadiantNotifier.Notifications.Email.Mailkit
{
    public class EmailServerConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string EmailFrom { get; set; }
        //public string PopPassword { get; set; }
        //public int PopPort { get; set; } = 995;
        //public string PopServer { get; set; }
        //public string PopUsername { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; } = 465;
        public string SmtpServer { get; set; } = "smtp.google.com";
        public string SmtpUsername { get; set; }
    }
}
