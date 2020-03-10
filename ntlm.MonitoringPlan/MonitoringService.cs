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
                service.Sender = service.Sender ?? config.Sender;
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
                service.SendErrorMail(ex);
            }
        }
    }

}
