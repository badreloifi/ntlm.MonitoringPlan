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

        private string GetFilePath(string fileName)
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
                    Smtp = "smtp.gmail.com",
                    Email = "ntlm.tech@gmail.com",
                    DisplayName = "ntlm.tech",
                    Credentials = new Credentials() {
                        UserName = "ntlm.tech@gmail.com",
                        Password = "Resistance979091"
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
            var cfg = srv.DeserializeConfiguration(GetFilePath("config.json"));

            // Then
            Assert.IsNotNull(cfg);
        }
        

        [TestMethod]
        public async Task IsServerFTPActive()
        {
            //Given
            var srv = new MonitoringService();
            var FTP = new Service()
            {
                Uri = "ftp://80.248.213.227:21",
                Credentials = new Credentials()
                {
                    UserName = @"FTP_W6",
                    Password = @"+m]u<CxGKy<2}p,R"
                },
            };

            //When
            await srv.Test(FTP);

            //Then
            Assert.IsTrue(FTP.IsServerFTPActive());

        }

        [TestMethod]
        public async Task IsServerActive()
        {
            //Given
            var srv = new MonitoringService();
            var ServerActive = new Service()
            {
                Uri = "icmp://80.248.213.227"
            };

            //When
            await srv.Test(ServerActive);

            //Then
            Assert.IsTrue(ServerActive.IsServerActive());

        }

        [TestMethod]
        public async Task IsServicesOKAsync()
        {
            //Given
            var srv = new MonitoringService();
            var ServicesOk = new Service()
            {
                Uri = "http://www.sdpv.com/",
                
            };

            //When
            await srv.Test(ServicesOk);

            //Then
            
        }

    }
}
