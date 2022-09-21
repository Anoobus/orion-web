using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using orion.web.Util;

namespace orion.web.Common
{
    public struct DateTimeWithZone
    {
        public static DateTime UniversalTime => DateTime.UtcNow; 

        public static TimeZoneInfo IANA => TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x =>  x.Id == "America/Detroit");
        public static TimeZoneInfo WINDOWS => TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == "Eastern Standard Time");
        public static TimeZoneInfo TimeZone => ComputeTZ();

        private static TimeZoneInfo ComputeTZ()
        {
            if (IANA != null)
                return IANA;

            if (WINDOWS != null)
                return WINDOWS;

            var debug = TimeZoneInfo.GetSystemTimeZones().Select(x => new { x.Id, offset = x.BaseUtcOffset.ToString() }).ToList();
            throw new InvalidTimeZoneException($"could not find valid eastern timezone, system contains: {debug.Dump()}");
        }

        public static DateTime EasternStandardTime
        {
            get
            {
                
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);
            }
        }

        public static DateTimeOffset EasternStandardTimeOffset
        {
            get
            {
                return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZone);
            }
        }

        public static DateTime ConvertToEST(DateTime utc)
        {
            return TimeZoneInfo.ConvertTime(utc, TimeZone);
        }
        public static DateTime ConvertToEST(DateTimeOffset date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZone).LocalDateTime;
        }

        public static DateTime ConvertToEST(int year, int month, int day)
        {
            var safeOffset = new DateTimeOffset(year, month, day,11,50,50,new TimeSpan());
            return TimeZoneInfo.ConvertTime(safeOffset, TimeZone).LocalDateTime;
        }
    }
}
