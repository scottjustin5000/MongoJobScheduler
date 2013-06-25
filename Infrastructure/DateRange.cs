using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Specifies a date range. Date ranges ignore time values.
    /// </summary>
    public sealed class DateRange
    {
        /// <summary>
        /// Raised when the time range changes.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Represents the empty or zero date range.
        /// </summary>
        public static DateRange Empty = new DateRange(DateUtility.ZeroDateTime, DateUtility.ZeroDateTime);

        /// <summary>
        /// Represents the maximum date range.
        /// </summary>
        public static DateRange Every = new DateRange(DateTime.MinValue, DateTime.MaxValue);

        /// <summary>
        /// 
        /// </summary>
        //[Fx1Bridge]
        //[Obsolete("Use DateRange.Every instead.")]
        //public static DateRange MaxValue = Every;

        private DateTime _startDate;
        private DateTime _stopDate;
        private bool _keepTime;

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        public DateRange() { }

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        /// <param name="startDate">
        /// The start of the date range.
        /// </param>
        public DateRange(DateTime startDate) : this(startDate, DateUtility.ZeroDateTime, false) { }

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        /// <param name="startDate">
        /// The start of the date range.
        /// </param>
        /// <param name="stopDate">
        /// The end of the date range.
        /// </param>
        public DateRange(DateTime startDate, DateTime stopDate) : this(startDate, stopDate, false) { }

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        /// <param name="startDate">
        /// The start of the date range.
        /// </param>
        /// <param name="stopDate">
        /// The end of the date range.
        /// </param>
        /// <param name="includeTime">
        /// True to include the times in the date range. By default, date ranges
        /// do not include time values.
        /// </param>
        public DateRange(DateTime startDate, DateTime stopDate, bool includeTime)
        {
            _keepTime = includeTime;
            StartDate = startDate;
            StopDate = stopDate;
        }

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        /// <param name="startDate">
        /// The start of the date range.
        /// </param>
        /// <param name="duration">
        /// The duration of the date range.
        /// </param>
        public DateRange(DateTime startDate, TimeSpan duration) : this(startDate, duration, false) { }

        /// <summary>
        /// Creates a new instance of type <see cref="DateRange"/>.
        /// </summary>
        /// <param name="startDate">
        /// The start of the date range.
        /// </param>
        /// <param name="duration">
        /// The duration of the date range.
        /// </param>
        /// <param name="includeTime">
        /// True to include the times in the date range. By default, date ranges
        /// do not include time values.
        /// </param>
        public DateRange(DateTime startDate, TimeSpan duration, bool includeTime)
        {
            _keepTime = includeTime;
            StartDate = startDate;
            Duration = duration;
        }

        /// <summary>
        /// Parses the specified string and returns the parsed date range.
        /// </summary>
        public static DateRange Parse(string dateRange)
        {
            if (dateRange == null)
            {
                throw new ArgumentNullException("dateRange");
            }

            if (dateRange.Length == 0)
            {
                throw new ArgumentException("Parameter \"dateRange\" must be specified.");
            }

            string[] dateRangeParts = dateRange.Split('-');
            DateRange value = new DateRange();
            value.StartDate = DateTime.Parse(dateRangeParts[0].Trim());

            if (dateRangeParts.Length != 2)
            {
                value.StopDate = DateTime.MaxValue;
            }
            else
            {
                value.StopDate = DateTime.Parse(dateRangeParts[1].Trim());
            }

            return value;
        }

        /// <summary>
        /// Parses the specified string and returns the parsed date range.
        /// </summary>
        public static bool TryParse(string dateRangeString, out DateRange dateRangeOutput)
        {
            bool success = false;
            dateRangeOutput = null;

            if (dateRangeString != null && dateRangeString.Length != 0)
            {
                string[] dateRangeParts = dateRangeString.Split('-');
                dateRangeOutput = new DateRange();
                DateTime startDate = DateUtility.ZeroDateTime;

                if (DateTime.TryParse(dateRangeParts[0].Trim(), out startDate))
                {
                    dateRangeOutput.StartDate = startDate;
                    success = true;

                    if (dateRangeParts.Length != 2)
                    {
                        dateRangeOutput.StopDate = DateTime.MaxValue;
                    }
                    else
                    {
                        DateTime stopDate = DateUtility.ZeroDateTime;

                        if (DateTime.TryParse(dateRangeParts[1].Trim(), out stopDate))
                        {
                            dateRangeOutput.StopDate = stopDate;
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Gets/sets the start date of the date range.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (!_keepTime)
                {
                    value = value.Subtract(value.TimeOfDay);
                }

                if (_startDate != value)
                {
                    _startDate = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets/sets the stop date of the date range.
        /// </summary>
        public DateTime StopDate
        {
            get
            {
                return _stopDate;
            }

            set
            {
                if (!_keepTime)
                {
                    value = value.Subtract(value.TimeOfDay);
                }

                if (_stopDate != value)
                {
                    _stopDate = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets/sets the length of the date range. If the start date has been
        /// set, then the stop date is adjusted. If the stop date has been set, 
        /// then the start date is adjusted. If both the start date and stop date 
        /// have been set or neither the start date or stop date have been set, 
        /// then the stop date is adjusted.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _stopDate.Subtract(_startDate);
            }

            set
            {
                if (_startDate != DateUtility.ZeroDateTime ||
                    (_startDate == DateUtility.ZeroDateTime &&
                     _stopDate == DateUtility.ZeroDateTime))
                {
                    StopDate = _startDate.Add(value);
                }
                else
                {
                    StartDate = _stopDate.Subtract(value);
                }
            }
        }

        /// <summary>
        /// Returns whether the specified date is contained in the current
        /// date range.
        /// </summary>
        public bool Contains(DateTime dateTime)
        {
            if (_keepTime)
            {
                dateTime = dateTime.Subtract(dateTime.TimeOfDay);
            }

            bool contains = (dateTime >= _startDate);

            if (contains && _stopDate != DateUtility.ZeroDateTime)
            {
                contains = (dateTime <= _stopDate);
            }

            return contains;
        }

        /// <summary>
        /// Returns an intersection of the current data range and the specified
        /// date range. If there is no intersection between the date ranges, then
        /// <see cref="DateRange.Empty"/> is returned.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown if dateRange is null.
        /// </exception>
        public DateRange Intersect(DateRange dateRange)
        {
            // ***************************************************************************
            // scenario 1 
            // ---------------------------------------------------------------------------
            //  DateRange1                  [02/20/1972] ... [04/21/2005] 
            //  DateRange2 [01/30/1905] ..................................... [05/20/2010] 
            // ---------------------------------------------------------------------------
            //  Intersects                  [02/20/1972] ... [04/21/2005]
            // ***************************************************************************

            // ***************************************************************************
            // scenario 2
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] ..................................... [04/21/2005] 
            //  DateRange2                  [07/01/1975] ... [07/31/1978] 
            // ---------------------------------------------------------------------------
            //  Intersects                  [07/01/1975] ... [07/31/1978] 
            // ***************************************************************************

            // ***************************************************************************
            // scenario 3
            // ---------------------------------------------------------------------------
            //  DateRange1                  [02/20/1972] .................... [04/21/2005]  
            //  DateRange2 [01/30/1905] .................... [12/20/1989] 
            // ---------------------------------------------------------------------------
            //  Intersects                 [02/20/1972] ... [12/20/1989] 
            // ***************************************************************************

            // ***************************************************************************
            // scenario 4
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] .................... [04/21/2005] 
            //  DateRange2                  [05/18/1975] .................... [12/20/2007] 
            // ---------------------------------------------------------------------------
            //  Intersects                  [05/18/1975] ... [04/21/2005] 
            // ***************************************************************************

            // ***************************************************************************
            // scenario 5
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] ... [04/21/2005]
            //  DateRange2                  [04/21/2005] ... [12/20/2007]
            // ---------------------------------------------------------------------------
            //  Intersects                  [04/21/2005]
            // ***************************************************************************

            // ***************************************************************************
            // scenario 6
            // ---------------------------------------------------------------------------
            //  DateRange1                  [02/20/1972] ... [12/20/2007]
            //  DateRange2 [04/15/1968] ... [02/20/1972] 
            // ---------------------------------------------------------------------------
            //  Intersects                  [02/20/1972]
            // ***************************************************************************

            // ***************************************************************************
            // scenario 7
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] ... [04/21/2005]
            //  DateRange2 [02/20/1972] .................... [12/20/2007] 
            // ---------------------------------------------------------------------------
            //  Intersects [02/20/1972] ... [04/21/2005]
            // ***************************************************************************

            // ***************************************************************************
            // scenario 8
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] .................... [04/21/2005] 
            //  DateRange2                  [04/12/1975] ... [04/21/2005] 
            // ---------------------------------------------------------------------------
            //  Intersects                  [04/12/1975] ... [04/21/2005]
            // ***************************************************************************

            // ***************************************************************************
            // scenario 9
            // ---------------------------------------------------------------------------
            //  DateRange1 [02/20/1972] ... [04/21/2005] 
            //  DateRange2                                   [06/23/2005] ... [04/21/2007] 
            // ---------------------------------------------------------------------------
            //  Intersects                  
            // ***************************************************************************

            // ***************************************************************************
            // scenario 10
            // ---------------------------------------------------------------------------
            //  DateRange1                                   [02/20/1972] ... [04/21/2005] 
            //  DateRange2 [06/23/1908] ... [04/21/1965] 
            // ---------------------------------------------------------------------------
            //  Intersects                  
            // ***************************************************************************

            if (dateRange == null)
            {
                throw new ArgumentNullException("dateRange");
            }

            DateRange intersection = DateRange.Empty;

            // make sure this is not scenario 9 or 10
            if (_startDate <= dateRange.StopDate &&
                _stopDate >= dateRange.StartDate)
            {
                // if we passed the first test, then there is some overlap.
                DateTime startDate = DateUtility.ZeroDateTime;
                DateTime stopDate = DateUtility.ZeroDateTime;

                if (_startDate >= dateRange.StartDate)
                {
                    startDate = _startDate;
                }
                else
                {
                    startDate = dateRange.StartDate;
                }

                if (_stopDate <= dateRange.StopDate)
                {
                    stopDate = _stopDate;
                }
                else
                {
                    stopDate = dateRange.StopDate;
                }

                intersection = new DateRange(startDate, stopDate);
            }

            return intersection;
        }

        /// <summary>
        /// Returns a union of the current date range and the specified
        /// date range. The date ranges do not have to overlap.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown if dateRange is null.
        /// </exception>
        public DateRange Union(DateRange dateRange)
        {
            if (dateRange == null)
            {
                throw new ArgumentNullException("dateRange");
            }

            DateTime startDate = DateUtility.ZeroDateTime;
            DateTime stopDate = DateUtility.ZeroDateTime;

            if (_startDate <= dateRange.StartDate)
            {
                startDate = _startDate;
            }
            else
            {
                startDate = dateRange.StartDate;
            }

            if (_stopDate >= dateRange.StopDate)
            {
                stopDate = _stopDate;
            }
            else
            {
                stopDate = dateRange.StopDate;
            }

            return new DateRange(startDate, stopDate);
        }

        /// <summary>
        /// Returns whether the specified object equals the current <see cref="DateRange"/>.
        /// </summary>
        public bool Equals(DateRange compare)
        {
            bool equals = false;

            if (compare != null)
            {
                equals = (compare.StartDate == _startDate && compare.StopDate == _stopDate);
            }

            return equals;
        }

        /// <summary>
        /// Returns whether the specified object equals the current <see cref="DateRange"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            bool equals = false;

            if (obj != null && obj is DateRange)
            {
                equals = Equals((DateRange)obj);
            }

            return equals;
        }

        /// <summary>
        /// Returns the hash code of the date range.
        /// </summary>
        public override int GetHashCode()
        {
            return _startDate.GetHashCode() + _stopDate.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the date range.
        /// </summary>
        public override string ToString()
        {
            return ToString("MM/dd/yyyy");
        }

        /// <summary>
        /// Returns a string representation of the date range.
        /// </summary>
        public string ToString(string format)
        {
            string buffer = _startDate.ToString(format);

            if (_stopDate != DateUtility.ZeroDateTime && _stopDate != _startDate)
            {
                buffer += " - " + _stopDate.ToString(format);
            }

            return buffer;
        }

        private void OnChanged(EventArgs args)
        {
            if (Changed != null)
            {
                Changed(this, args);
            }
        }
    }
}
