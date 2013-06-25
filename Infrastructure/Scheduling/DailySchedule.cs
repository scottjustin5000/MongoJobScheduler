using System;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// A recurring schedule that elapsed once per day.
    /// </summary>

    public class DailySchedule : RecurringSchedule
    {
        private TimeSpan _timeOfDay;
        private DailyScheduleSettings _typedSettings;

        public DailySchedule() { }

        public override void Configure(ScheduleSettings settings, bool preConfigured)
        {

            if (!preConfigured)
            {
                _typedSettings = Upcaster.ToDerived<ScheduleSettings, DailyScheduleSettings>(settings);
                _timeOfDay = _typedSettings.TimeOfDay;
                base.Configure(_typedSettings, true);
            }
            else
            {
                _typedSettings = settings as DailyScheduleSettings;
                _timeOfDay = _typedSettings.TimeOfDay;
                base.Configure(settings, true);
            }

        }

        public new DailyScheduleSettings Settings
        {
            get
            {
                return _typedSettings;
            }
        }

        /// <summary>
        /// Gets/sets the daily time of the schedule. 
        /// </summary>
        public TimeSpan TimeOfDay
        {
            get
            {
                return _timeOfDay;
            }

            set
            {
                _timeOfDay = value;
            }
        }

        /// <summary>
        /// Determines if the daily schedule has elapsed.
        /// </summary>
        public override bool HasElapsed()
        {
            DateTime dateTime = DateTime.Now;
            bool hasElapsed = base.HasElapsed();

            if (hasElapsed)
            {
                DateTime lastElapsed = Settings.LastElapsed;

                if (lastElapsed != DateUtility.ZeroDateTime)
                {
                    hasElapsed = (dateTime >= lastElapsed.AddDays(1));
                }

                if (hasElapsed)
                {
                    hasElapsed = (dateTime.TimeOfDay >= _timeOfDay &&
                                  dateTime.TimeOfDay <= _timeOfDay.Add(TimeSpan.FromMinutes(5)));
                }

            }

            return hasElapsed;
        }
    }
}
