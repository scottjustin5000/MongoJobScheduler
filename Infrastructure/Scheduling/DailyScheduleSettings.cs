using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// Provides the settings class for a schedule.
    /// </summary>

    public class DailyScheduleSettings : RecurringScheduleSettings
    {
        /// <summary>
        /// Creates a new, empty schedule settings object.
        /// </summary>
        public DailyScheduleSettings() { }

        /// <summary>
        /// Deserializes settings.
        /// </summary>
        internal DailyScheduleSettings(Dictionary<string, object> settings) : base(settings) { }

        /// <summary>
        /// Gets/sets the time of day the schedule should elapse.
        /// The format of this property in the configuration file
        /// is "hh:mm[:ss] tt" (e.g. "11:15 AM") or "hh:mm:ss"
        /// (e.g. "22:50:00").
        /// </summary>

        // public string RawTimeOfDay { get; set; }
        public TimeSpan TimeOfDay
        {
            get
            {
                DateTime timeOfDay = DateUtility.ZeroDateTime;

                if (Contains("timeOfDay"))
                {
                    DateTime.TryParse(GetString("timeOfDay"), out timeOfDay);
                }

                return timeOfDay.TimeOfDay;
            }

            set
            {
                if (value != TimeSpan.FromMilliseconds(-1))
                {
                    SetValue("timeOfDay", DateUtility.ZeroDateTime.Add(value).ToString("hh:mm:ss tt"));
                }
                else
                {
                    Remove("timeOfDay");
                }
            }
        }
    }
}
