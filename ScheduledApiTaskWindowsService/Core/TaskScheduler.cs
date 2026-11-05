using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using ScheduledApiTaskWindowsService.Config;

namespace ScheduledApiTaskWindowsService.Core
{
    /// <summary>
    /// 任务调度器：每秒检查一次各任务是否到期，到期任务放到线程池执行
    /// </summary>
    public class TaskScheduler
    {
        private readonly List<ScheduledTask> _tasks = new List<ScheduledTask>();
        private Timer _timer;

        public void Start()
        {
            LoadTasks();

            TaskLogger.Log("System", string.Format("服务启动，共加载 {0} 个任务", _tasks.Count));

            _timer = new Timer(OnTick, null, 0, 1000);
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            TaskLogger.Log("System", "服务停止");
        }

        private void LoadTasks()
        {
            _tasks.Clear();

            TaskSchedulerSection section = ConfigurationManager.GetSection("taskScheduler") as TaskSchedulerSection;
            if (section == null || section.Tasks == null)
            {
                TaskLogger.Log("System", "未找到 taskScheduler 配置节");
                return;
            }

            foreach (ApiTaskElement element in section.Tasks)
            {
                _tasks.Add(new ScheduledTask(element));
                TaskLogger.Log("System", string.Format("加载任务：{0}，类型：{1}，地址：{2}", element.Name, element.ScheduleType, element.Url));
            }
        }

        private void OnTick(object state)
        {
            DateTime now = DateTime.Now;

            foreach (ScheduledTask task in _tasks)
            {
                if (!task.IsDue(now)) continue;
                if (!task.TryAcquire()) continue; // 上一次还未执行完，跳过本次

                ThreadPool.QueueUserWorkItem(ExecuteTask, task);
            }
        }

        private void ExecuteTask(object state)
        {
            ScheduledTask task = (ScheduledTask)state;
            try
            {
                task.LastRun = DateTime.Now;

                TaskLogger.Log(task.Config.Name, string.Format("开始调用 {0} {1}", task.Config.Method, task.Config.Url));

                int statusCode;
                string responseText;
                bool success = HttpHelper.Call(task, out statusCode, out responseText);

                if (success)
                {
                    TaskLogger.Log(task.Config.Name, string.Format("调用成功，状态码：{0}，返回内容：{1}", statusCode, responseText));
                }
                else
                {
                    TaskLogger.Log(task.Config.Name, string.Format("调用失败，状态码：{0}，返回内容：{1}", statusCode, responseText));
                }
            }
            catch (Exception ex)
            {
                TaskLogger.Log(task.Config.Name, "任务执行异常：" + ex);
            }
            finally
            {
                task.Release();
            }
        }
    }
}
