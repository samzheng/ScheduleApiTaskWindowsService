using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ScheduledApiTaskWindowsService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;

            ServiceInstaller serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = "ScheduledApiTaskWindowsService";
            serviceInstaller.DisplayName = "Scheduled API Task Windows Service";
            serviceInstaller.Description = "按照配置文件中的计划（每天/每月/每隔几分钟）定时调用指定 API 接口，并记录每个任务的独立日志。";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
