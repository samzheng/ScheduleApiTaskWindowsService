using System.Configuration;

namespace ScheduledApiTaskWindowsService.Config
{
    /// <summary>
    /// 单个定时任务的配置项。
    /// scheduleType: Daily（每天 timeOfDay 运行）、Monthly（每月 dayOfMonth 日 timeOfDay 运行）、Interval（每 intervalSeconds 秒运行一次）
    /// </summary>
    public class ApiTaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
        }

        [ConfigurationProperty("method", DefaultValue = "GET")]
        public string Method
        {
            get { return (string)this["method"]; }
        }

        [ConfigurationProperty("postData", DefaultValue = "")]
        public string PostData
        {
            get { return (string)this["postData"]; }
        }

        [ConfigurationProperty("contentType", DefaultValue = "application/json")]
        public string ContentType
        {
            get { return (string)this["contentType"]; }
        }

        [ConfigurationProperty("timeoutSeconds", DefaultValue = 60)]
        public int TimeoutSeconds
        {
            get { return (int)this["timeoutSeconds"]; }
        }

        /// <summary>Daily / Monthly / Interval</summary>
        [ConfigurationProperty("scheduleType", IsRequired = true)]
        public string ScheduleType
        {
            get { return (string)this["scheduleType"]; }
        }

        /// <summary>HH:mm:ss，Daily / Monthly 时使用</summary>
        [ConfigurationProperty("timeOfDay", DefaultValue = "")]
        public string TimeOfDay
        {
            get { return (string)this["timeOfDay"]; }
        }

        /// <summary>1-31，Monthly 时使用；当月没有该日期时取当月最后一天</summary>
        [ConfigurationProperty("dayOfMonth", DefaultValue = 0)]
        public int DayOfMonth
        {
            get { return (int)this["dayOfMonth"]; }
        }

        /// <summary>Interval 时使用，单位秒</summary>
        [ConfigurationProperty("intervalSeconds", DefaultValue = 0)]
        public int IntervalSeconds
        {
            get { return (int)this["intervalSeconds"]; }
        }
    }
}
