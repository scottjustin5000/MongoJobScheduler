using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{

   /// <summary>
   /// Specifies a time range. Time ranges are concerned only with times and ignore date values.
   /// </summary>
   public sealed class TimeRange
   {
      /// <summary>
      /// Raised when the time range changes.
      /// </summary>
      public event EventHandler Changed;

      /// <summary>
      /// Provides an empty time range.
      /// </summary>
      public static TimeRange Empty = new TimeRange(DateUtility.ZeroDateTime, DateUtility.ZeroDateTime);

      /// <summary>
      /// Provides a default time range of all day.
      /// </summary>
      public static TimeRange Default = new TimeRange(DateUtility.ZeroDateTime, new DateTime(DateUtility.ZeroDateTime.AddDays(1).Ticks - 1));

      private TimeSpan _startTime;
      private TimeSpan _stopTime;

      /// <summary>
      /// Creates an emtpy time range.
      /// </summary>
      public TimeRange () {}

      /// <summary>
      /// Creates a time range with the specified start time.
      /// </summary>
      /// <param name="startTime">The start time of the time range.</param>
      public TimeRange (TimeSpan startTime) : this(startTime, TimeSpan.Zero) {}

      /// <summary>
      /// Creates a time range with the specified values.
      /// </summary>
      /// <param name="startTime">The start time of the time range.</param>
      /// <param name="stopTime">The stop time of the time range.</param>
      public TimeRange (TimeSpan startTime, TimeSpan stopTime) 
      {
         _startTime = startTime;
         _stopTime = stopTime;
      }

      /// <summary>
      /// Creates a time range with the specified start time.
      /// </summary>
      /// <param name="startTime">The start time of the time range.</param>
      public TimeRange (DateTime startTime) : this(startTime, DateUtility.ZeroDateTime) {}

      /// <summary>
      /// Creates a time range with the specified values.
      /// </summary>
      /// <param name="startTime">The start time of the time range.</param>
      /// <param name="stopTime">The stop time of the time range.</param>
      public TimeRange (DateTime startTime, DateTime stopTime) 
      {
         _startTime = startTime.TimeOfDay;
         _stopTime = stopTime.TimeOfDay;
      }

      /// <summary>
      /// Creates a time range with the specified values.
      /// </summary>
      /// <param name="startTime">The start time of the time range.</param>
      /// <param name="duration">The length time of the time range.</param>
      public TimeRange (DateTime startTime, TimeSpan duration) 
      {
         _startTime = startTime.TimeOfDay;
         Duration = duration;
      }

      /// <summary>
      /// Parses the specified string and returns the parsed date range.
      /// </summary>
      public static TimeRange Parse (string timeRange)
      {
         if (timeRange == null)
         {
            throw new ArgumentNullException("timeRange");
         }

         if (timeRange.Length == 0)
         {
            throw new ArgumentException("Parameter \"timeRange\" must be specified.");
         }

         TimeRange timeRangeOutput = Empty;
         
         if (!TryParse(timeRange, out timeRangeOutput))
         {
            throw new ArgumentException("Could not parse the specified timerange.");
         }

         return timeRangeOutput;
      }

      /// <summary>
      /// Parses the specified string and returns the parsed date range.
      /// </summary>
      public static bool TryParse (string timeRangeString, out TimeRange timeRangeOutput)
      {
         timeRangeOutput = null;
         bool success = false;

         if (timeRangeString != null && timeRangeString.Length != 0)
         {
            string[] timeRangeParts = timeRangeString.Split('-');
            timeRangeOutput = new TimeRange();
            DateTime startDate = DateUtility.ZeroDateTime;
            string firstPart = timeRangeParts[0].Trim();

            if (DateTime.TryParse(firstPart, out startDate))
            {
               timeRangeOutput.StartTime = startDate.TimeOfDay;
               success = true;

               if (timeRangeParts.Length != 2)
               {
                  timeRangeOutput.StopTime = TimeSpan.Zero;
               }
               else
               {
                  DateTime stopDate = DateUtility.ZeroDateTime;

                  if (DateTime.TryParse(timeRangeParts[1].Trim(), out stopDate))
                  {
                     timeRangeOutput.StopTime = stopDate.TimeOfDay;
                     success = true;
                  }
                  else
                  {
                     success = false;
                  }
               }
            }
            else
            {
               TimeSpan startTime = TimeSpan.Zero;

               if (TimeSpan.TryParse(firstPart, out startTime))
               {
                  timeRangeOutput.StartTime = startTime;
                  success = true;

                  if (timeRangeParts.Length != 2)
                  {
                     timeRangeOutput.StopTime = TimeSpan.Zero;
                  }
                  else
                  {
                     TimeSpan stopTime = TimeSpan.Zero;

                     if (TimeSpan.TryParse(timeRangeParts[1].Trim(), out stopTime))
                     {
                        timeRangeOutput.StopTime = stopTime;
                        success = true;
                     }
                     else
                     {
                        success = false;
                     }
                  }
               }
            }
         }

         return success;
      }

      /// <summary>
      /// Gets/sets the start time of the time range.
      /// </summary>
      public TimeSpan StartTime
      {
         get
         {
            return _startTime;
         }

         set
         {
            if (_startTime != value)
            {
               _startTime = value;
               OnChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Gets/sets the stop time of the time range.
      /// </summary>
      public TimeSpan StopTime
      {
         get
         {
            return _stopTime;
         }

         set
         {
            if (_stopTime != value)
            {
               _stopTime = value;
               OnChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Gets/sets the length of the time range. If the start time has been
      /// set, then the stop time is adjusted. If the stop time has been set, 
      /// then the start time is adjusted. If both the start time and stop time 
      /// have been set or neither the start time or stop time have been set, 
      /// then the stop time is adjusted.
      /// </summary>
      public TimeSpan Duration
      {
         get
         {
            TimeSpan duration = TimeSpan.Zero;

            if (_startTime <= _stopTime)
            {
               duration = _stopTime.Subtract(_startTime);
            }

            return duration;
         }

         set
         {
            if (_startTime != TimeSpan.Zero || _stopTime == TimeSpan.Zero)
            {
               StopTime = _startTime.Add(value);
            }
            else
            {
               StartTime = _stopTime.Subtract(value);
            }
         }
      }
  
      /// <summary>
      /// Returns an intersection of the current data range and the specified
      /// date range. If there is no intersection between the date ranges, then
      /// <see cref="TimeRange.Empty"/> is returned.
      /// </summary>
      public TimeRange Intersect (TimeRange timeRange)
      {
         TimeRange intersection = TimeRange.Empty;

         // test for scenario 5 & 6
         if (Compare(_startTime, timeRange.StopTime) != 1 &&
             Compare(_stopTime, timeRange.StartTime) != -1)
         {
            // if we passed the first test, then there is some overlap.
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan stopTime = TimeSpan.Zero;

            if (Compare(_startTime, timeRange.StartTime) != -1)
            {
               startTime = _startTime;
            }
            else
            {
               startTime = timeRange.StartTime;
            }

            if (Compare(_stopTime, timeRange.StopTime) != 1)
            {
               stopTime = _stopTime;
            }
            else
            {
               stopTime = timeRange.StopTime;
            }

            intersection = new TimeRange(startTime, stopTime);
         }

         return intersection;
      }

      /// <summary>
      /// Returns a union of the current date range and the specified
      /// date range. The date ranges do not have to overlap.
      /// </summary>
      public TimeRange Union (TimeRange timeRange)
      {
         TimeSpan startTime = TimeSpan.Zero;
         TimeSpan stopTime = TimeSpan.Zero;

         if (Compare(_startTime, timeRange.StartTime) != 1)
         {
            startTime = _startTime;
         }
         else
         {
            startTime = timeRange.StartTime;
         }

         if (Compare(_stopTime, timeRange.StopTime) != -1)
         {
            stopTime = _stopTime;
         }
         else
         {
            stopTime = timeRange.StopTime;
         }

         return new TimeRange(startTime, stopTime);
      }

      /// <summary>
      /// Returns whether the specified object equals the current <see cref="TimeRange"/>.
      /// </summary>
      public bool Equals (TimeRange compare)
      {
         return (Compare(_startTime, compare.StartTime) == 0 && Compare(_stopTime, compare.StopTime) == 0);
      }

      /// <summary>
      /// Returns whether the specified object equals the current <see cref="TimeRange"/>.
      /// </summary>
      public override bool Equals (object obj)
      {
         bool equals = false;

         if (obj != null && obj is TimeRange)
         {
            equals = Equals((TimeRange)obj);
         }

         return equals;
      }

      /// <summary>
      /// Returns whether the specified time is contained in the current
      /// time range.
      /// </summary>
      public bool Contains (TimeSpan timeSpan)
      {
         bool contains = (Compare(timeSpan, _startTime) != -1);

         if (contains && _stopTime != TimeSpan.Zero)
         {
            contains = (Compare(timeSpan, _stopTime) != 1);
         }

         return contains; 
      }

      /// <summary>
      /// Returns whether the specified time is contained in the current
      /// time range.
      /// </summary>
      public bool Contains (DateTime dateTime)
      {
         return Contains(dateTime.TimeOfDay); 
      }

      /// <summary>
      /// Returns the hash code of the date range.
      /// </summary>
      public override int GetHashCode ()
      {
         return _startTime.GetHashCode() + _stopTime.GetHashCode();
      }

      /// <summary>
      /// Returns a string representation of the time range.
      /// </summary>
      public override string ToString ()
      {
         return ToString("hh:mm:ss tt");
      }

      /// <summary>
      /// Returns a string representation of the time range.
      /// </summary>
      public string ToString (string format)
      {
         string text = null;

         if (_stopTime != TimeSpan.Zero)
         {
            text = DateUtility.ZeroDateTime.Add(_startTime).ToString(format) + " - " + DateUtility.ZeroDateTime.Add(_stopTime).ToString(format);
         }
         else
         {
            text = DateUtility.ZeroDateTime.Add(_startTime).ToString(format);
         }

         return text;
      }

      /// <summary>
      /// Provides an implicit conversion from TimeSpan.
      /// </summary>
      public static implicit operator TimeRange (TimeSpan timeSpan)
      {
         return new TimeRange(timeSpan);
      }

      /// <summary>
      /// Provides an implicit conversion from TimeSpan.
      /// </summary>
      public static implicit operator TimeRange (DateTime dateTime)
      {
         return new TimeRange(dateTime);
      }

      private void OnChanged (EventArgs args)
      {
         if (Changed != null)
         {
            Changed(this, args);
         }
      }

      /// <summary>
      /// Compare to times. Precision is to the second (not the default ticks).
      /// -1 if firstTime is less than secondTime.
      /// +1 if firstTime is greater than secondTime.
      /// 0 if the two times are equal.
      /// </summary>
      private static int Compare (TimeSpan firstTime, TimeSpan secondTime)
      {
         int firstSeconds = firstTime.Seconds + 60 * firstTime.Minutes + 3600 * firstTime.Hours;
         int secondSeconds = secondTime.Seconds + 60 * secondTime.Minutes + 3600 * secondTime.Hours;
         int compare = 0;

         if (firstSeconds < secondSeconds)
         {
            compare = -1;
         }
         else if (firstSeconds > secondSeconds)
         {
            compare = 1;
         }

         return compare;
      }
   }
}
