using System;
using Infrastructure.Scheduling.Configuration;

namespace Infrastructure.Scheduling
{
    public static class ConfigurationManager
    {

        private static ScheduleConfigurationProvider _provider;

        public static ScheduleSection GetSectionItem(string type)
        {
            //TaskScheduleConfigProvider
            Type t = Type.GetType(string.Format("Infrastructure.Scheduling.Configuration.{0}ScheduleConfigProvider", type));
            _provider = Activator.CreateInstance(t) as ScheduleConfigurationProvider;
            return _provider.GetConfigurations();

        }
        public static bool SchedulesStale()
        {
            return _provider.ScheduleAreStale();
        }
        public static void DeleteSchedule(ScheduleBase schedule)
        {
            _provider.DeleteSchedule(schedule);
        }
        public static ScheduleSection RefreshSchedule()
        {
            return _provider.RefeshConfigurations();
        }
    }
}
