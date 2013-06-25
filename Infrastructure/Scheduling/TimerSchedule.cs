using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// A recurring schedule.
    /// </summary>

    public class TimerSchedule : RecurringSchedule
    {
        private TimeSpan _frequency;
        private TimeRange _timeRange;

        /// <summary>
        /// Creates a new, empty event settings object.
        /// </summary>
        public TimerSchedule() { }

        /// <summary>
        /// Configures the schedule with information from the application
        /// configuration file.
        /// </summary>
        private TimerScheduleSettings _typedSettings;
        public override void Configure(ScheduleSettings settings, bool preConfigured)
        {
            if (!preConfigured)
            {
                _typedSettings = Upcaster.ToDerived<ScheduleSettings, TimerScheduleSettings>(settings);
                _frequency = _typedSettings.Frequency;
                _timeRange = _typedSettings.TimeRange;
                base.Configure(_typedSettings, true);
            }
            else
            {
                base.Configure(settings, true);
            }

        }

        ///// <summary>
        ///// Gets the settings for the schedule.
        ///// </summary>
        public new TimerScheduleSettings Settings
        {
            get
            {
                return _typedSettings;
            }
        }

        /// <summary>
        /// Gets/sets the frequency of the schedule. The frequency
        /// of the schedule is how often the schedule elapses.
        /// </summary>
        public TimeSpan Frequency
        {
            get
            {
                return _frequency;
            }

            set
            {
                _frequency = value;
            }
        }

        /// <summary>
        /// Gets/sets the daily time range of the schedule. If the 
        /// schedule should only occur once per day, then the start 
        /// time and the stop time should be the same (or can be a 
        /// single value). If no value is specified for the daily 
        /// start time or daily stop time, then all day is assumed 
        /// (12:00:00 AM - 11:59:59 PM).
        /// </summary>
        public TimeRange TimeRange
        {
            get
            {
                return _timeRange;
            }

            set
            {
                _timeRange = value;
            }
        }

        /// <summary>
        /// Determines if the event has elapsed.
        /// </summary>
        public override bool HasElapsed()
        {
            DateTime dateTime = DateTime.Now;
            bool hasElapsed = base.HasElapsed();

            if (hasElapsed)
            {
                hasElapsed = (_timeRange == null || _timeRange.Contains(dateTime));
            }

            if (hasElapsed)
            {
                DateTime lastElapsed = Settings.LastElapsed;

                if (lastElapsed != DateUtility.ZeroDateTime)
                {
                    if (_frequency != TimeSpan.Zero)
                    {
                        // if the current time is greater than the elapsed 
                        // time plus the frequency, then the time is okay.
                        hasElapsed = (dateTime >= lastElapsed.Add(_frequency));
                    }
                    else
                    {
                        // if no value is specified for the frequency, then 
                        // this must be a once-per-day event.
                        hasElapsed = (dateTime >= lastElapsed.AddDays(1));
                    }
                }
                else
                {
                    if (_frequency != TimeSpan.Zero)
                    {
                        // since the event has not yet elapsed, we need to
                        // go ahead and let the event elapse.
                        hasElapsed = true;
                    }
                    else if (_timeRange != null)
                    {
                        // if no frequency is specified, then this is a once per
                        // day event. We need to make sure the specified time is
                        // within five minutes of the time range's start time.
                        hasElapsed = (dateTime.TimeOfDay >= _timeRange.StartTime &&
                                      dateTime.TimeOfDay <= _timeRange.StartTime.Add(TimeSpan.FromMinutes(5)));
                    }
                    else
                    {
                        // no time range and no frequency specified. We will let
                        // this schedule elapse one time only.
                        hasElapsed = true;
                    }
                }
            }

            return hasElapsed;
        }
    }
}
