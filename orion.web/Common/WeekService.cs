using System;
using System.Globalization;

namespace orion.web.Common
{
    public interface IWeekService : IRegisterByConvention
    {
        WeekDTO Next(int year, int id);
        WeekDTO Get(DateTime date);
        WeekDTO Previous(int year, int id);
        DateTime GetWeekDate(int year, int id, DayOfWeek dayOfWeek);
    }

    public class WeekService : IWeekService
    {
        private readonly Calendar cal;

        public WeekService()
        {
            this.cal = DateTimeFormatInfo.CurrentInfo.Calendar;
        }
        public WeekDTO Next(int year, int id)
        {
            var date = new DateTime(year, 1, 1);
            var next = id + 1;
            var canididate = Get(date);
            while(canididate.WeekId != next)
            {
                date = date.AddDays(1);
                canididate = Get(date);
            }
            return canididate;
        }

        public WeekDTO Get(DateTime date)
        {
            var weekId = cal.GetWeekOfYear(date, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DayOfWeek.Saturday);
            return new WeekDTO()
            {
                WeekId = weekId,
                Year = date.Year
            };
        }

        public DateTime GetWeekDate(int year, int id, DayOfWeek dayOfWeek)
        {
            var date = new DateTime(year, 1, 1);
            var canididate = Get(date);
            while(canididate.WeekId != id || date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);
                canididate = Get(date);
            }
          
            return date;
        }    

        public WeekDTO Previous( int year, int id)
        {
            var date = new DateTime(year, 12,31);
            var next = id - 1;
            var canididate = Get(date);
            while(canididate.WeekId != next)
            {
                date = date.AddDays(-1);
                canididate = Get(date);
            }
            return canididate;
        }
    }
}
