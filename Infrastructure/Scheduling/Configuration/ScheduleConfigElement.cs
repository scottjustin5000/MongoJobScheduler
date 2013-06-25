using System.Configuration;

namespace Infrastructure.Scheduling.Configuration
{
    public class ScheduleConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
        [ConfigurationProperty("enabled", IsRequired = true)]
        public string Enabled
        {
            get
            {
                return (string)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }
        [ConfigurationProperty("dateRange", IsRequired = false)]
        public string DateRange
        {
            get
            {
                return (string)this["dateRange"];
            }
            set
            {
                this["dateRange"] = value;
            }
        }
        [ConfigurationProperty("daysOfMonth", IsRequired = true)]
        public string DayofMonth
        {
            get
            {
                return (string)this["daysOfMonth"];
            }
            set
            {
                this["daysOfMonth"] = value;
            }
        }
        [ConfigurationProperty("timeOfDay", IsRequired = true)]
        public string TimeOfDay
        {
            get
            {
                return (string)this["timeOfDay"];
            }
            set
            {
                this["timeOfDay"] = value;
            }
        }

        [ConfigurationProperty("task", IsRequired = false)]
        public string Task
        {
            get
            {
                return (string)this["task"];
            }
            set
            {
                this["task"] = value;
            }
        }
        //only used with Timer Schedule
        [ConfigurationProperty("timeRange", IsRequired = false)]
        public string TimeRange
        {
            get
            {
                return (string)this["timeRange"];
            }
            set
            {
                this["timeRange"] = value;
            }
        }
        [ConfigurationProperty("frequency", IsRequired = false)]
        public string Frequency
        {
            get
            {
                return (string)this["frequency"];
            }
            set
            {
                this["frequency"] = value;
            }
        }

    }
}
