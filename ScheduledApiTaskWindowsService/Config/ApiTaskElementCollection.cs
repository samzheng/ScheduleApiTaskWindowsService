using System.Collections.Generic;
using System.Configuration;

namespace ScheduledApiTaskWindowsService.Config
{
    [ConfigurationCollection(typeof(ApiTaskElement), AddItemName = "task")]
    public class ApiTaskElementCollection : ConfigurationElementCollection, IEnumerable<ApiTaskElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApiTaskElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApiTaskElement)element).Name;
        }

        public new IEnumerator<ApiTaskElement> GetEnumerator()
        {
            foreach (ConfigurationElement element in (System.Collections.IEnumerable)this)
            {
                yield return (ApiTaskElement)element;
            }
        }
    }
}
