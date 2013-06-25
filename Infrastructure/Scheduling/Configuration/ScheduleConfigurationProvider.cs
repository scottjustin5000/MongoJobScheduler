
namespace Infrastructure.Scheduling.Configuration
{
    public abstract class ScheduleConfigurationProvider
    {
        public abstract ScheduleSection GetConfigurations();
        public abstract ScheduleSection RefeshConfigurations();
        public abstract bool ScheduleAreStale();
        public abstract void DeleteSchedule(ScheduleBase schedule);

    }
}
