using System;
using System.Collections.Generic;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// Provides the settings class for an event.
    /// </summary>
    [Serializable]
    public class TimerScheduleSettings : RecurringScheduleSettings
    {
        private TimeRange _timeRange = null;

        /// <summary>
        /// Creates a new, empty schedule settings object.
        /// </summary>
        public TimerScheduleSettings() { }

        /// <summary>
        /// Deserializes settings.
        /// </summary>
        internal TimerScheduleSettings(Dictionary<string, object> settings) : base(settings) { }

        /// <summary>
        /// Gets/sets how often the schedule elapses per day. If the 
        /// event is to elapse every hour, then this property will be 
        /// 1 hour. If the schedule is to elapse only once per day, 
        /// then this value does not need to be set.
        /// </summary>

        public TimeSpan Frequency
        {
            get
            {
                TimeSpan frequency = TimeSpan.Zero;

                if (Contains("frequency"))
                {
                    frequency = TimeSpan.Parse(GetString("frequency", TimeSpan.Zero.ToString()));
                }

                return frequency;
            }

            set
            {
                if (value != TimeSpan.Zero)
                {
                    SetValue("frequency", value.ToString());
                }
                else
                {
                    Remove("frequency");
                }
            }
        }

        /// <summary>
        /// Gets/sets the daily time range of the schedule. If no value 
        /// is specified for the daily start time or daily stop time, 
        /// then all day is assumed (12:00:00 AM - 11:59:59 PM).
        /// </summary>
        public TimeRange TimeRange
        {
            get
            {
                if (_timeRange == null && Contains("timeRange"))
                {
                    string timeRange = GetString("timeRange");

                    if (!string.IsNullOrEmpty(timeRange) &&
                        TimeRange.TryParse(timeRange, out _timeRange))
                    {
                        _timeRange.Changed += new EventHandler(TimeRange_Changed);
                    }
                }

                return _timeRange;
            }

            set
            {
                if (value != TimeRange.Empty)
                {
                    SetValue("timeRange", value.ToString("hh:mm:ss tt"));
                }
                else
                {
                    Remove("timeRange");
                }
            }
        }

        private void TimeRange_Changed(object sender, EventArgs e)
        {
            TimeRange = _timeRange;
        }
    }
}
