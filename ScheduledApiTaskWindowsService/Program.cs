using System.ServiceProcess;

namespace ScheduledApiTaskWindowsService
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[]
            {
                new ScheduledTaskService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
