using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Provides a pseudo-enumeration of the days of a month that supports 
    /// absolute days (i.e. the "5th", "15th", "23rd", etc.), as well as 
    /// relative days (i.e. "FirstMonday", "ThirdWeekday", etc.). The 
    /// <see cref="Parse"/> method method can convert complex strings, such 
    /// as "AllDays-5th-FirstWeekday" (representing every day except for the
    /// 5th day of the month and the first weekday of the month), to 
    /// <see cref="DayOfMonth"/> values.
    /// </summary>
    public struct DayOfMonth
    {
        #region Private Constants
        private const long EMPTY = 0;

        // unioned values.
        private const long ALL_DAYS = FIRST_SUNDAY + SECOND_SUNDAY + THIRD_SUNDAY + FOURTH_SUNDAY + LAST_SUNDAY +
                                      FIRST_MONDAY + SECOND_MONDAY + THIRD_MONDAY + FOURTH_MONDAY + LAST_MONDAY +
                                      FIRST_TUESDAY + SECOND_TUESDAY + THIRD_TUESDAY + FOURTH_TUESDAY + LAST_TUESDAY +
                                      FIRST_WEDNESDAY + SECOND_WEDNESDAY + THIRD_WEDNESDAY + FOURTH_WEDNESDAY + LAST_WEDNESDAY +
                                      FIRST_THURSDAY + SECOND_THURSDAY + THIRD_THURSDAY + FOURTH_THURSDAY + LAST_THURSDAY +
                                      FIRST_FRIDAY + SECOND_FRIDAY + THIRD_FRIDAY + FOURTH_FRIDAY + LAST_FRIDAY +
                                      FIRST_SATURDAY + SECOND_SATURDAY + THIRD_SATURDAY + FOURTH_SATURDAY + LAST_SATURDAY +
                                      FIRST_WEEKDAY + LAST_WEEKDAY + FIRST_DAY + LAST_DAY;
        private const long ALL_WEEKDAYS = FIRST_MONDAY + SECOND_MONDAY + THIRD_MONDAY + FOURTH_MONDAY + LAST_MONDAY +
                                          FIRST_TUESDAY + SECOND_TUESDAY + THIRD_TUESDAY + FOURTH_TUESDAY + LAST_TUESDAY +
                                          FIRST_WEDNESDAY + SECOND_WEDNESDAY + THIRD_WEDNESDAY + FOURTH_WEDNESDAY + LAST_WEDNESDAY +
                                          FIRST_THURSDAY + SECOND_THURSDAY + THIRD_THURSDAY + FOURTH_THURSDAY + LAST_THURSDAY +
                                          FIRST_FRIDAY + SECOND_FRIDAY + THIRD_FRIDAY + FOURTH_FRIDAY + LAST_FRIDAY;
        private const long ALL_WEEKENDS = FIRST_SUNDAY + SECOND_SUNDAY + THIRD_SUNDAY + FOURTH_SUNDAY + LAST_SUNDAY +
                                          FIRST_SATURDAY + SECOND_SATURDAY + THIRD_SATURDAY + FOURTH_SATURDAY + LAST_SATURDAY;
        private const long ALL_SUNDAYS = FIRST_SUNDAY + SECOND_SUNDAY + THIRD_SUNDAY + FOURTH_SUNDAY + LAST_SUNDAY;
        private const long ALL_MONDAYS = FIRST_MONDAY + SECOND_MONDAY + THIRD_MONDAY + FOURTH_MONDAY + LAST_MONDAY;
        private const long ALL_TUESDAYS = FIRST_TUESDAY + SECOND_TUESDAY + THIRD_TUESDAY + FOURTH_TUESDAY + LAST_TUESDAY;
        private const long ALL_WEDNESDAYS = FIRST_WEDNESDAY + SECOND_WEDNESDAY + THIRD_WEDNESDAY + FOURTH_WEDNESDAY + LAST_WEDNESDAY;
        private const long ALL_THURSDAYS = FIRST_THURSDAY + SECOND_THURSDAY + THIRD_THURSDAY + FOURTH_THURSDAY + LAST_THURSDAY;
        private const long ALL_FRIDAYS = FIRST_FRIDAY + SECOND_FRIDAY + THIRD_FRIDAY + FOURTH_FRIDAY + LAST_FRIDAY;
        private const long ALL_SATURDAYS = FIRST_SATURDAY + SECOND_SATURDAY + THIRD_SATURDAY + FOURTH_SATURDAY + LAST_SATURDAY;

        // standard values.
        private const long FIRST_SUNDAY = 1;
        private const long SECOND_SUNDAY = 2;
        private const long THIRD_SUNDAY = 4;
        private const long FOURTH_SUNDAY = 8;
        private const long LAST_SUNDAY = 16;

        private const long FIRST_MONDAY = 32;
        private const long SECOND_MONDAY = 64;
        private const long THIRD_MONDAY = 128;
        private const long FOURTH_MONDAY = 256;
        private const long LAST_MONDAY = 512;

        private const long FIRST_TUESDAY = 1024;
        private const long SECOND_TUESDAY = 2048;
        private const long THIRD_TUESDAY = 4096;
        private const long FOURTH_TUESDAY = 8192;
        private const long LAST_TUESDAY = 16384;

        private const long FIRST_WEDNESDAY = 32768;
        private const long SECOND_WEDNESDAY = 65536;
        private const long THIRD_WEDNESDAY = 131072;
        private const long FOURTH_WEDNESDAY = 262144;
        private const long LAST_WEDNESDAY = 524288;

        private const long FIRST_THURSDAY = 1048576;
        private const long SECOND_THURSDAY = 2097152;
        private const long THIRD_THURSDAY = 4194304;
        private const long FOURTH_THURSDAY = 8388608;
        private const long LAST_THURSDAY = 16777216;

        private const long FIRST_FRIDAY = 33554432;
        private const long SECOND_FRIDAY = 67108864;
        private const long THIRD_FRIDAY = 134217728;
        private const long FOURTH_FRIDAY = 268435456;
        private const long LAST_FRIDAY = 536870912;

        private const long FIRST_SATURDAY = 1073741824;
        private const long SECOND_SATURDAY = 2147483648;
        private const long THIRD_SATURDAY = 4294967296;
        private const long FOURTH_SATURDAY = 8589934592;
        private const long LAST_SATURDAY = 17179869184;

        // special cases.
        private const long FIRST_DAY = 34359738368;
        private const long LAST_DAY = 68719476736;
        private const long FIRST_WEEKDAY = 137438953472;
        private const long LAST_WEEKDAY = 274877906944;
        private const long FIRST_WEEKEND = 549755813888;
        private const long LAST_WEEKEND = 1099511627776;
        private const long ABSOLUTE_DAY = 2199023255552;

        private const long ABSOLUTE_01 = 1;
        private const long ABSOLUTE_02 = 2;
        private const long ABSOLUTE_03 = 4;
        private const long ABSOLUTE_04 = 8;
        private const long ABSOLUTE_05 = 16;
        private const long ABSOLUTE_06 = 32;
        private const long ABSOLUTE_07 = 64;
        private const long ABSOLUTE_08 = 128;
        private const long ABSOLUTE_09 = 256;
        private const long ABSOLUTE_10 = 512;
        private const long ABSOLUTE_11 = 1024;
        private const long ABSOLUTE_12 = 2048;
        private const long ABSOLUTE_13 = 4096;
        private const long ABSOLUTE_14 = 8192;
        private const long ABSOLUTE_15 = 16384;
        private const long ABSOLUTE_16 = 32768;
        private const long ABSOLUTE_17 = 65536;
        private const long ABSOLUTE_18 = 131072;
        private const long ABSOLUTE_19 = 262144;
        private const long ABSOLUTE_20 = 524288;
        private const long ABSOLUTE_21 = 1048576;
        private const long ABSOLUTE_22 = 2097152;
        private const long ABSOLUTE_23 = 4194304;
        private const long ABSOLUTE_24 = 8388608;
        private const long ABSOLUTE_25 = 16777216;
        private const long ABSOLUTE_26 = 33554432;
        private const long ABSOLUTE_27 = 67108864;
        private const long ABSOLUTE_28 = 134217728;
        private const long ABSOLUTE_29 = 268435456;
        private const long ABSOLUTE_30 = 536870912;
        private const long ABSOLUTE_31 = 1073741824;
        #endregion

        private long _includeValue;
        private long _excludeValue;
        private long _includeAbsolute;
        private long _excludeAbsolute;

        private static Regex _parser = new Regex(@"(\+|\||\-)", RegexOptions.Compiled);

        private DayOfMonth(long include) : this(include, 0) { }
        private DayOfMonth(long include, long exclude)
        {
            _includeValue = include;
            _excludeValue = exclude;
            _includeAbsolute = 0;
            _excludeAbsolute = 0;
        }

        /// <summary>
        /// No month day specified.
        /// </summary>
        public static readonly DayOfMonth Empty = new DayOfMonth(EMPTY);

        /// <summary>
        /// All of the days in a month.
        /// </summary>
        public static readonly DayOfMonth AllDays = new DayOfMonth(ALL_DAYS);

        /// <summary>
        /// All of the weekdays in a month.
        /// </summary>
        public static readonly DayOfMonth AllWeekdays = new DayOfMonth(ALL_WEEKDAYS);

        /// <summary>
        /// All of the weekends in a month.
        /// </summary>
        public static readonly DayOfMonth AllWeekends = new DayOfMonth(ALL_WEEKENDS);

        /// <summary>
        /// All of the Sundays in a month.
        /// </summary>
        public static readonly DayOfMonth AllSundays = new DayOfMonth(ALL_SUNDAYS);

        /// <summary>
        /// All of the Mondays in a month.
        /// </summary>
        public static readonly DayOfMonth AllMondays = new DayOfMonth(ALL_MONDAYS);

        /// <summary>
        /// All of the Tuesdays in a month.
        /// </summary>
        public static readonly DayOfMonth AllTuesdays = new DayOfMonth(ALL_TUESDAYS);

        /// <summary>
        /// All of the Wednesdays in a month.
        /// </summary>
        public static readonly DayOfMonth AllWednesdays = new DayOfMonth(ALL_WEDNESDAYS);

        /// <summary>
        /// All of the Thursdays in a month.
        /// </summary>
        public static readonly DayOfMonth AllThursdays = new DayOfMonth(ALL_THURSDAYS);

        /// <summary>
        /// All of the Fridays in a month.
        /// </summary>
        public static readonly DayOfMonth AllFridays = new DayOfMonth(ALL_FRIDAYS);

        /// <summary>
        /// All of the Saturdays in a month.
        /// </summary>
        public static readonly DayOfMonth AllSaturdays = new DayOfMonth(ALL_SATURDAYS);

        /// <summary>
        /// The first weekday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstWeekday = new DayOfMonth(FIRST_WEEKDAY);

        /// <summary>
        /// The last weekday in a month.
        /// </summary>
        public static readonly DayOfMonth LastWeekday = new DayOfMonth(LAST_WEEKDAY);

        /// <summary>
        /// The first weekend day in a month.
        /// </summary>
        public static readonly DayOfMonth FirstWeekend = new DayOfMonth(FIRST_WEEKEND);

        /// <summary>
        /// The last weekend day in a month.
        /// </summary>
        public static readonly DayOfMonth LastWeekend = new DayOfMonth(LAST_WEEKEND);

        /// <summary>
        /// The first day in a month.
        /// </summary>
        public static readonly DayOfMonth FirstDay = new DayOfMonth(FIRST_DAY);

        /// <summary>
        /// The last day in a month.
        /// </summary>
        public static readonly DayOfMonth LastDay = new DayOfMonth(LAST_DAY);

        /// <summary>
        /// The first Sunday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstSunday = new DayOfMonth(FIRST_SUNDAY);

        /// <summary>
        /// The second Sunday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondSunday = new DayOfMonth(SECOND_SUNDAY);

        /// <summary>
        /// The third Sunday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdSunday = new DayOfMonth(THIRD_SUNDAY);

        /// <summary>
        /// The fourth Sunday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthSunday = new DayOfMonth(FOURTH_SUNDAY);

        /// <summary>
        /// The last Sunday in a month.
        /// </summary>
        public static readonly DayOfMonth LastSunday = new DayOfMonth(LAST_SUNDAY);

        /// <summary>
        /// The first Monday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstMonday = new DayOfMonth(FIRST_MONDAY);

        /// <summary>
        /// The second Monday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondMonday = new DayOfMonth(SECOND_MONDAY);

        /// <summary>
        /// The third Monday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdMonday = new DayOfMonth(THIRD_MONDAY);

        /// <summary>
        /// The fourth Monday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthMonday = new DayOfMonth(FOURTH_MONDAY);

        /// <summary>
        /// The last Monday in a month.
        /// </summary>
        public static readonly DayOfMonth LastMonday = new DayOfMonth(LAST_MONDAY);

        /// <summary>
        /// The first Tuesday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstTuesday = new DayOfMonth(FIRST_TUESDAY);

        /// <summary>
        /// The second Tuesday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondTuesday = new DayOfMonth(SECOND_TUESDAY);

        /// <summary>
        /// The third Tuesday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdTuesday = new DayOfMonth(THIRD_TUESDAY);

        /// <summary>
        /// The fourth Tuesday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthTuesday = new DayOfMonth(FOURTH_TUESDAY);

        /// <summary>
        /// The last Tuesday in a month.
        /// </summary>
        public static readonly DayOfMonth LastTuesday = new DayOfMonth(LAST_TUESDAY);

        /// <summary>
        /// The first Wednesday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstWednesday = new DayOfMonth(FIRST_WEDNESDAY);

        /// <summary>
        /// The second Wednesday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondWednesday = new DayOfMonth(SECOND_WEDNESDAY);

        /// <summary>
        /// The third Wednesday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdWednesday = new DayOfMonth(THIRD_WEDNESDAY);

        /// <summary>
        /// The fourth Wednesday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthWednesday = new DayOfMonth(FOURTH_WEDNESDAY);

        /// <summary>
        /// The last Wednesday in a month.
        /// </summary>
        public static readonly DayOfMonth LastWednesday = new DayOfMonth(LAST_WEDNESDAY);

        /// <summary>
        /// The first Thursday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstThursday = new DayOfMonth(FIRST_THURSDAY);

        /// <summary>
        /// The second Thursday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondThursday = new DayOfMonth(SECOND_THURSDAY);

        /// <summary>
        /// The third Thursday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdThursday = new DayOfMonth(THIRD_THURSDAY);

        /// <summary>
        /// The fourth Thursday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthThursday = new DayOfMonth(FOURTH_THURSDAY);

        /// <summary>
        /// The last Thursday in a month.
        /// </summary>
        public static readonly DayOfMonth LastThursday = new DayOfMonth(LAST_THURSDAY);

        /// <summary>
        /// The first Friday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstFriday = new DayOfMonth(FIRST_FRIDAY);

        /// <summary>
        /// The second Friday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondFriday = new DayOfMonth(SECOND_FRIDAY);

        /// <summary>
        /// The third Friday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdFriday = new DayOfMonth(THIRD_FRIDAY);

        /// <summary>
        /// The fourth Friday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthFriday = new DayOfMonth(FOURTH_FRIDAY);

        /// <summary>
        /// The last Friday in a month.
        /// </summary>
        public static readonly DayOfMonth LastFriday = new DayOfMonth(LAST_FRIDAY);

        /// <summary>
        /// The first Saturday in a month.
        /// </summary>
        public static readonly DayOfMonth FirstSaturday = new DayOfMonth(FIRST_SATURDAY);

        /// <summary>
        /// The second Saturday in a month.
        /// </summary>
        public static readonly DayOfMonth SecondSaturday = new DayOfMonth(SECOND_SATURDAY);

        /// <summary>
        /// The third Saturday in a month.
        /// </summary>
        public static readonly DayOfMonth ThirdSaturday = new DayOfMonth(THIRD_SATURDAY);

        /// <summary>
        /// The fourth Saturday in a month.
        /// </summary>
        public static readonly DayOfMonth FourthSaturday = new DayOfMonth(FOURTH_SATURDAY);

        /// <summary>
        /// The last Saturday in a month.
        /// </summary>
        public static readonly DayOfMonth LastSaturday = new DayOfMonth(LAST_SATURDAY);

        /// <summary>
        /// Creates an absolute day-of-month. Used to specify
        /// the 3rd day of the month, the 5th day of the month,
        /// the 15th day of the month, etc.
        /// </summary>
        public static DayOfMonth Absolute(byte day)
        {
            DayOfMonth dayOfMonth = new DayOfMonth(ABSOLUTE_DAY);
            dayOfMonth._includeAbsolute = (long)Math.Pow(2, day - 1);
            return dayOfMonth;
        }

        private void Append(string operation, string monthDays)
        {
            if (monthDays != null && monthDays.Length != 0)
            {
                DayOfMonth innerValue = Parse(monthDays);

                if (operation != null)
                {
                    switch (operation)
                    {
                        case "+":
                        case "|":
                            {
                                _includeValue |= innerValue._includeValue;
                                _includeAbsolute |= innerValue._includeAbsolute;
                                break;
                            }

                        case "-":
                        case "&~":
                            {
                                _excludeValue |= innerValue._includeValue;
                                _excludeAbsolute |= innerValue._includeAbsolute;
                                break;
                            }
                    }
                }
                else
                {
                    _includeValue = innerValue._includeValue;
                    _includeAbsolute = innerValue._includeAbsolute;
                }
            }
        }

        /// <summary>
        /// Parses the specified string value.
        /// </summary>
        public static DayOfMonth Parse(string value)
        {
            DayOfMonth monthDays = new DayOfMonth(0);

            if (value != null && value.Length != 0)
            {
                MatchCollection matches = _parser.Matches(value);

                if (matches.Count != 0)
                {
                    int startIndex = 0;
                    string lastMatch = null;

                    foreach (Match match in matches)
                    {
                        string innerString = value.Substring(startIndex, match.Index - startIndex);
                        monthDays.Append(lastMatch, innerString);
                        lastMatch = match.Value;
                        startIndex = match.Index + 1;
                    }

                    if (startIndex != value.Length)
                    {
                        string innerString = value.Substring(startIndex, value.Length - startIndex);
                        monthDays.Append(lastMatch, innerString);
                    }
                }
                else
                {
                    string valueUpper = value.ToUpper();

                    switch (valueUpper)
                    {
                        case "EMPTY":
                            {
                                monthDays = Empty;
                                break;
                            }

                        case "ALLDAYS":
                            {
                                monthDays = AllDays;
                                break;
                            }

                        case "ALLWEEKDAYS":
                            {
                                monthDays = AllWeekdays;
                                break;
                            }

                        case "ALLWEEKENDS":
                            {
                                monthDays = AllWeekends;
                                break;
                            }

                        case "ALLSUNDAYS":
                            {
                                monthDays = AllSundays;
                                break;
                            }

                        case "ALLMONDAYS":
                            {
                                monthDays = AllMondays;
                                break;
                            }

                        case "ALLTUESDAYS":
                            {
                                monthDays = AllTuesdays;
                                break;
                            }

                        case "ALLWEDNESDAYS":
                            {
                                monthDays = AllWednesdays;
                                break;
                            }

                        case "ALLTHURSDAYS":
                            {
                                monthDays = AllThursdays;
                                break;
                            }

                        case "ALLFRIDAYS":
                            {
                                monthDays = AllFridays;
                                break;
                            }

                        case "ALLSATURDAYS":
                            {
                                monthDays = AllSaturdays;
                                break;
                            }

                        case "FIRSTDAY":
                            {
                                monthDays = FirstDay;
                                break;
                            }

                        case "FIRSTFRIDAY":
                            {
                                monthDays = FirstFriday;
                                break;
                            }

                        case "FIRSTMONDAY":
                            {
                                monthDays = FirstMonday;
                                break;
                            }

                        case "FIRSTSATURDAY":
                            {
                                monthDays = FirstSaturday;
                                break;
                            }

                        case "FIRSTSUNDAY":
                            {
                                monthDays = FirstSunday;
                                break;
                            }

                        case "FIRSTTHURSDAY":
                            {
                                monthDays = FirstThursday;
                                break;
                            }

                        case "FIRSTTUESDAY":
                            {
                                monthDays = FirstTuesday;
                                break;
                            }

                        case "FIRSTWEDNESDAY":
                            {
                                monthDays = FirstWednesday;
                                break;
                            }

                        case "FIRSTWEEKDAY":
                            {
                                monthDays = FirstWeekday;
                                break;
                            }

                        case "FIRSTWEEKEND":
                            {
                                monthDays = FirstWeekend;
                                break;
                            }

                        case "FOURTHFRIDAY":
                            {
                                monthDays = FourthFriday;
                                break;
                            }

                        case "FOURTHMONDAY":
                            {
                                monthDays = FourthMonday;
                                break;
                            }

                        case "FOURTHSATURDAY":
                            {
                                monthDays = FourthSaturday;
                                break;
                            }

                        case "FOURTHSUNDAY":
                            {
                                monthDays = FourthSunday;
                                break;
                            }

                        case "FOURTHTHURSDAY":
                            {
                                monthDays = FourthThursday;
                                break;
                            }

                        case "FOURTHTUESDAY":
                            {
                                monthDays = FourthTuesday;
                                break;
                            }

                        case "FOURTHWEDNESDAY":
                            {
                                monthDays = FourthWednesday;
                                break;
                            }

                        case "LASTDAY":
                            {
                                monthDays = LastDay;
                                break;
                            }

                        case "LASTFRIDAY":
                            {
                                monthDays = LastFriday;
                                break;
                            }

                        case "LASTMONDAY":
                            {
                                monthDays = LastMonday;
                                break;
                            }

                        case "LASTSATURDAY":
                            {
                                monthDays = LastSaturday;
                                break;
                            }

                        case "LASTSUNDAY":
                            {
                                monthDays = LastSunday;
                                break;
                            }

                        case "LASTTHURSDAY":
                            {
                                monthDays = LastThursday;
                                break;
                            }

                        case "LASTTUESDAY":
                            {
                                monthDays = LastTuesday;
                                break;
                            }

                        case "LASTWEDNESDAY":
                            {
                                monthDays = LastWednesday;
                                break;
                            }

                        case "LASTWEEKDAY":
                            {
                                monthDays = LastWeekday;
                                break;
                            }

                        case "LASTWEEKEND":
                            {
                                monthDays = LastWeekend;
                                break;
                            }

                        case "SECONDFRIDAY":
                            {
                                monthDays = SecondFriday;
                                break;
                            }

                        case "SECONDMONDAY":
                            {
                                monthDays = SecondMonday;
                                break;
                            }

                        case "SECONDSATURDAY":
                            {
                                monthDays = SecondSaturday;
                                break;
                            }

                        case "SECONDSUNDAY":
                            {
                                monthDays = SecondSunday;
                                break;
                            }

                        case "SECONDTHURSDAY":
                            {
                                monthDays = SecondThursday;
                                break;
                            }

                        case "SECONDTUESDAY":
                            {
                                monthDays = SecondTuesday;
                                break;
                            }

                        case "SECONDWEDNESDAY":
                            {
                                monthDays = SecondWednesday;
                                break;
                            }

                        case "THIRDFRIDAY":
                            {
                                monthDays = ThirdFriday;
                                break;
                            }

                        case "THIRDMONDAY":
                            {
                                monthDays = ThirdMonday;
                                break;
                            }

                        case "THIRDSATURDAY":
                            {
                                monthDays = ThirdSaturday;
                                break;
                            }

                        case "THIRDSUNDAY":
                            {
                                monthDays = ThirdSunday;
                                break;
                            }

                        case "THIRDTHURSDAY":
                            {
                                monthDays = ThirdThursday;
                                break;
                            }

                        case "THIRDTUESDAY":
                            {
                                monthDays = ThirdTuesday;
                                break;
                            }

                        case "THIRDWEDNESDAY":
                            {
                                monthDays = ThirdWednesday;
                                break;
                            }

                        default:
                            {
                                if (IsNumeric(value))
                                {
                                    monthDays = Absolute(byte.Parse(value));
                                }
                                else if (valueUpper.EndsWith("ST") || valueUpper.EndsWith("RD") ||
                                         valueUpper.EndsWith("TH") || valueUpper.EndsWith("ND"))
                                {
                                    value = value.Substring(0, value.Length - 2);
                                    monthDays = Absolute(byte.Parse(value));
                                }

                                break;
                            }
                    }
                }
            }

            return monthDays;
        }

        /// <summary>
        /// Gets all of the defined values.
        /// </summary>
        public static DayOfMonth[] GetValues()
        {
            Type type = typeof(DayOfMonth);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            ArrayList monthDays = new ArrayList();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.FieldType == typeof(DayOfMonth))
                {
                    monthDays.Add(field.GetValue(null));
                }
            }

            DayOfMonth[] returnArray = new DayOfMonth[monthDays.Count];
            monthDays.CopyTo(returnArray, 0);
            return returnArray;
        }

        /// <summary>
        /// Gets the names of all of the defined values.
        /// </summary>
        public static string[] GetNames()
        {
            Type type = typeof(DayOfMonth);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            ArrayList names = new ArrayList();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.FieldType == typeof(DayOfMonth))
                {
                    names.Add(field.Name);
                }
            }

            string[] returnArray = new string[names.Count];
            names.CopyTo(returnArray, 0);
            return returnArray;
        }

        /// <summary>
        /// Returns a string representation of the current value.
        /// </summary>
        public override string ToString()
        {
            string text = GetString(_includeValue, "+", _includeAbsolute);
            text += GetString(_excludeValue, "-", _excludeAbsolute);

            if (text != null && text.Length != 0)
            {
                text = text.Substring(1, text.Length - 1);
            }

            return text;
        }
        public List<KeyValuePair<int, DayOfWeek>> ToDowKvp()
        {
            var dic = new List<KeyValuePair<int, DayOfWeek>>();
            if ((_includeValue & FIRST_SUNDAY) == FIRST_SUNDAY)
            {
                // text += operation + "1:Sunday";
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Sunday));
            }

            if ((_includeValue & FIRST_MONDAY) == FIRST_MONDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Monday));
            }

            if ((_includeValue & FIRST_TUESDAY) == FIRST_TUESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Tuesday));
            }

            if ((_includeValue & FIRST_WEDNESDAY) == FIRST_WEDNESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Wednesday));
            }

            if ((_includeValue & FIRST_THURSDAY) == FIRST_THURSDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Thursday));
            }

            if ((_includeValue & FIRST_FRIDAY) == FIRST_FRIDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Friday));
            }

            if ((_includeValue & FIRST_SATURDAY) == FIRST_SATURDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(1, DayOfWeek.Saturday));
            }

            if ((_includeValue & SECOND_SUNDAY) == SECOND_SUNDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Sunday));
            }

            if ((_includeValue & SECOND_MONDAY) == SECOND_MONDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Monday));
            }

            if ((_includeValue & SECOND_TUESDAY) == SECOND_TUESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Tuesday));
            }

            if ((_includeValue & SECOND_WEDNESDAY) == SECOND_WEDNESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Wednesday));
            }

            if ((_includeValue & SECOND_THURSDAY) == SECOND_THURSDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Thursday));
            }

            if ((_includeValue & SECOND_FRIDAY) == SECOND_FRIDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Friday));
            }

            if ((_includeValue & SECOND_SATURDAY) == SECOND_SATURDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(2, DayOfWeek.Saturday));
            }

            if ((_includeValue & THIRD_SUNDAY) == THIRD_SUNDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Sunday));
            }

            if ((_includeValue & THIRD_MONDAY) == THIRD_MONDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Monday));
            }

            if ((_includeValue & THIRD_TUESDAY) == THIRD_TUESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Tuesday));
            }

            if ((_includeValue & THIRD_WEDNESDAY) == THIRD_WEDNESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Wednesday));
            }

            if ((_includeValue & THIRD_THURSDAY) == THIRD_THURSDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Thursday));
            }

            if ((_includeValue & THIRD_FRIDAY) == THIRD_FRIDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Friday));
            }

            if ((_includeValue & THIRD_SATURDAY) == THIRD_SATURDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(3, DayOfWeek.Saturday));
            }

            if ((_includeValue & FOURTH_SUNDAY) == FOURTH_SUNDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Sunday));
            }

            if ((_includeValue & FOURTH_MONDAY) == FOURTH_MONDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Monday));
            }

            if ((_includeValue & FOURTH_TUESDAY) == FOURTH_TUESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Tuesday));
            }

            if ((_includeValue & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Wednesday));
            }

            if ((_includeValue & FOURTH_THURSDAY) == FOURTH_THURSDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Thursday));
            }

            if ((_includeValue & FOURTH_FRIDAY) == FOURTH_FRIDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Friday));
            }

            if ((_includeValue & FOURTH_SATURDAY) == FOURTH_SATURDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(4, DayOfWeek.Saturday));
            }

            if ((_includeValue & LAST_SUNDAY) == LAST_SUNDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Sunday));
            }

            if ((_includeValue & LAST_MONDAY) == LAST_MONDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Monday));
            }

            if ((_includeValue & LAST_TUESDAY) == LAST_TUESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Tuesday));
            }

            if ((_includeValue & LAST_WEDNESDAY) == LAST_WEDNESDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Wednesday));
            }

            if ((_includeValue & LAST_THURSDAY) == LAST_THURSDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Thursday));
            }

            if ((_includeValue & LAST_FRIDAY) == LAST_FRIDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Friday));
            }

            if ((_includeValue & LAST_SATURDAY) == LAST_SATURDAY)
            {
                dic.Add(new KeyValuePair<int, DayOfWeek>(5, DayOfWeek.Saturday));
            }


            return dic;
        }
        private string GetString(long value, string operation, long absolute)
        {
            string text = string.Empty;

            if ((value & ALL_DAYS) == ALL_DAYS)
            {
                text += operation + "AllDays";
            }

            if ((value & ALL_WEEKDAYS) == ALL_WEEKDAYS)
            {
                text += operation + "AllWeekdays";
            }

            if ((value & ALL_WEEKENDS) == ALL_WEEKENDS)
            {
                text += operation + "AllWeekends";
            }

            if ((value & ALL_SUNDAYS) == ALL_SUNDAYS)
            {
                text += operation + "AllSundays";
            }

            if ((value & ALL_MONDAYS) == ALL_MONDAYS)
            {
                text += operation + "AllMondays";
            }

            if ((value & ALL_TUESDAYS) == ALL_TUESDAYS)
            {
                text += operation + "AllTuesdays";
            }

            if ((value & ALL_WEDNESDAYS) == ALL_WEDNESDAYS)
            {
                text += operation + "AllWednesdays";
            }

            if ((value & ALL_THURSDAYS) == ALL_THURSDAYS)
            {
                text += operation + "AllThursdays";
            }

            if ((value & ALL_FRIDAYS) == ALL_FRIDAYS)
            {
                text += operation + "AllFridays";
            }

            if ((value & ALL_SATURDAYS) == ALL_SATURDAYS)
            {
                text += operation + "AllSaturdays";
            }

            if ((value & FIRST_DAY) == FIRST_DAY)
            {
                text += operation + "FirstDay";
            }

            if ((value & FIRST_WEEKDAY) == FIRST_WEEKDAY)
            {
                text += operation + "FirstWeekday";
            }

            if ((value & FIRST_WEEKEND) == FIRST_WEEKEND)
            {
                text += operation + "FirstWeekend";
            }

            if ((value & FIRST_SUNDAY) == FIRST_SUNDAY)
            {
                text += operation + "FirstSunday";
            }

            if ((value & FIRST_MONDAY) == FIRST_MONDAY)
            {
                text += operation + "FirstMonday";
            }

            if ((value & FIRST_TUESDAY) == FIRST_TUESDAY)
            {
                text += operation + "FirstTuesday";
            }

            if ((value & FIRST_WEDNESDAY) == FIRST_WEDNESDAY)
            {
                text += operation + "FirstWednesday";
            }

            if ((value & FIRST_THURSDAY) == FIRST_THURSDAY)
            {
                text += operation + "FirstThursday";
            }

            if ((value & FIRST_FRIDAY) == FIRST_FRIDAY)
            {
                text += operation + "FirstFriday";
            }

            if ((value & FIRST_SATURDAY) == FIRST_SATURDAY)
            {
                text += operation + "FirstSaturday";
            }

            if ((value & SECOND_SUNDAY) == SECOND_SUNDAY)
            {
                text += operation + "SecondSunday";
            }

            if ((value & SECOND_MONDAY) == SECOND_MONDAY)
            {
                text += operation + "SecondMonday";
            }

            if ((value & SECOND_TUESDAY) == SECOND_TUESDAY)
            {
                text += operation + "SecondTuesday";
            }

            if ((value & SECOND_WEDNESDAY) == SECOND_WEDNESDAY)
            {
                text += operation + "SecondWednesday";
            }

            if ((value & SECOND_THURSDAY) == SECOND_THURSDAY)
            {
                text += operation + "SecondThursday";
            }

            if ((value & SECOND_FRIDAY) == SECOND_FRIDAY)
            {
                text += operation + "SecondFriday";
            }

            if ((value & SECOND_SATURDAY) == SECOND_SATURDAY)
            {
                text += operation + "SecondSaturday";
            }

            if ((value & THIRD_SUNDAY) == THIRD_SUNDAY)
            {
                text += operation + "ThirdSunday";
            }

            if ((value & THIRD_MONDAY) == THIRD_MONDAY)
            {
                text += operation + "ThirdMonday";
            }

            if ((value & THIRD_TUESDAY) == THIRD_TUESDAY)
            {
                text += operation + "ThirdTuesday";
            }

            if ((value & THIRD_WEDNESDAY) == THIRD_WEDNESDAY)
            {
                text += operation + "ThirdWednesday";
            }

            if ((value & THIRD_THURSDAY) == THIRD_THURSDAY)
            {
                text += operation + "ThirdThursday";
            }

            if ((value & THIRD_FRIDAY) == THIRD_FRIDAY)
            {
                text += operation + "ThirdFriday";
            }

            if ((value & THIRD_SATURDAY) == THIRD_SATURDAY)
            {
                text += operation + "ThirdSaturday";
            }

            if ((value & FOURTH_SUNDAY) == FOURTH_SUNDAY)
            {
                text += operation + "FourthSunday";
            }

            if ((value & FOURTH_MONDAY) == FOURTH_MONDAY)
            {
                text += operation + "FourthMonday";
            }

            if ((value & FOURTH_TUESDAY) == FOURTH_TUESDAY)
            {
                text += operation + "FourthTuesday";
            }

            if ((value & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY)
            {
                text += operation + "FourthWednesday";
            }

            if ((value & FOURTH_THURSDAY) == FOURTH_THURSDAY)
            {
                text += operation + "FourthThursday";
            }

            if ((value & FOURTH_FRIDAY) == FOURTH_FRIDAY)
            {
                text += operation + "FourthFriday";
            }

            if ((value & FOURTH_SATURDAY) == FOURTH_SATURDAY)
            {
                text += operation + "FourthSaturday";
            }

            if ((value & LAST_SUNDAY) == LAST_SUNDAY)
            {
                text += operation + "LastSunday";
            }

            if ((value & LAST_MONDAY) == LAST_MONDAY)
            {
                text += operation + "LastMonday";
            }

            if ((value & LAST_TUESDAY) == LAST_TUESDAY)
            {
                text += operation + "LastTuesday";
            }

            if ((value & LAST_WEDNESDAY) == LAST_WEDNESDAY)
            {
                text += operation + "LastWednesday";
            }

            if ((value & LAST_THURSDAY) == LAST_THURSDAY)
            {
                text += operation + "LastThursday";
            }

            if ((value & LAST_FRIDAY) == LAST_FRIDAY)
            {
                text += operation + "LastFriday";
            }

            if ((value & LAST_SATURDAY) == LAST_SATURDAY)
            {
                text += operation + "LastSaturday";
            }

            if ((value & LAST_DAY) == LAST_DAY)
            {
                text += operation + "LastDay";
            }

            if ((value & LAST_WEEKDAY) == LAST_WEEKDAY)
            {
                text += operation + "LastWeekday";
            }

            if ((value & LAST_WEEKEND) == LAST_WEEKEND)
            {
                text += operation + "LastWeekend";
            }

            if ((value & ABSOLUTE_DAY) == ABSOLUTE_DAY)
            {
                if ((absolute & ABSOLUTE_01) == ABSOLUTE_01)
                {
                    text += operation + "1st";
                }

                if ((absolute & ABSOLUTE_02) == ABSOLUTE_02)
                {
                    text += operation + "2nd";
                }

                if ((absolute & ABSOLUTE_03) == ABSOLUTE_03)
                {
                    text += operation + "3rd";
                }

                if ((absolute & ABSOLUTE_04) == ABSOLUTE_04)
                {
                    text += operation + "4th";
                }

                if ((absolute & ABSOLUTE_05) == ABSOLUTE_05)
                {
                    text += operation + "5th";
                }

                if ((absolute & ABSOLUTE_06) == ABSOLUTE_06)
                {
                    text += operation + "6th";
                }

                if ((absolute & ABSOLUTE_07) == ABSOLUTE_07)
                {
                    text += operation + "7th";
                }

                if ((absolute & ABSOLUTE_08) == ABSOLUTE_08)
                {
                    text += operation + "8th";
                }

                if ((absolute & ABSOLUTE_09) == ABSOLUTE_09)
                {
                    text += operation + "9th";
                }

                if ((absolute & ABSOLUTE_10) == ABSOLUTE_10)
                {
                    text += operation + "10th";
                }

                if ((absolute & ABSOLUTE_11) == ABSOLUTE_11)
                {
                    text += operation + "11th";
                }

                if ((absolute & ABSOLUTE_12) == ABSOLUTE_12)
                {
                    text += operation + "12th";
                }

                if ((absolute & ABSOLUTE_13) == ABSOLUTE_13)
                {
                    text += operation + "13th";
                }

                if ((absolute & ABSOLUTE_14) == ABSOLUTE_14)
                {
                    text += operation + "14th";
                }

                if ((absolute & ABSOLUTE_15) == ABSOLUTE_15)
                {
                    text += operation + "15th";
                }

                if ((absolute & ABSOLUTE_16) == ABSOLUTE_16)
                {
                    text += operation + "16th";
                }

                if ((absolute & ABSOLUTE_17) == ABSOLUTE_17)
                {
                    text += operation + "17th";
                }

                if ((absolute & ABSOLUTE_18) == ABSOLUTE_18)
                {
                    text += operation + "18th";
                }

                if ((absolute & ABSOLUTE_19) == ABSOLUTE_19)
                {
                    text += operation + "19th";
                }

                if ((absolute & ABSOLUTE_20) == ABSOLUTE_20)
                {
                    text += operation + "20th";
                }

                if ((absolute & ABSOLUTE_21) == ABSOLUTE_21)
                {
                    text += operation + "21st";
                }

                if ((absolute & ABSOLUTE_22) == ABSOLUTE_22)
                {
                    text += operation + "22nd";
                }

                if ((absolute & ABSOLUTE_23) == ABSOLUTE_23)
                {
                    text += operation + "23rd";
                }

                if ((absolute & ABSOLUTE_24) == ABSOLUTE_24)
                {
                    text += operation + "24th";
                }

                if ((absolute & ABSOLUTE_25) == ABSOLUTE_25)
                {
                    text += operation + "25th";
                }

                if ((absolute & ABSOLUTE_26) == ABSOLUTE_26)
                {
                    text += operation + "26th";
                }

                if ((absolute & ABSOLUTE_27) == ABSOLUTE_27)
                {
                    text += operation + "27th";
                }

                if ((absolute & ABSOLUTE_28) == ABSOLUTE_28)
                {
                    text += operation + "28th";
                }

                if ((absolute & ABSOLUTE_29) == ABSOLUTE_29)
                {
                    text += operation + "29th";
                }

                if ((absolute & ABSOLUTE_30) == ABSOLUTE_30)
                {
                    text += operation + "30th";
                }

                if ((absolute & ABSOLUTE_31) == ABSOLUTE_31)
                {
                    text += operation + "31st";
                }
            }

            return text;
        }
        private string GetIncludedString(long value, string operation, long absolute)
        {
            string text = string.Empty;

            if ((value & FIRST_SUNDAY) == FIRST_SUNDAY)
            {
                text += operation + "1:Sunday";
            }

            if ((value & FIRST_MONDAY) == FIRST_MONDAY)
            {
                text += operation + "1:Monday";
            }

            if ((value & FIRST_TUESDAY) == FIRST_TUESDAY)
            {
                text += operation + "1:Tuesday";
            }

            if ((value & FIRST_WEDNESDAY) == FIRST_WEDNESDAY)
            {
                text += operation + "1:Wednesday";
            }

            if ((value & FIRST_THURSDAY) == FIRST_THURSDAY)
            {
                text += operation + "1:Thursday";
            }

            if ((value & FIRST_FRIDAY) == FIRST_FRIDAY)
            {
                text += operation + "1:Friday";
            }

            if ((value & FIRST_SATURDAY) == FIRST_SATURDAY)
            {
                text += operation + "1:Saturday";
            }

            if ((value & SECOND_SUNDAY) == SECOND_SUNDAY)
            {
                text += operation + "2:Sunday";
            }

            if ((value & SECOND_MONDAY) == SECOND_MONDAY)
            {
                text += operation + "2:Monday";
            }

            if ((value & SECOND_TUESDAY) == SECOND_TUESDAY)
            {
                text += operation + "2:Tuesday";
            }

            if ((value & SECOND_WEDNESDAY) == SECOND_WEDNESDAY)
            {
                text += operation + "2:Wednesday";
            }

            if ((value & SECOND_THURSDAY) == SECOND_THURSDAY)
            {
                text += operation + "2:Thursday";
            }

            if ((value & SECOND_FRIDAY) == SECOND_FRIDAY)
            {
                text += operation + "2:Friday";
            }

            if ((value & SECOND_SATURDAY) == SECOND_SATURDAY)
            {
                text += operation + "2:Saturday";
            }

            if ((value & THIRD_SUNDAY) == THIRD_SUNDAY)
            {
                text += operation + "3:Sunday";
            }

            if ((value & THIRD_MONDAY) == THIRD_MONDAY)
            {
                text += operation + "3:Monday";
            }

            if ((value & THIRD_TUESDAY) == THIRD_TUESDAY)
            {
                text += operation + "3:Tuesday";
            }

            if ((value & THIRD_WEDNESDAY) == THIRD_WEDNESDAY)
            {
                text += operation + "3:Wednesday";
            }

            if ((value & THIRD_THURSDAY) == THIRD_THURSDAY)
            {
                text += operation + "3:Thursday";
            }

            if ((value & THIRD_FRIDAY) == THIRD_FRIDAY)
            {
                text += operation + "3:Friday";
            }

            if ((value & THIRD_SATURDAY) == THIRD_SATURDAY)
            {
                text += operation + "3:Saturday";
            }

            if ((value & FOURTH_SUNDAY) == FOURTH_SUNDAY)
            {
                text += operation + "4:Sunday";
            }

            if ((value & FOURTH_MONDAY) == FOURTH_MONDAY)
            {
                text += operation + "4:Monday";
            }

            if ((value & FOURTH_TUESDAY) == FOURTH_TUESDAY)
            {
                text += operation + "4:Tuesday";
            }

            if ((value & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY)
            {
                text += operation + "4:Wednesday";
            }

            if ((value & FOURTH_THURSDAY) == FOURTH_THURSDAY)
            {
                text += operation + "4:Thursday";
            }

            if ((value & FOURTH_FRIDAY) == FOURTH_FRIDAY)
            {
                text += operation + "4:Friday";
            }

            if ((value & FOURTH_SATURDAY) == FOURTH_SATURDAY)
            {
                text += operation + "4:Saturday";
            }

            if ((value & LAST_SUNDAY) == LAST_SUNDAY)
            {
                text += operation + "5:Sunday";
            }

            if ((value & LAST_MONDAY) == LAST_MONDAY)
            {
                text += operation + "LastMonday";
            }

            if ((value & LAST_TUESDAY) == LAST_TUESDAY)
            {
                text += operation + "LastTuesday";
            }

            if ((value & LAST_WEDNESDAY) == LAST_WEDNESDAY)
            {
                text += operation + "LastWednesday";
            }

            if ((value & LAST_THURSDAY) == LAST_THURSDAY)
            {
                text += operation + "LastThursday";
            }

            if ((value & LAST_FRIDAY) == LAST_FRIDAY)
            {
                text += operation + "LastFriday";
            }

            if ((value & LAST_SATURDAY) == LAST_SATURDAY)
            {
                text += operation + "LastSaturday";
            }

            if ((value & LAST_DAY) == LAST_DAY)
            {
                text += operation + "LastDay";
            }

            if ((value & LAST_WEEKDAY) == LAST_WEEKDAY)
            {
                text += operation + "LastWeekday";
            }

            if ((value & LAST_WEEKEND) == LAST_WEEKEND)
            {
                text += operation + "LastWeekend";
            }

            return text;
        }
        private bool IsExact(DateTime value)
        {
            bool isExact = false;

            switch (_includeValue)
            {
                case FIRST_DAY:
                    {
                        isExact = (value.AddDays(-1).Month != value.Month);
                        break;
                    }

                case LAST_DAY:
                    {
                        isExact = (value.AddDays(1).Month != value.Month);
                        break;
                    }

                case FIRST_WEEKDAY:
                    {
                        switch (value.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                {
                                    isExact = (value.Day <= 3);
                                    break;
                                }

                            case DayOfWeek.Tuesday:
                            case DayOfWeek.Wednesday:
                            case DayOfWeek.Thursday:
                            case DayOfWeek.Friday:
                                {
                                    isExact = (value.Day == 1);
                                    break;
                                }
                        }

                        break;
                    }

                case LAST_WEEKDAY:
                    {
                        switch (value.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                            case DayOfWeek.Tuesday:
                            case DayOfWeek.Wednesday:
                            case DayOfWeek.Thursday:
                                {
                                    isExact = (value.AddDays(1).Month != value.Month);
                                    break;
                                }

                            case DayOfWeek.Friday:
                                {
                                    isExact = (value.AddDays(3).Month != value.Month);
                                    break;
                                }
                        }

                        break;
                    }

                case FIRST_WEEKEND:
                    {
                        switch (value.DayOfWeek)
                        {
                            case DayOfWeek.Saturday:
                                {
                                    isExact = (value.Day <= 6);
                                    break;
                                }

                            case DayOfWeek.Sunday:
                                {
                                    isExact = (value.Day == 1);
                                    break;
                                }
                        }

                        break;
                    }

                case LAST_WEEKEND:
                    {
                        switch (value.DayOfWeek)
                        {
                            case DayOfWeek.Saturday:
                                {
                                    isExact = (value.AddDays(1).Month != value.Month);
                                    break;
                                }

                            case DayOfWeek.Sunday:
                                {
                                    isExact = (value.AddDays(5).Month != value.Month);
                                    break;
                                }
                        }

                        break;
                    }

                case FIRST_SUNDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Sunday && value.Day <= 7);
                        break;
                    }

                case FIRST_MONDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Monday && value.Day <= 7);
                        break;
                    }

                case FIRST_TUESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Tuesday && value.Day <= 7);
                        break;
                    }

                case FIRST_WEDNESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Wednesday && value.Day <= 7);
                        break;
                    }

                case FIRST_THURSDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Thursday && value.Day <= 7);
                        break;
                    }

                case FIRST_FRIDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Friday && value.Day <= 7);
                        break;
                    }

                case FIRST_SATURDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Saturday && value.Day <= 7);
                        break;
                    }

                case SECOND_SUNDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Sunday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_MONDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Monday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_TUESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Tuesday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_WEDNESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Wednesday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_THURSDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Thursday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_FRIDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Friday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case SECOND_SATURDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Saturday && value.Day > 7 && value.Day <= 14);
                        break;
                    }

                case THIRD_SUNDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Sunday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_MONDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Monday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_TUESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Tuesday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_WEDNESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Wednesday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_THURSDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Thursday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_FRIDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Friday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case THIRD_SATURDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Saturday && value.Day > 14 && value.Day <= 21);
                        break;
                    }

                case FOURTH_SUNDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Sunday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_MONDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Monday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_TUESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Tuesday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_WEDNESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Wednesday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_THURSDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Thursday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_FRIDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Friday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case FOURTH_SATURDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Saturday && value.Day > 21 && value.Day <= 28);
                        break;
                    }

                case LAST_SUNDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Sunday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_MONDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Monday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_TUESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Tuesday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_WEDNESDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Wednesday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_THURSDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Thursday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_FRIDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Friday && value.AddDays(7).Month != value.Month);
                        break;
                    }

                case LAST_SATURDAY:
                    {
                        isExact = (value.DayOfWeek == DayOfWeek.Saturday && value.AddDays(7).Month != value.Month);
                        break;
                    }
            }

            return isExact;
        }

        /// <summary>
        /// Determines if the current <see cref="DayOfMonth"/> equals
        /// the specified value.
        /// </summary>
        public bool IsMatch(DateTime value)
        {
            bool isMatch = true;

            if (!IsExact(value))
            {
                bool isIncluded = false;
                bool isExcluded = false;

                // if this is the first day of the month...
                if (value.Day == 1)
                {
                    isIncluded = ((_includeValue & FIRST_DAY) == FIRST_DAY);
                    isExcluded = ((_excludeValue & FIRST_DAY) == FIRST_DAY);
                }
                // if this is the last day of the month...
                else if (value.AddDays(1).Month != value.Month)
                {
                    isIncluded = ((_includeValue & LAST_DAY) == LAST_DAY);
                    isExcluded = ((_excludeValue & LAST_DAY) == LAST_DAY);
                }

                switch (value.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-3).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_MONDAY) == FIRST_MONDAY) || ((_includeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_MONDAY) == FIRST_MONDAY) || ((_excludeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_MONDAY) == FIRST_MONDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_MONDAY) == FIRST_MONDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_MONDAY) == SECOND_MONDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_MONDAY) == SECOND_MONDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_MONDAY) == THIRD_MONDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_MONDAY) == THIRD_MONDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & LAST_MONDAY) == LAST_MONDAY) || ((_includeValue & FOURTH_MONDAY) == FOURTH_MONDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & LAST_MONDAY) == LAST_MONDAY) || ((_excludeValue & FOURTH_MONDAY) == FOURTH_MONDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_MONDAY) == FOURTH_MONDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_MONDAY) == FOURTH_MONDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_MONDAY) == LAST_MONDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_MONDAY) == LAST_MONDAY));
                            }

                            if (value.AddDays(1).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                            }

                            break;
                        }

                    case DayOfWeek.Tuesday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-1).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_TUESDAY) == FIRST_TUESDAY) || ((_includeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_TUESDAY) == FIRST_TUESDAY) || ((_excludeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_TUESDAY) == FIRST_TUESDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_TUESDAY) == FIRST_TUESDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_TUESDAY) == SECOND_TUESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_TUESDAY) == SECOND_TUESDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_TUESDAY) == THIRD_TUESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_TUESDAY) == THIRD_TUESDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_TUESDAY) == FOURTH_TUESDAY) || ((_includeValue & LAST_TUESDAY) == LAST_TUESDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_TUESDAY) == FOURTH_TUESDAY) || ((_excludeValue & LAST_TUESDAY) == LAST_TUESDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_TUESDAY) == FOURTH_TUESDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_TUESDAY) == FOURTH_TUESDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_TUESDAY) == LAST_TUESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_TUESDAY) == LAST_TUESDAY));
                            }

                            if (value.AddDays(1).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                            }

                            break;
                        }

                    case DayOfWeek.Wednesday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-1).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_WEDNESDAY) == FIRST_WEDNESDAY) || ((_includeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_WEDNESDAY) == FIRST_WEDNESDAY) || ((_excludeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_WEDNESDAY) == FIRST_WEDNESDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_WEDNESDAY) == FIRST_WEDNESDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_WEDNESDAY) == SECOND_WEDNESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_WEDNESDAY) == SECOND_WEDNESDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_WEDNESDAY) == THIRD_WEDNESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_WEDNESDAY) == THIRD_WEDNESDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY) || ((_includeValue & LAST_WEDNESDAY) == LAST_WEDNESDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY) || ((_excludeValue & LAST_WEDNESDAY) == LAST_WEDNESDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_WEDNESDAY) == FOURTH_WEDNESDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEDNESDAY) == LAST_WEDNESDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEDNESDAY) == LAST_WEDNESDAY));
                            }

                            if (value.AddDays(1).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                            }

                            break;
                        }

                    case DayOfWeek.Thursday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-1).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_THURSDAY) == FIRST_THURSDAY) || ((_includeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_THURSDAY) == FIRST_THURSDAY) || ((_excludeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_THURSDAY) == FIRST_THURSDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_THURSDAY) == FIRST_THURSDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_THURSDAY) == SECOND_THURSDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_THURSDAY) == SECOND_THURSDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_THURSDAY) == THIRD_THURSDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_THURSDAY) == THIRD_THURSDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_THURSDAY) == FOURTH_THURSDAY) || ((_includeValue & LAST_THURSDAY) == LAST_THURSDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_THURSDAY) == FOURTH_THURSDAY) || ((_excludeValue & LAST_THURSDAY) == LAST_THURSDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_THURSDAY) == FOURTH_THURSDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_THURSDAY) == FOURTH_THURSDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_THURSDAY) == LAST_THURSDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_THURSDAY) == LAST_THURSDAY));
                            }

                            if (value.AddDays(1).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                            }

                            break;
                        }

                    case DayOfWeek.Friday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-1).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_FRIDAY) == FIRST_FRIDAY) || ((_includeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_FRIDAY) == FIRST_FRIDAY) || ((_excludeValue & FIRST_WEEKDAY) == FIRST_WEEKDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_FRIDAY) == FIRST_FRIDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_FRIDAY) == FIRST_FRIDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_FRIDAY) == SECOND_FRIDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_FRIDAY) == SECOND_FRIDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_FRIDAY) == THIRD_FRIDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_FRIDAY) == THIRD_FRIDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_FRIDAY) == FOURTH_FRIDAY) || ((_includeValue & LAST_FRIDAY) == LAST_FRIDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_FRIDAY) == FOURTH_FRIDAY) || ((_excludeValue & LAST_FRIDAY) == LAST_FRIDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_FRIDAY) == FOURTH_FRIDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_FRIDAY) == FOURTH_FRIDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_FRIDAY) == LAST_FRIDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_FRIDAY) == LAST_FRIDAY));
                            }

                            if (value.AddDays(3).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKDAY) == LAST_WEEKDAY));
                            }

                            break;
                        }

                    case DayOfWeek.Saturday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.AddDays(-6).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_SATURDAY) == FIRST_SATURDAY) || ((_includeValue & FIRST_WEEKEND) == FIRST_WEEKEND)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_SATURDAY) == FIRST_SATURDAY) || ((_excludeValue & FIRST_WEEKEND) == FIRST_WEEKEND)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_SATURDAY) == FIRST_SATURDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_SATURDAY) == FIRST_SATURDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_SATURDAY) == SECOND_SATURDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_SATURDAY) == SECOND_SATURDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_SATURDAY) == THIRD_SATURDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_SATURDAY) == THIRD_SATURDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_SATURDAY) == FOURTH_SATURDAY) || ((_includeValue & LAST_SATURDAY) == LAST_SATURDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_SATURDAY) == FOURTH_SATURDAY) || ((_excludeValue & LAST_SATURDAY) == LAST_SATURDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_SATURDAY) == FOURTH_SATURDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_SATURDAY) == FOURTH_SATURDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_SATURDAY) == LAST_SATURDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_SATURDAY) == LAST_SATURDAY));
                            }

                            if (value.AddDays(1).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKEND) == LAST_WEEKEND));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKEND) == LAST_WEEKEND));
                            }

                            break;
                        }

                    case DayOfWeek.Sunday:
                        {
                            if (value.Day <= 7)
                            {
                                if (value.Day == 1)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FIRST_SUNDAY) == FIRST_SUNDAY) || ((_includeValue & FIRST_WEEKEND) == FIRST_WEEKEND)));
                                    isExcluded = (isExcluded || (((_excludeValue & FIRST_SUNDAY) == FIRST_SUNDAY) || ((_excludeValue & FIRST_WEEKEND) == FIRST_WEEKEND)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FIRST_SUNDAY) == FIRST_SUNDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FIRST_SUNDAY) == FIRST_SUNDAY));
                                }
                            }
                            else if (value.Day <= 14)
                            {
                                isIncluded = (isIncluded || ((_includeValue & SECOND_SUNDAY) == SECOND_SUNDAY));
                                isExcluded = (isExcluded || ((_excludeValue & SECOND_SUNDAY) == SECOND_SUNDAY));
                            }
                            else if (value.Day <= 21)
                            {
                                isIncluded = (isIncluded || ((_includeValue & THIRD_SUNDAY) == THIRD_SUNDAY));
                                isExcluded = (isExcluded || ((_excludeValue & THIRD_SUNDAY) == THIRD_SUNDAY));
                            }
                            else if (value.Day <= 28)
                            {
                                if (value.AddDays(7).Month != value.Month)
                                {
                                    isIncluded = (isIncluded || (((_includeValue & FOURTH_SUNDAY) == FOURTH_SUNDAY) || ((_includeValue & LAST_SUNDAY) == LAST_SUNDAY)));
                                    isExcluded = (isExcluded || (((_excludeValue & FOURTH_SUNDAY) == FOURTH_SUNDAY) || ((_excludeValue & LAST_SUNDAY) == LAST_SUNDAY)));
                                }
                                else
                                {
                                    isIncluded = (isIncluded || ((_includeValue & FOURTH_SUNDAY) == FOURTH_SUNDAY));
                                    isExcluded = (isExcluded || ((_excludeValue & FOURTH_SUNDAY) == FOURTH_SUNDAY));
                                }
                            }
                            else
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_SUNDAY) == LAST_SUNDAY));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_SUNDAY) == LAST_SUNDAY));
                            }

                            if (value.AddDays(6).Month != value.Month)
                            {
                                isIncluded = (isIncluded || ((_includeValue & LAST_WEEKEND) == LAST_WEEKEND));
                                isExcluded = (isExcluded || ((_excludeValue & LAST_WEEKEND) == LAST_WEEKEND));
                            }

                            break;
                        }
                }

                if (!isIncluded && ((_includeValue & ABSOLUTE_DAY) == ABSOLUTE_DAY))
                {
                    long compare = (long)Math.Pow(2, value.Day - 1);
                    isIncluded = ((_includeAbsolute & compare) == compare);
                }

                if (isIncluded && !isExcluded && ((_excludeValue & ABSOLUTE_DAY) == ABSOLUTE_DAY))
                {
                    long compare = (long)Math.Pow(2, value.Day - 1);
                    isExcluded = ((_excludeAbsolute & compare) == compare);
                }

                isMatch = (isIncluded && !isExcluded);
            }

            return isMatch;
        }

        /// <summary>
        /// Determines if the current <see cref="DayOfMonth"/> equals
        /// the specified value. Values are considered equivalent if
        /// any intersection occurs between the two values.
        /// </summary>
        public override bool Equals(object obj)
        {
            bool equals = false;

            if (obj != null)
            {
                if (obj is DayOfMonth)
                {
                    equals = (_includeValue == ((DayOfMonth)obj)._includeValue);
                }
                //            else if (obj is DateTime)
                //            {
                //               equals = (_include == Creates((DateTime)obj)._include);
                //            }
            }

            return equals;
        }

        /// <summary>
        /// Gets the hashcode of the current <see cref="DayOfMonth"/>.
        /// </summary>
        public override int GetHashCode()
        {
            return _includeValue.GetHashCode();
        }

        /// <summary>
        /// Returns an union of two values.
        /// </summary>
        public static DayOfMonth operator |(DayOfMonth first, DayOfMonth second)
        {
            DayOfMonth monthDays = new DayOfMonth(first._includeValue, first._excludeValue);
            monthDays._includeValue |= second._includeValue;
            monthDays._excludeValue |= second._excludeValue;
            return monthDays;
        }

        /// <summary>
        /// Returns an intersection of two values.
        /// </summary>
        public static DayOfMonth operator &(DayOfMonth first, DayOfMonth second)
        {
            DayOfMonth monthDays = new DayOfMonth(first._includeValue, first._excludeValue);
            monthDays._includeValue &= second._includeValue;
            monthDays._excludeValue &= second._excludeValue;
            return monthDays;
        }

        /// <summary>
        /// Returns an union of two values.
        /// </summary>
        public static DayOfMonth operator +(DayOfMonth first, DayOfMonth second)
        {
            DayOfMonth monthDays = new DayOfMonth(first._includeValue, first._excludeValue);
            monthDays._includeValue |= second._includeValue;
            return monthDays;
        }

        /// <summary>
        /// Returns an intersection of two values.
        /// </summary>
        public static DayOfMonth operator -(DayOfMonth first, DayOfMonth second)
        {
            DayOfMonth monthDays = new DayOfMonth(first._includeValue, first._excludeValue);
            monthDays._excludeValue |= second._includeValue;
            return monthDays;
        }

        /// <summary>
        /// Returns the one's complement of the specified value.
        /// </summary>
        public static DayOfMonth operator ~(DayOfMonth value)
        {
            return new DayOfMonth(~value._includeValue);
        }

        /// <summary>
        /// Determines if two values are equal.
        /// </summary>
        public static bool operator ==(DayOfMonth first, DayOfMonth second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Determines if two values are not equal.
        /// </summary>
        public static bool operator !=(DayOfMonth first, DayOfMonth second)
        {
            return !first.Equals(second);
        }
        private static bool IsNumeric(string data)
        {
            var numericExpression = new Regex(@"^[-+]?\d+(\.\d+)?$", RegexOptions.Compiled);
            return numericExpression.IsMatch(data);
        }
    }
}
