namespace ntlm.MonitoringPlan
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mail;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using Newtonsoft.Json;


    /// <summary>
    /// Service to plan monitoring on distant services.
    /// </summary>
    public class MonitoringService
    {
        public static FtpWebRequest client;



        /// <summary>
        /// Deserializes the configuration file.
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public Configuration DeserializeConfiguration(string configFilePath)
        {
            string content = File.ReadAllText(configFilePath);
            var config = JsonConvert.DeserializeObject<Configuration>(content);
            foreach (var service in config.Services)
            {
                service.MailTo = service.MailTo?.Length == 0 ? config.MailTo : service.MailTo;
                service.Sender = service.Sender == null ? config.Sender : service.Sender;
            }
            return config;
        }

        /// <summary>
        /// Tests configured services
        /// </summary>
        /// <returns></returns>
        public async Task Test(string configFilePath)
        {
            var config = DeserializeConfiguration(configFilePath);
            await Test(config.Services);
        }


        /// <summary>
        /// Tests an array of services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public async Task Test(Service[] services)
        {
            foreach (var service in services) await Test(service);
        }

        /// <summary>
        /// Tests a service, sends an email on error.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public async Task Test(Service service)
        {
            try
            {
                await service.Test();
            }
            catch (ServiceDownException ex)
            {
                SendErrorMail(ex);
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

        public bool IsServerActive()
        {
            Ping myPing;
            PingReply reply;
            try
            {
                myPing = new Ping();
                reply = myPing.Send(Host);
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

        public async Task PlanAsync()
        {
            try
            {
                SerializationFileConfig();
                DeserializeConfigFile();
                IsServerActive();
                if (IsServerActive() == false)
                    throw new Exception(string.Format(@"Monitor is Down: {0}", Host));
                IsServerFTPActive();
                if (IsServerFTPActive() == false)
                    throw new Exception("Monitor is Down: FTP.");
            }
            catch (Exception ex)
            {
                SendErrorMail(ex);
            }
            await IsServicesOKAsync();
        }

        public async Task IsServicesOKAsync()
        {
            HttpClient client = new HttpClient();

            for (int i = 0; i < Services.Count; i++)
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(Services[i]);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    Console.WriteLine(response.EnsureSuccessStatusCode());
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    SendErrorMail(e);
                }

            }


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
