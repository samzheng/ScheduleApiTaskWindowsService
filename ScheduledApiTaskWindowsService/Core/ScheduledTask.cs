using System;
using System.Threading;
using ScheduledApiTaskWindowsService.Config;

namespace ScheduledApiTaskWindowsService.Core
{
    /// <summary>
    /// 一个已加载的定时任务，包含配置与运行状态。
    /// </summary>
    public class ScheduledTask
    {
        private int _isRunning;

        public ScheduledTask(ApiTaskElement config)
        {
            Config = config;
        }

        public ApiTaskElement Config { get; private set; }

        public DateTime? LastRun { get; set; }

        /// <summary>判断当前时间任务是否到期</summary>
        public bool IsDue(DateTime now)
        {
            string type = (Config.ScheduleType ?? string.Empty).Trim();

            if (type.Equals("Interval", StringComparison.OrdinalIgnoreCase))
            {
                if (Config.IntervalMinutes <= 0) return false;
                // 服务启动后立即运行一次
                return !LastRun.HasValue || (now - LastRun.Value).TotalMinutes >= Config.IntervalMinutes;
            }

            TimeSpan timeOfDay;
            if (!TimeSpan.TryParse(Config.TimeOfDay, out timeOfDay)) return false;

            if (type.Equals("Daily", StringComparison.OrdinalIgnoreCase))
            {
                DateTime scheduled = now.Date.Add(timeOfDay);
                return now >= scheduled && (!LastRun.HasValue || LastRun.Value < scheduled);
            }

            if (type.Equals("Monthly", StringComparison.OrdinalIgnoreCase))
            {
                if (Config.DayOfMonth < 1 || Config.DayOfMonth > 31) return false;
                // 当月没有该日期时取当月最后一天
                int day = Math.Min(Config.DayOfMonth, DateTime.DaysInMonth(now.Year, now.Month));
                DateTime scheduled = new DateTime(now.Year, now.Month, day).Add(timeOfDay);
                return now >= scheduled && (!LastRun.HasValue || LastRun.Value < scheduled);
            }

            return false;
        }

        /// <summary>尝试占用运行标记，防止同一任务并发执行</summary>
        public bool TryAcquire()
        {
            return Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0;
        }

        public void Release()
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }
    }
}
