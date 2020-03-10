namespace ntlm.MonitoringPlan
{
    using System.Threading.Tasks;
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Http;
    using System.Net.Mail;

    /// <summary>
    /// Service to monitor.
    /// </summary>
    public class Service
    {
        public static FtpWebRequest client; 

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
            //throw new NotImplementedException();

            var uri = new Uri(Uri);

            
            switch (uri.Scheme)
            {
                case "https":
                    await IsServicesOKAsync();
                    break;
                case "http":
                    await IsServicesOKAsync();
                    break;
                case "ftp":
                    IsServerFTPActive();
                    break;
                case "icmp":
                    IsServerActive();
                    break;
                default:
                    break;
            }

        }


        public bool IsServerFTPActive()
        {
            WebResponse response;
            try
            {
                client = (FtpWebRequest)WebRequest.Create(Uri);
                client.Credentials = new NetworkCredential(Credentials.UserName, Credentials.Password);
                client.Method = WebRequestMethods.Ftp.ListDirectory;
                response = client.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;

            }
        }

        public bool IsServerActive()
        {
            Ping myPing;
            PingReply reply;
            var uri = new Uri(Uri);
            try
            {
                myPing = new Ping();
                reply = myPing.Send(uri.Host);
                if (reply != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " \n Address : " + reply.Address);
                }
                return true;
            }
            catch (PingException)
            {
                return false;

            }
        }

        public async Task<HttpStatusCode> IsServicesOKAsync()
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(Uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);

            Console.WriteLine(response.EnsureSuccessStatusCode());

            return response.StatusCode;
          
              

        }

        public void SendErrorMail(ServiceDownException exception)
        {
            var txt = new System.Text.StringBuilder(); // variable pour le message d'erreur.
            txt.AppendLine(exception.Message); // retourne le message d'erreur.
            txt.AppendLine("");
            txt.AppendLine(exception.StackTrace); // retourne précisemment l'erreur.

            MailMessage mail = new MailMessage(); // determine la variable de création de l' email.
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"); // determine le smtp client avec sont Adresse serveur smtp.
            mail.From = new MailAddress("ntlm.tech@gmail.com", "ntlm.tech"); // Adresse email source.

            if (exception.Service.MailTo != null)
                foreach (var email in exception.Service.MailTo)
                    mail.To.Add(email);

            mail.Subject = string.Format("ERREUR Problèmes serveurs."); // determine l'objet de l' email.
            mail.Body = txt.ToString(); // écrit le message d'erreur dans le coprs 

            SmtpServer.Port = 587; // port associée au serveur smtp que nous voulons joindre.
            SmtpServer.Credentials = new NetworkCredential(mail.From.Address, "Resistance979091");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail); // envoie l'email avec toute les infos créer dans la variable mail.

        }


    }

}


