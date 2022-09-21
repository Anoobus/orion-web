using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public struct DateTimeWithZone
    {
        public static DateTime UniversalTime => DateTime.UtcNow; 

        public static TimeZoneInfo IANA => TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x =>  x.Id == "America/Detroit");
        public static TimeZoneInfo EST => TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == "Eastern Standard Time");
        public static TimeZoneInfo EDT => TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == "Eastern Daylight Time");
        public static TimeZoneInfo TimeZone => ComputeTZ();

        private static TimeZoneInfo ComputeTZ()
        {
            if (IANA != null)
                return IANA;

            if(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EST).IsDaylightSavingTime()
                || TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EDT).IsDaylightSavingTime())
            {
                return EDT;
            }

            return EST;

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
    }
}
