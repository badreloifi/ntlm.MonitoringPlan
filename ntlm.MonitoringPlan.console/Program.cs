using System.Threading.Tasks;
namespace ntlm.BackupPlan.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string PathConfig = args[0];
            string PathConfigJson = args[1];
            var srv = new MonitoringPlan.MonitoringService(PathConfig, PathConfigJson);
            var action = srv.PlanAsync();
            Task.WaitAll(action);
        }
    }
}
