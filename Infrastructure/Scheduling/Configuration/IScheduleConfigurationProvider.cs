

namespace Infrastructure.Scheduling.Configuration
{
    public interface IScheduleConfigurationProvider
    {
        ScheduleSection GetConfigurations(bool refresh);
    }
}
