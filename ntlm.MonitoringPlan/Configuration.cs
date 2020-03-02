namespace ntlm.MonitoringPlan
{
    public class Configuration
    {
        public Service[] Services { get; set; }
        public EmailAccount Sender { get; set; }
        public string[] MailTo { get; set; }
    }
}
