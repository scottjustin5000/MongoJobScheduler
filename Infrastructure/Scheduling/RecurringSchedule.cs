using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Scheduling
{
    public abstract class RecurringSchedule : ScheduleBase
    {
        private DayOfMonth _dayOfMonth;
        private DateRange _dateRange;

        /// <summary>
        /// Creates a new, empty event settings object.
        /// </summary>
        public RecurringSchedule() { }

        /// <summary>
        /// Configures the schedule with information 
        /// </summary>

        private RecurringScheduleSettings _typedSettings;

        public override void Configure(ScheduleSettings settings, bool preConfigured)
        {
            if (!preConfigured)
            {
                _typedSettings = Upcaster.ToDerived<ScheduleSettings, RecurringScheduleSettings>(settings);
                _dayOfMonth = _typedSettings.DayOfMonth;
                _dateRange = _typedSettings.DateRange;
                base.Configure(_typedSettings, true);
            }
            else
            {
                _typedSettings = settings as RecurringScheduleSettings;
                _dayOfMonth = _typedSettings.DayOfMonth;
                _dateRange = _typedSettings.DateRange;
                base.Configure(settings, true);
            }


        }

        public new RecurringScheduleSettings Settings
        {
            get
            {
                return (RecurringScheduleSettings)base.Settings;
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
                return _dateRange;
            }

            set
            {
                _dateRange = value;
            }
        }

        /// <summary>
        /// Gets/sets the day(s) of the month of the event. If this 
        /// property is not set, then the event can occur on any 
        /// day of the month.
        /// </summary>
        public DayOfMonth DayOfMonth
        {
            get
            {
                return _dayOfMonth;
            }

            set
            {
                _dayOfMonth = value;
            }
        }

        /// <summary>
        /// Determines if the recurring schedule has elapsed.
        /// </summary>
        public override bool HasElapsed()
        {
            DateTime dateTime = DateTime.Now;
            bool hasElapsed = true;

            if (_dateRange != null && _dateRange != DateRange.Empty)
            {
                hasElapsed = _dateRange.Contains(dateTime);
            }

            if (hasElapsed)
            {
                if (_dayOfMonth != DayOfMonth.Empty)
                {
                    hasElapsed = _dayOfMonth.IsMatch(dateTime);
                }
            }

            return hasElapsed;
        }

        /// <summary>
        /// Determines if the recurring schedule has expired.
        /// </summary>
        public override bool HasExpired()
        {
            bool hasExpired = false;

            if (Settings.DateExpired != DateUtility.ZeroDateTime)
            {
                hasExpired = true;
            }
            else
            {
                if (_dateRange != null && _dateRange != DateRange.Empty &&
                    _dateRange.StopDate != DateUtility.ZeroDateTime)
                {
                    hasExpired = DateTime.Now > _dateRange.StopDate;
                }
            }

            return hasExpired;
        }
    }
}
