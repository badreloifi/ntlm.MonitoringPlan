namespace ntlm.MonitoringPlan
{
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// Service to monitor.
    /// </summary>
    public class Service
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public EmailAccount Sender { get; set; }
        public string[] MailTo { get; set; }
        public Credentials Credentials { get; set; }
        public string Uri { get; set; }
        public int Port { get; set; }

        public virtual async Task Test()
        {
            await Task.Yield();
            throw new NotImplementedException();

            var uri = new Uri(Uri);


            switch (uri.Scheme)
            {
                case "http":

                    break;
                default:   
            }

        }


        public bool IsServerFTPActive()
        {
            WebResponse response;
            try
            {
                client = (FtpWebRequest)WebRequest.Create(string.Format(@"ftp://{0}:{1}", Host, Port));
                client.Credentials = new NetworkCredential(UserName, Password);
                client.Method = WebRequestMethods.Ftp.ListDirectory;
                response = client.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;

            }
        }

    }

}
