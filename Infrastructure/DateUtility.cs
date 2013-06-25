using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Provides date related helper methods.
    /// </summary>
    public static class DateUtility
    {
        /// <summary>
        /// The zero date time.
        /// </summary>
        public static readonly DateTime ZeroDateTime = new DateTime(0);

        /// <summary>
        /// Convert UNIX date to DateTime
        /// </summary>
        /// <param name="time_t"></param>
        /// <returns></returns>
        public static DateTime FromUnixTime(uint time_t)
        {
            long win32FileTime = 10000000 * (long)time_t + 116444736000000000;
            return DateTime.FromFileTimeUtc(win32FileTime);
        }
    }
}
