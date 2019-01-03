//using System;
//using System.Globalization;

//namespace orion.web.Common
//{
//    public interface IWeekService : IRegisterByConvention
//    {
//        WeekDTO Next(int year, int id);
//        WeekDTO Get(DateTime date);
//        WeekDTO Previous(int year, int id);
//        DateTime GetWeekDate(int year, int id, DayOfWeek dayOfWeek);
//    }

//    public class WeekService : IWeekService
//    {
//        private readonly Calendar cal;

//        public WeekService()
//        {
//            this.cal = DateTimeFormatInfo.CurrentInfo.Calendar;
//        }

//        private DateTime GetFirsStartOfWeekDateFor(WeekDTO week)
//        {
//            var date = new DateTime(week.Year - 1, 12, 20);
//            var candidate = Get(date);
//            while (week != candidate)
//            {

//                date = date.AddDays(1);
//                while (date.DayOfWeek != TimeEntries.WeekIdentifier.WEEK_START)
//                {
//                    date = date.AddDays(1);
//                }
//                candidate = Get(date);
//            }
//            return date;
//        }
//        public WeekDTO Next(int year, int id)
//        {
//            var current = new WeekDTO() { Year = year, WeekId = id };
//            var candidate = new WeekDTO() { Year = year, WeekId = id };
//            var date = GetFirsStartOfWeekDateFor(current);
//            while (candidate == current)
//            {
//                date = date.AddDays(1);
//                candidate = Get(date);
//            }
//            return candidate;
//        }

//        public WeekDTO Get(DateTime date)
//        {
//            //always roll the date back to the START_OF_WEEK
//            while (date.DayOfWeek != TimeEntries.WeekIdentifier.WEEK_START)
//            {
//                date = date.AddDays(-1);
//            }
//            var weekId = cal.GetWeekOfYear(date,CalendarWeekRule.FirstFullWeek, DayOfWeek.Saturday);
//            return new WeekDTO()
//            {
//                WeekId = weekId,
//                Year = date.Year
//            };
//        }

//        public DateTime GetWeekDate(int year, int id, DayOfWeek dayOfWeek)
//        {
//            var candidate = GetFirsStartOfWeekDateFor(new WeekDTO() { Year = year, WeekId = id });
//            while(candidate.DayOfWeek != dayOfWeek)
//            {
//                candidate = candidate.AddDays(1);
//            }

//            return candidate;
//        }

//        public WeekDTO Previous(int year, int id)
//        {
//            var current = new WeekDTO() { Year = year, WeekId = id };
//            var candidate = new WeekDTO() { Year = year, WeekId = id };
//            var date = GetFirsStartOfWeekDateFor(current);
//            while (candidate == current)
//            {
//                date = date.AddDays(-1);
//                while (date.DayOfWeek != TimeEntries.WeekIdentifier.WEEK_START)
//                {
//                    date = date.AddDays(-1);
//                }
//                candidate = Get(date);
//            }
//            return candidate;
//        }
//    }
//}
