using System.Threading.Tasks;
namespace ntlm.BackupPlan.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string PathConfig = args[0];
            var srv = new MonitoringPlan.MonitoringService();
            var action = srv.Test(PathConfig);
            Task.WaitAll(action);
        }
    }
}
