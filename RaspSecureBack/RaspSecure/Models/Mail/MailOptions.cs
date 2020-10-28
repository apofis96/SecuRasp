namespace RaspSecure.Models.Mail
{
    public class MailOptions
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpAddress { get; set; }
        public string MailHeaderName { get; set; }
    }
}
