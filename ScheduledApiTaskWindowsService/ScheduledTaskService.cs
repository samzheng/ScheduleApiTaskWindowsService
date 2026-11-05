using System.Net;
using System.ServiceProcess;
using ScheduledApiTaskWindowsService.Core;

namespace ScheduledApiTaskWindowsService
{
    public class ScheduledTaskService : ServiceBase
    {
        private TaskScheduler _scheduler;

        public ScheduledTaskService()
        {
            ServiceName = "ScheduledApiTaskWindowsService";
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            // 兼容只支持 TLS 1.1 / 1.2 的接口（.NET 4.5 默认协议较低）
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;

            _scheduler = new TaskScheduler();
            _scheduler.Start();
        }

        protected override void OnStop()
        {
            if (_scheduler != null)
            {
                _scheduler.Stop();
                _scheduler = null;
            }
        }
    }
}
