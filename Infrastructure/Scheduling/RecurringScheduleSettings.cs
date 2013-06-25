using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    /// <summary>
    /// Provides the settings class for a recurring schedule.
    /// </summary>
    [Serializable]
    public class RecurringScheduleSettings : ScheduleSettings
    {
        private DateRange _dateRange = null;

        public RecurringScheduleSettings() { }

        //public string RawDateRange { get; set; }
        //public string RawDayOfMonth { get; set; }

        /// <summary>
        /// Deserializes settings.
        /// </summary>
        internal RecurringScheduleSettings(Dictionary<string, object> settings) : base(settings) { }
        /// <summary>
        /// Gets/sets the day(s) of the month of the schedule. 
        /// If this property is not set, then the schedule can 
        /// occur on any day of the month.
        /// </summary>
        /// <remarks>
        /// This property supports absolute days (e.g. the "5th", 
        /// "15th", "23rd", etc.), as well as relative days (e.g. 
        /// "FirstMonday", "ThirdWeekday", etc.) and a combination 
        /// thereof (e.g "AllDays-5th-FirstWeekday" - representing 
        /// every day except for the 5th day of the month and the 
        /// first weekday of the month).
        /// </remarks>

        private DayOfMonth _dayOfMonth;
        public DayOfMonth DayOfMonth
        {
            get
            {
                if (Contains("dayOfMonth"))
                {
                   // DayOfMonth dayOfMonth = DayOfMonth.Empty;
                    _dayOfMonth = DayOfMonth.Parse(GetString("dayOfMonth"));
                }
                return _dayOfMonth;
            }

            set
            {
                if (value != DayOfMonth.Empty)
                {
                    SetValue(new string[] { "dayOfMonth", "daysOfMonth" }, value.ToString());
                }

            }
        }

        /// <summary>
        /// Gets/sets the absolute start and stop dates for the event.
        /// If no stop date is specified, then it is assumed that the  
        /// schedule will never expire.
        /// </summary>



        public DateRange DateRange
        {
            get
            {
                if (_dateRange == null && Contains("dateRange"))
                {
                    string dateRange = GetString("dateRange");

                    if (dateRange != null && dateRange.Length != 0 &&
                        DateRange.TryParse(dateRange, out _dateRange))
                    {
                        _dateRange.Changed += new EventHandler(DateRange_Changed);
                    }
                }

                return _dateRange;
            }

            set
            {
                if (value != DateRange.Empty)
                {
                    SetValue("dateRange", value.ToString("MM/dd/yyyy"));
                }
                else
                {
                    Remove("dateRange");
                }

                _dateRange = value;
            }
        }
        private void DateRange_Changed(object sender, EventArgs e)
        {
            DateRange = _dateRange;
        }
    }
}
