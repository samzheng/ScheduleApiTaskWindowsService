using System.Configuration;

namespace ScheduledApiTaskWindowsService.Config
{
    public class TaskSchedulerSection : ConfigurationSection
    {
        [ConfigurationProperty("apiTasks", IsDefaultCollection = false)]
        public ApiTaskElementCollection Tasks
        {
            get { return (ApiTaskElementCollection)this["apiTasks"]; }
        }
    }
}
