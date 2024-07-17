using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Orion.Web.Common
{
    public class WeekDTO
    {
        private static readonly ConcurrentDictionary<WeekDTO, bool> PPEStatusByWeek = new ConcurrentDictionary<WeekDTO, bool>();
        private static readonly ConcurrentDictionary<int, WeekDTO> WeeksByWeekId = new ConcurrentDictionary<int, WeekDTO>();
        private static readonly ConcurrentDictionary<WeekDTO, int> WeekIdsByWeek = new ConcurrentDictionary<WeekDTO, int>();
        public static readonly DateTime WeekEpoch = new DateTime(2000, 1, 1);

        public const DayOfWeek WEEKSTART = DayOfWeek.Saturday;
        public const DayOfWeek WEEKEND = DayOfWeek.Friday;

        public DateTime WeekEnd => _weekEnd;
        public DateTime WeekStart => _weekStart;
        private readonly DateTime _weekEnd;
        private readonly DateTime _weekStart;
        public int Year => WeekStart.Year;
        public Lazy<int> WeekId => new Lazy<int>(() => GetWeekId());
        public Lazy<bool> IsPPE => new Lazy<bool>(() => GetPPEStatus());

        private bool GetPPEStatus()
        {
            return WeekId.Value % 2 == 1;
        }

        public WeekDTO(DateTime weekStart)
        {
            if (weekStart.DayOfWeek != WEEKSTART)
            {
                throw new InvalidOperationException($"Week start must be on a {WEEKSTART}");
            }

            _weekStart = weekStart.Date;
            _weekEnd = weekStart.Date;
            while (_weekEnd.DayOfWeek != WEEKEND)
            {
                _weekEnd = _weekEnd.AddDays(1);
            }
        }

        private int GetWeekId()
        {
            return WeekIdsByWeek.GetOrAdd(this, (week) =>
             {
                 var candidate = new WeekDTO(WeekEpoch);
                 var weekId = 0;
                 while (candidate != this)
                 {
                     candidate = candidate.Next();
                     ++weekId;
                 }

                 return weekId;
             });
        }

        public WeekDTO Next()
        {
            return new WeekDTO(WeekEnd.AddDays(1));
        }

        public WeekDTO Previous()
        {
            return new WeekDTO(WeekStart.AddDays(-7));
        }

        public override string ToString()
        {
            return WeekStart.ToShortDateString();
        }

        public string UrlId()
        {
            return $"{WeekStart.Year:0000}.{WeekStart.Month:00}.{WeekStart.Day:00}";
        }

        public static WeekDTO CreateWithWeekContaining(DateTime date)
        {
            while (date.DayOfWeek != WEEKSTART)
            {
                date = date.AddDays(-1);
            }

            return new WeekDTO(date);
        }

        public static WeekDTO CreateWithWeekContaining(DateTimeOffset date)
        {
            return CreateWithWeekContaining(DateTimeWithZone.ConvertToEST(date.UtcDateTime));
        }

        public static WeekDTO CreateForWeekId(int weekId)
        {
            return WeeksByWeekId.GetOrAdd(weekId, (id) =>
             {
                 var candidate = new WeekDTO(WeekEpoch);
                 var candidateWeekId = 0;
                 while (candidateWeekId != weekId)
                 {
                     candidate = candidate.Next();
                     ++candidateWeekId;
                 }

                 return candidate;
             });
        }

        public DateTime GetDateFor(DayOfWeek dayOfWeek)
        {
            var candidateDay = WeekStart;
            while (candidateDay.DayOfWeek != dayOfWeek)
            {
                candidateDay = candidateDay.AddDays(1);
            }

            return candidateDay;
        }

        public static bool operator ==(WeekDTO week, WeekDTO other)
        {
            if (ReferenceEquals(week, null))
            {
                return ReferenceEquals(other, null);
            }

            return week.Equals(other);
        }

        public static bool operator !=(WeekDTO week, WeekDTO other)
        {
            if (ReferenceEquals(week, null))
            {
                return !ReferenceEquals(other, null);
            }

            return !week.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WeekDTO);
        }

        public bool Equals(WeekDTO obj)
        {
            return obj != null
                && obj.WeekStart.Date == WeekStart.Date;
        }

        public override int GetHashCode()
        {
            return WeekStart.Date.GetHashCode();
        }
    }
}
