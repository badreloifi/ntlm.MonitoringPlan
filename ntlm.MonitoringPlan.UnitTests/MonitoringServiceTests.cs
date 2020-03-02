using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ntlm.MonitoringPlan.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [TestClass]
    public class MonitoringServiceTests
    {
        private const string PathConfig = @"config.json";


        public MonitoringServiceTests()
        {

        }

        [TestMethod]
        public async Task TestGoogle_Ok()
        {
            // Given
            var srv = new MonitoringService();
            var google = new Service()
            {
                Uri = "https://www.google.com"
            };

            // When
            await srv.Test(google);

            // Then
        }

        private string getFilePath(string fileName)
        {
            // This will get the current WORKING directory (i.e. \bin\Debug)
            string workingDirectory = Environment.CurrentDirectory;
            // or: Directory.GetCurrentDirectory() gives the same result

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

            return projectDirectory + "\\" + fileName;
        }

        [TestMethod]
        public async Task Test_ServiceIsDown_MailSent()
        {
            // Given
            var srv = new MonitoringService();
            var down = new DownService();

            // When
            await srv.Test(down);

            // Then : check your email
        }

        /// <summary>
        /// Service that is always down.
        /// </summary>
        public class DownService : Service
        {
            public DownService()
            {
                Uri = "https://www.google.com";
                Sender = new EmailAccount()
                {
                    Email = "ntlm.tech@gmail.com",
                    Credentials = new Credentials() {
                        UserName = "",
                        Password = ""
                    }
                };
                MailTo = new string[] { "badreloifi@gmail.com" };
            }
            public override Task Test()
            {
                throw new ServiceDownException(this, new Exception());
            }

        }


        [TestMethod]
        public void SerializationFileConfig()
        {
            // Given
            var srv = new MonitoringService();

            // When
            var cfg = srv.DeserializeConfiguration(getFilePath("config2.json");

            // Then
            Assert.IsNotNull(cfg);
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
        public void SendErrorMail()
        {
            List<string> M = new List<string>
            {
                "badreloifi@gmail.com",
                "eloifi@et.esiea.fr"
            };
            srv.MailTo = M;
            var action = false;
            try
            {
                if (action == false)
                    throw new Exception("test Mail.");

            }
            catch (Exception ex)
            {
                srv.SendErrorMail(ex);
            }
        }
    }
}
