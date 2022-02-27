using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.Common
{
    public struct DateTimeWithZone
    {
        public static DateTime UniversalTime => DateTime.UtcNow; 

        public static TimeZoneInfo TimeZone => TimeZoneInfo.GetSystemTimeZones().Single(x =>  x.Id == "America/Detroit");

        public static DateTime EasternStandardTime
        {
            get
            {
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZone);
            }
        }

        public static DateTime ConvertToEST(DateTime utc)
        {
            return TimeZoneInfo.ConvertTime(utc, TimeZone);
        }
    }
}
