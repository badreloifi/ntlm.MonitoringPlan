using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ntlm.MonitoringPlan.Tests
{
    [TestClass]
    public class UnitTest1
    {
        /*private const string Host = @"80.248.213.227";
        private const string UserName = @"FTP_W6";
        private const string Password = @"+m]u<CxGKy<2}p,R";
        private const string MailTo = @"badreloifi@gmail.com";
        private static  string[] Services = { "http://www.sdpv.com/", "http://lesbelleslettres.com/" };*/


        public MonitoringService srv = new MonitoringService();

        [TestMethod]
        public void SerializationFileConfig()
        {
            srv.SerializationFileConfig();

        }

        [TestMethod]
        public void DeSerializationFileConfig()
        {
            srv.DeSerializationFileConfig();

        }

        [TestMethod]
        public void IsServerFTPActive()
        {
            srv.Host = @"80.248.213.227";
            srv.Port = 21;
            srv.UserName = @"FTP_W6";
            srv.Password = @"+m]u<CxGKy<2}p,R";
            Assert.IsTrue(srv.IsServerFTPActive());
            
        }

        [TestMethod]
        public void IsServerActive()
        {
            srv.Host = @"80.248.213.227";
            Assert.IsTrue(srv.IsServerActive());

        }

        [TestMethod]
        public async Task IsServicesOKAsync()
        {
            List<string> S = new List<string>
            {
                "http://www.sdpv.com/",
                "http://www.lesbelleslettres.com/"
            };
            srv.Services = S;
            await srv.IsServicesOKAsync();
        }

        [TestMethod]
        public async Task Plan()
        {
            await srv.PlanAsync();
        }

        [TestMethod]
        public void SendErrorMail(Exception ex)
        {
            srv.SendErrorMail(ex);
        }


        [TestMethod]
        public async Task SendEmailOnError()
        {
            srv.Host = @"80.248.213.285";
            await srv.PlanAsync();
        }



    }
}
