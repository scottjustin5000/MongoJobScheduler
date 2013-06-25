using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;


namespace Infrastructure.Scheduling.Configuration
{
    public class TaskScheduleConfigProvider : IScheduleConfigurationProvider
    {
        public ScheduleSection GetConfigurations(bool refresh)
        {
            var section = new ScheduleSection();
            List<ScheduleSettings> settings = LoadFromConfig();

            section.ScheduleSettings = settings;
            return section;
        }
        private List<ScheduleSettings> LoadFromConfig()
        {
            List<ScheduleSettings> settings = new List<ScheduleSettings>();
            try
            {
                // Get the application configuration file.
                System.Configuration.Configuration config =
                        System.Configuration.ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None) as System.Configuration.Configuration;

                // Read and display the custom section.
                ScheduleConfigurationSection scheduleSection =
                   System.Configuration.ConfigurationManager.GetSection("scheduling") as ScheduleConfigurationSection;

                if (scheduleSection == null)
                {
                    throw new Exception("Failed to load scheduling config.");
                }
                else
                {
                    for (int i = 0; i < scheduleSection.ScheduleProviders.Count; i++)
                    {
                        var prxy = scheduleSection.ScheduleProviders[i];
                        if (!string.IsNullOrEmpty(prxy.Name))
                        {
                            var setting = new ScheduleSettings(GetScheduleSettings(prxy));
                            settings.Add(setting);
                        }
                    }
                }

            }
            catch (ConfigurationErrorsException err)
            {
                throw err;
            }
            return settings;
        }
        private Dictionary<string, object> GetScheduleSettings(ScheduleConfigElement element)
        {
            Dictionary<string, object> vals = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            PropertyInfo[] pi = element.GetType().GetProperties();
            foreach (var p in pi)
            {
                object o = p.GetValue(element, null);
                if (o != null && (!string.IsNullOrEmpty(o.ToString())))
                {
                    vals.Add(p.Name, p.GetValue(element, null));
                }
            }

            return vals;
        }
    }
}
