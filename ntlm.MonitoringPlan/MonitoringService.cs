using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace ntlm.MonitoringPlan
{
    public class MonitoringService
    {
        public static FtpWebRequest client;
        public MonitoringService(string PathConfig, string PathConfigJson)
        {
            this.PathConfig = PathConfig;
            this.PathConfigJson = PathConfigJson;
        }

        public string PathConfig { get; set; }
        public string PathConfigJson { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> MailTo { get; set; }
        public List<string> Services { get; set; }
        public int Port { get; set; }

        public void SerializationFileConfig()
        {
            //var Path = @".\config.txt";
            var configFile = File.ReadAllText(PathConfig);
            string jsonString = JsonConvert.SerializeObject(configFile, Formatting.Indented);
            File.WriteAllText(PathConfigJson, jsonString);  
        }

        public void  DeSerializationFileConfig()
        {
                var JsonConfigFile = File.ReadAllText(PathConfigJson);
                var jConfig = JsonConvert.DeserializeObject(JsonConfigFile).ToString();
                RootObject Config = JsonConvert.DeserializeObject<RootObject>(jConfig);

                Host = Config.MonitoringService[0].Host;
                UserName = Config.MonitoringService[0].UserName;
                Password = Config.MonitoringService[0].Password;
                MailTo = Config.MonitoringService[0].MailTo;
                Services = Config.MonitoringService[0].Services;
                Port = Config.MonitoringService[0].Port;
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
            catch(WebException)
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
            catch(PingException )
            {
                return false;
                
            }
        }

        public async Task PlanAsync()
        {
            try
            {
                SerializationFileConfig();
                DeSerializationFileConfig();
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
            
  
        public void SendErrorMail(Exception exception)
        {
            var txt = new System.Text.StringBuilder(); // variable pour le message d'erreur.
            txt.AppendLine(exception.Message); // retourne le message d'erreur.
            txt.AppendLine("");
            txt.AppendLine(exception.StackTrace); // retourne précisemment l'erreur.

            MailMessage mail = new MailMessage(); // determine la variable de création de l' email.
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"); // determine le smtp client avec sont Adresse serveur smtp.
            mail.From = new MailAddress("ntlm.tech@gmail.com", "ntlm.tech"); // Adresse email source.
            for (int i = 0; i < MailTo.Count; i++)
            {
                mail.To.Add(MailTo[i]); // Adresse email destination.
            }
            mail.Subject = string.Format("ERREUR Problèmes serveurs."); // determine l'objet de l' email.
            mail.Body = txt.ToString(); // écrit le message d'erreur dans le coprs 

            SmtpServer.Port = 587; // port associée au serveur smtp que nous voulons joindre.
            SmtpServer.Credentials = new System.Net.NetworkCredential(mail.From.Address, "Resistance979091");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail); // envoie l'email avec toute les infos créer dans la variable mail.

        }
    }

    public class RootObject
    {
        public List<MonitoringService> MonitoringService { get; set; }
    }
}
