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
            return GetRelativeWeek(year, id + 1);
        }

        private WeekDTO GetRelativeWeek(int year, int weekId)
        {
            var date = new DateTime(year, 1, 1);
            var canididate = Get(date);
            while (year == date.Year && canididate.WeekId != weekId)
            {
                date = date.AddDays(1);
                canididate = Get(date);
            }
            return canididate;
        }
        public WeekDTO Get(DateTime date)
        {
            var weekId = cal.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Saturday);
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
            while (canididate.WeekId != id || date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);
                canididate = Get(date);
            }

            return date;
        }

        public WeekDTO Previous(int year, int id)
        {
            if (id - 1 == 0)
            {
                var lastOfPreviousYear = new DateTime(year - 1, 12, 31);
                return Get(lastOfPreviousYear);
            }
            else
            {
                return GetRelativeWeek(year, id - 1);
            }

        }
    }
}
