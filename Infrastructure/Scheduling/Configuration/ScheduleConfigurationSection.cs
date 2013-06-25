using System.Configuration;

namespace Infrastructure.Scheduling.Configuration
{
    public class ScheduleConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("schedules", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduleConfigCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ScheduleConfigCollection ScheduleProviders
        {
            get
            {
                ScheduleConfigCollection scheduleCollection =
              (ScheduleConfigCollection)base["schedules"];
                return scheduleCollection;
            }

        }
    }
}
