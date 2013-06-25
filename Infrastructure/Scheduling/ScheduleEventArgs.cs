using System;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// Provides the event arguments for elapsed events.
    /// </summary>
    public class ScheduleEventArgs : EventArgs
    {
        private readonly ScheduleBase _schedule;
        private readonly ScheduleSettings _settings;

        /// <summary>
        /// Creates a new instance of type <see cref="ScheduleEventArgs"/>.
        /// </summary>
        public ScheduleEventArgs(ScheduleBase schedule, ScheduleSettings settings)
        {
            _schedule = schedule;
            _settings = settings;
        }

        /// <summary>
        /// Gets the schedule for the event.
        /// </summary>
        public ScheduleBase Schedule
        {
            get
            {
                return _schedule;
            }
        }

        /// <summary>
        /// Gets the settings for the event.
        /// </summary>
        public ScheduleSettings Settings
        {
            get
            {
                return _settings;
            }
        }
    }
}
